using CastlePrototype.Battle.Logic.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace CastlePrototype.Battle.Logic.Systems
{
    [DisableAutoCreation]
    public partial struct TargetingSystem : ISystem 
    {
        private ComponentLookup<LocalTransform> transformLookup;
        private ComponentLookup<SettingComponent> settingLookup;
        private ComponentLookup<HpComponent> hpLookup;
        private ComponentLookup<TeamComponent> teamLookup;
        private ComponentLookup<TargetComponent> targetLookup;
        private ComponentLookup<TargetedComponent> targetedLookup;

        
        private const float TargetingDelay = 0.5f;
        private float delayTimer;
        public void OnCreate(ref SystemState state)
        {
            delayTimer = TargetingDelay;
            transformLookup = state.GetComponentLookup<LocalTransform>();
            settingLookup = state.GetComponentLookup<SettingComponent>(true);
            hpLookup = state.GetComponentLookup<HpComponent>(true);
            teamLookup = state.GetComponentLookup<TeamComponent>(true);
            targetLookup = state.GetComponentLookup<TargetComponent>();
            targetedLookup = state.GetComponentLookup<TargetedComponent>();
        }
        
        public void OnUpdate(ref SystemState state)
        {
            if (delayTimer > 0)
            {
                delayTimer -= SystemAPI.Time.DeltaTime;
                return;
            }
            transformLookup.Update(ref state);
            settingLookup.Update(ref state);
            hpLookup.Update(ref state);
            teamLookup.Update(ref state);
            targetLookup.Update(ref state);
            targetedLookup.Update(ref state);

            
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            // Remove all not active targeters
            foreach (var targetedC in SystemAPI.Query<RefRW<TargetedComponent>>())
            {
                for (int i = targetedC.ValueRO.Targeters.Length-1; i >= 0; i--)
                {
                    if (!state.EntityManager.Exists(targetedC.ValueRO.Targeters[i]))
                    {
                        targetedC.ValueRW.Targeters.RemoveAt(i);
                    }
                }
            }
   
         
            foreach (var (transform, attackC, settingC, entity) in 
                     SystemAPI.Query<RefRO<LocalTransform>, RefRO<AttackComponent>, RefRO<SettingComponent>>()
                         .WithAll<TeamComponent>()
                         .WithEntityAccess())
            {
                
                if (HasValidTarget(ref state, entity, attackC.ValueRO.AttackDistance))
                    continue; 
                
                var targetEntity = FindAndAssignTarget(ref state, ref ecb, entity, transform.ValueRO.Position, 
                    attackC.ValueRO.AttackDistance, settingC.ValueRO.DistanceAxes);
                UpdateTarget(ref ecb, entity, targetEntity);
            }
            
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
 
        private Entity FindAndAssignTarget(ref SystemState state, ref EntityCommandBuffer ecb, Entity myEntity, 
            float3 myPosition, float myTargetRange, float3 myDistanceAxes)
        {
            float myTargetRangeSqr = myTargetRange * myTargetRange;
            var myTeam = teamLookup.GetRefRO(myEntity).ValueRO.Team;

            var closestDistance = float.MaxValue;
            var closestEntity = Entity.Null;
            var closestEntityWithNoTarget = Entity.Null;
            var closestDistanceWithNoTarget = float.MaxValue;
 
            
            foreach (var (otherTeamC, otherTrC, otherSettingC, otherHpC, otherEntity) in 
                     SystemAPI.Query<
                             RefRO<TeamComponent>, 
                             RefRO<LocalTransform>, 
                             RefRO<SettingComponent>, 
                             RefRO<HpComponent>>()
                         .WithEntityAccess())
            {
                // Skip entities on the same team or with zero health
                if (otherTeamC.ValueRO.Team == myTeam || otherHpC.ValueRO.Hp <= 0)
                    continue;

                // Calculate distance
                var distanceSqr = Utils.DistanceSqr(
                    myPosition, myDistanceAxes, 
                    otherTrC.ValueRO.Position, otherSettingC.ValueRO.DistanceAxes);
          
                // Skip entities out of range
                if (distanceSqr > myTargetRangeSqr)
                    continue;
            
                // Update closest target
                if (distanceSqr < closestDistance)
                {
                    closestDistance = distanceSqr;
                    closestEntity = otherEntity;
                }
                // Update closest with no target
                if (distanceSqr < closestDistanceWithNoTarget 
                    && (!targetedLookup.HasComponent(otherEntity) || targetedLookup[otherEntity].Targeters.IsEmpty))
                {
                    closestDistanceWithNoTarget = distanceSqr;
                    closestEntityWithNoTarget = otherEntity;
                }
            }
            return closestEntityWithNoTarget != Entity.Null ? closestEntityWithNoTarget : closestEntity;
        }


        private void AssignTarget(ref EntityCommandBuffer ecb, Entity mainTargetEntity, Entity targetEntity)
        {
            // SAME TARGET
            if (targetLookup.HasComponent(mainTargetEntity) && targetLookup[mainTargetEntity].Target == targetEntity)
                return;
            
            TryReleaseTarget(ref ecb, mainTargetEntity);
            ecb.AddComponent(mainTargetEntity, new TargetComponent { Target = targetEntity });

            if (targetedLookup.HasComponent(targetEntity))
            {
                targetedLookup[targetEntity].Targeters.Add(mainTargetEntity);
            }
            else
            {
                ecb.AddComponent(targetEntity, new TargetedComponent
                {
                    Targeters = new FixedList512Bytes<Entity>{mainTargetEntity}
                });   
            }
        }
        private void TryReleaseTarget(ref EntityCommandBuffer ecb, Entity mainTargetEntity)
        {
            if (!targetLookup.HasComponent(mainTargetEntity))
                return;
            
            if (targetLookup[mainTargetEntity].Target != Entity.Null)
            {
                var targeters = targetedLookup[targetLookup[mainTargetEntity].Target].Targeters;
                for (int i = 0; i < targeters.Length; i++)
                {
                    if (targeters[i] == mainTargetEntity)
                    {
                        targeters.RemoveAt(i);
                        break;
                    }
                }
            }
            ecb.RemoveComponent<TargetComponent>(mainTargetEntity);
        }
        private void UpdateTarget(ref EntityCommandBuffer ecb, Entity entityLookingForTarget, Entity targetEntity)
        {
            //NO TARGET FOUND -> TRY RELEASE EXISTING TARGET
            if (targetEntity == Entity.Null)
            {
                TryReleaseTarget(ref ecb, entityLookingForTarget);
            }
            else
            {
                AssignTarget(ref ecb, entityLookingForTarget, targetEntity);
            }
        }


        private bool HasValidTarget(ref SystemState state, Entity entity, float targetDistance)
        {
            if (!targetLookup.HasComponent(entity))
                return false;

            if (targetLookup[entity].Target == Entity.Null)
                return false;
            
            if (!state.EntityManager.Exists(targetLookup[entity].Target))
                return false;

            var target = targetLookup[entity].Target;
            if (!hpLookup.HasComponent(target) || hpLookup[target].Hp <= 0)
                return false;
            
            var distanceSqr = Utils.DistanceSqr(
                transformLookup[entity].Position, settingLookup[entity].DistanceAxes,
                transformLookup[target].Position, settingLookup[target].DistanceAxes);
             
          
            if (distanceSqr > targetDistance * targetDistance)
                return false;

            return true;
        }
    }
}