using CastlePrototype.Battle.Logic.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

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


        public void OnCreate(ref SystemState state)
        {
            transformLookup = state.GetComponentLookup<LocalTransform>(true);
            settingLookup = state.GetComponentLookup<SettingComponent>(true);
            hpLookup = state.GetComponentLookup<HpComponent>(true);
            teamLookup = state.GetComponentLookup<TeamComponent>(true);
            targetLookup = state.GetComponentLookup<TargetComponent>(false);
        }
        
        public void OnUpdate(ref SystemState state)
        {
            transformLookup.Update(ref state);
            settingLookup.Update(ref state);
            hpLookup.Update(ref state);
            teamLookup.Update(ref state);
            targetLookup.Update(ref state);
            
            var ecb = new EntityCommandBuffer(Allocator.Temp);
                
            // Process entities with no target
            foreach (var (transform, attackC, settingC, entity) in 
                     SystemAPI.Query<RefRO<LocalTransform>, RefRO<AttackComponent>, RefRO<SettingComponent>>()
                         .WithNone<TargetComponent>()
                         .WithAll<TeamComponent>()
                         .WithEntityAccess())
            {
                FindAndAssignTarget(ref state, ref ecb, entity, transform.ValueRO.Position, 
                    attackC.ValueRO.TargetRange, settingC.ValueRO.DistanceAxes);
            }
            
            // Process entities with existing target
            foreach (var (transform, attackC, targetC, settingC, entity) in 
                     SystemAPI.Query<RefRO<LocalTransform>, RefRO<AttackComponent>, RefRO<TargetComponent>, RefRO<SettingComponent>>()
                         .WithAll<TeamComponent>()
                         .WithEntityAccess())
            {
                FindAndAssignTarget(ref state, ref ecb, entity, transform.ValueRO.Position, 
                    attackC.ValueRO.TargetRange, settingC.ValueRO.DistanceAxes);
                //CheckExistingTarget(ref state, ref ecb, entity, transform.ValueRO.Position, attackC.ValueRO.TargetRange, 
                //    targetC.ValueRO.Target, settingC.ValueRO.DistanceAxes);
            }
            
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }


        private void CheckExistingTarget(ref SystemState state, ref EntityCommandBuffer ecb, Entity entity,
            float3 myPosition, float myTargetRange, Entity currentTarget, float3 myDistanceAxes)
        {
            if (!transformLookup.HasComponent(currentTarget) || 
                !hpLookup.HasComponent(currentTarget) ||
                hpLookup.GetRefRO(currentTarget).ValueRO.Hp <= 0)
            {
                ecb.RemoveComponent<TargetComponent>(entity);
                return;
            }
           
            var targetPosition = transformLookup.GetRefRO(currentTarget).ValueRO.Position;
            var targetDistanceAxes = settingLookup.GetRefRO(currentTarget).ValueRO.DistanceAxes;
            
            var distance = Utils.DistanceSqr(myPosition, myDistanceAxes, targetPosition, targetDistanceAxes);
            if (distance > myTargetRange * myTargetRange)
            {
                ecb.RemoveComponent<TargetComponent>(entity);
            }
       }
        
        private void FindAndAssignTarget(ref SystemState state, ref EntityCommandBuffer ecb, Entity myEntity, 
            float3 myPosition, float myTargetRange, float3 myDistanceAxes)
        {
            // Pre-square the target range to avoid repeated calculations
            float myTargetRangeSqr = myTargetRange * myTargetRange;
    
            // Use component lookups instead of EntityManager
            var myTeam = teamLookup.GetRefRO(myEntity).ValueRO.Team;
    
            // Track the closest target directly without storing all potentials
            var closestDistance = float.MaxValue;
            var closestEntity = Entity.Null;

            // Single pass to find the closest target
            foreach (var (otherTeamC, otherTrC, otherSettingC, otherHpC, otherEntity) in
                     SystemAPI.Query<RefRO<TeamComponent>, RefRO<LocalTransform>, RefRO<SettingComponent>, RefRO<HpComponent>>()
                         .WithEntityAccess())
            {
                // Skip entities on the same team or with no health
                if (otherTeamC.ValueRO.Team == myTeam || otherHpC.ValueRO.Hp <= 0)
                    continue;

                // Calculate distance
                var distanceSqr = Utils.DistanceSqr(
                    myPosition, myDistanceAxes, 
                    otherTrC.ValueRO.Position, otherSettingC.ValueRO.DistanceAxes);
          
                // Skip entities out of range
                if (distanceSqr > myTargetRangeSqr)
                    continue;
            
                // Update closest target if this one is closer
                if (distanceSqr < closestDistance)
                {
                    closestDistance = distanceSqr;
                    closestEntity = otherEntity;
                }
            }

            if (closestEntity == Entity.Null)
            {
                if (state.EntityManager.HasComponent<TargetComponent>(myEntity))
                    ecb.RemoveComponent<TargetComponent>(myEntity);
            }
            else
            {
                if (state.EntityManager.HasComponent<TargetComponent>(myEntity))
                {
                    ref var targetC = ref targetLookup.GetRefRW(myEntity).ValueRW;
                    targetC.Target = closestEntity;
                }
                else
                {
                    ecb.AddComponent(myEntity, new TargetComponent { Target = closestEntity });
                }
            }
        }
    }
}