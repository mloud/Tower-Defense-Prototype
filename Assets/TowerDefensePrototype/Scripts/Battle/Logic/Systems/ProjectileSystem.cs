using CastlePrototype.Battle.Logic.Components;
using CastlePrototype.Battle.Logic.EcsUtils;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;

namespace CastlePrototype.Battle.Logic.Systems
{
    [DisableAutoCreation]
    public partial struct ProjectileSystem : ISystem
    {
        private const float TargetTreshold = 0.7f;
        private ComponentLookup<LocalTransform> localTransformLookup;
        private EntityQuery aoeDamageEntityQuery;
        
        public void OnCreate(ref SystemState state)
        {
            aoeDamageEntityQuery = state.GetEntityQuery(
                ComponentType.ReadWrite<HpComponent>(),
                ComponentType.ReadOnly<LocalTransform>(),
                ComponentType.ReadOnly<SettingComponent>(),
                ComponentType.ReadOnly<TeamComponent>());
            localTransformLookup = state.GetComponentLookup<LocalTransform>(true);
        }
        
        public void OnUpdate(ref SystemState state)
        {
            localTransformLookup.Update(ref state);
            
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            var deltaTime = SystemAPI.Time.DeltaTime;

            foreach (var (projectileC, transformC, projectileEntity) in 
                     SystemAPI.Query<RefRW<ProjectileComponent>, RefRW<LocalTransform>>().WithEntityAccess())
            {
                var directionNorm = math.normalize(projectileC.ValueRO.TargetPosition - transformC.ValueRO.Position);
                var step = projectileC.ValueRO.Speed * deltaTime;
                var newPosition = transformC.ValueRO.Position + directionNorm * step;
                transformC.ValueRW.Position = newPosition;

                // AOE
                if (projectileC.ValueRO.AoeRadius > 0)
                {
                    var distanceToInitialTargetPositionSqr = Utils.Distance2DSqr(
                        transformC.ValueRO.Position, 
                        projectileC.ValueRO.TargetPosition);
                    
                    if (distanceToInitialTargetPositionSqr < TargetTreshold * TargetTreshold)
                    {
                        VisualEffectUtils.PlayEffect(ref state, ref ecb, projectileC.ValueRO.TargetPosition, "effect_hit_aoe");
                        
                        AttackUtils.ApplyAoeDamage(
                            ref state,
                            ref ecb,
                            ref aoeDamageEntityQuery,
                            transformC.ValueRO.Position,
                            new float3(1, 0, 1),
                            projectileC.ValueRO.AttackerTeam,
                            projectileC.ValueRO.Damage,
                            projectileC.ValueRO.AoeRadius);
                        ecb.AddComponent<DestroyComponent>(projectileEntity);
                    }
                }
                // NOT AOE
                else
                {
                    // Target still alive
                    if (state.EntityManager.Exists(projectileC.ValueRO.Target))
                    {
                        var distanceToCurrentTargetPositionSqr = Utils.Distance2DSqr(
                            transformC.ValueRO.Position,
                            localTransformLookup[projectileC.ValueRO.Target].Position);
                        // we hit the target on the way
                        if (distanceToCurrentTargetPositionSqr < TargetTreshold * TargetTreshold)
                        {
                            VisualEffectUtils.PlayEffect(ref state, ref ecb, localTransformLookup[projectileC.ValueRO.Target].Position, "effect_hit_small");
                            AttackUtils.ApplyMeleeDamage(ref state, ref ecb, projectileC.ValueRO.Target,  projectileC.ValueRO.Damage);
                            ecb.AddComponent<DestroyComponent>(projectileEntity);
                        }
                        // we reached target position
                        else
                        {
                            var distanceToInitialTargetPositionSqr = Utils.Distance2DSqr(
                                transformC.ValueRO.Position, 
                                projectileC.ValueRO.TargetPosition);
                        
                            if (distanceToInitialTargetPositionSqr < TargetTreshold * TargetTreshold)
                            {
                                ecb.AddComponent<DestroyComponent>(projectileEntity);
                            }
                        }
                    }
                    // target destroyed
                    else
                    {
                        var distanceToInitialTargetPositionSqr = Utils.Distance2DSqr(
                            transformC.ValueRO.Position, 
                            projectileC.ValueRO.TargetPosition);
                        // we reached target position
                        if (distanceToInitialTargetPositionSqr < TargetTreshold * TargetTreshold)
                        {
                            ecb.AddComponent<DestroyComponent>(projectileEntity);
                        }
                    }
                }
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}