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
        private const float TargetTreshold = 0.2f;
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

            foreach (var (projectileC, transformC, teamC, projectileEntity) in 
                     SystemAPI.Query<RefRW<ProjectileComponent>, RefRW<LocalTransform>, RefRO<TeamComponent>>().WithEntityAccess())
            {
                var directionNorm = math.normalize(projectileC.ValueRO.TargetPosition - transformC.ValueRO.Position);
                var step = projectileC.ValueRO.Speed * deltaTime;
                var newPosition = transformC.ValueRO.Position + directionNorm * step;
                transformC.ValueRW.Position = newPosition;

                // AOE
                if (projectileC.ValueRO.AoeRadius > 0)
                {
                    Debug.Assert(projectileC.ValueRO.PenetrationCounter == 0, "Aoe projectiles should not have penetrations set");
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
                            projectileC.ValueRO.AoeRadius,
                            projectileC.ValueRO.KnockBack
                            );
                        ecb.AddComponent<DestroyComponent>(projectileEntity);
                    }
                }
                // NOT AOE
                else
                {
                    var distanceToTargetPosition = Utils.Distance2DSqr(
                        transformC.ValueRO.Position, 
                        projectileC.ValueRO.TargetPosition);
                        
                    const float hitDistanceSqr = TargetTreshold * TargetTreshold;
                    if (distanceToTargetPosition < hitDistanceSqr)
                    {
                        ecb.AddComponent<DestroyComponent>(projectileEntity);
                    }
                    // check if for any hit
                    else
                    {
                        foreach (var (otherLocalPosC, otherTeamC, otherHpC, otherEntity) in
                                 SystemAPI.Query<RefRO<LocalTransform>, RefRW<TeamComponent>, RefRO<HpComponent>>()
                                     .WithEntityAccess())
                        {
                            if (otherTeamC.ValueRO.Team == teamC.ValueRO.Team)
                                continue;
                            if (otherHpC.ValueRO.Hp <=0)
                                continue;
                            if (Utils.Distance2DSqr(otherLocalPosC.ValueRO.Position, transformC.ValueRO.Position) > hitDistanceSqr)
                                continue;

                            bool otherEntityAlreadyHit = false;
                            for (int i = 0; i < projectileC.ValueRO.HitEntities.Length; i++)
                            {
                                if (projectileC.ValueRO.HitEntities[i] == otherEntity)
                                {
                                    otherEntityAlreadyHit = true;
                                    break;
                                }
                            }
                            if (otherEntityAlreadyHit)
                                continue;
                            
                            
                            // we hit enemy
                            VisualEffectUtils.PlayEffect(ref state, ref ecb, localTransformLookup[otherEntity].Position, "effect_hit_small");
                            AttackUtils.ApplyMeleeDamage(ref state, ref ecb, otherEntity,  projectileC.ValueRO.Damage, projectileC.ValueRO.KnockBack);

                            if (projectileC.ValueRO.PenetrationCounter == 0)
                            {
                                ecb.AddComponent<DestroyComponent>(projectileEntity);
                            }
                            else
                            {
                                projectileC.ValueRW.HitEntities.Add(otherEntity);
                                projectileC.ValueRW.PenetrationCounter--;
                            }
                        }
                    }
                }
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}