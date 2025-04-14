using CastlePrototype.Battle.Logic.Components;
using CastlePrototype.Battle.Logic.EcsUtils;
using TowerDefensePrototype.Battle.Visuals.Effects;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;

namespace CastlePrototype.Battle.Logic.Systems
{
    [DisableAutoCreation]
    public partial struct ProjectileSystem : ISystem
    {
        private const float TargetTreshold = 0.4f;
        private ComponentLookup<LocalTransform> localTransformLookup;
        private EntityQuery aoeDamageEntityQuery;
        
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BattleFieldComponent>();
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

                // AOE ONLY - aka trajectory projectiles that hits ground only
                if (projectileC.ValueRO.AoeRadius > 0 && projectileC.ValueRO.AoeOnly)
                {
                    Debug.Assert(projectileC.ValueRO.PenetrationCounter == 0, "Aoe projectiles should not have penetrations set");
                    var distanceToInitialTargetPositionSqr = Utils.Distance2DSqr(
                        transformC.ValueRO.Position, 
                        projectileC.ValueRO.TargetPosition);
                    
                    if (distanceToInitialTargetPositionSqr < TargetTreshold * TargetTreshold)
                    {
                        VisualEffectUtils.PlayEffect(ref state, ref ecb, projectileC.ValueRO.TargetPosition, EffectKeys.HitEffectAoeNormal);
                        
                        AttackUtils.ApplyAoeDamage(
                            ref state,
                            ref ecb,
                            ref aoeDamageEntityQuery,
                            transformC.ValueRO.Position,
                            new float3(1, 0, 1),
                            projectileC.ValueRO.AttackerTeam,
                            projectileC.ValueRO.AoeDamage,
                            projectileC.ValueRO.AoeRadius,
                            projectileC.ValueRO.KnockBack,
                            Entity.Null
                            );
                        ecb.AddComponent<DestroyComponent>(projectileEntity);
                    }
                }
                // NOT AOE or projectile with aoe damage after enemy hit
                else
                {
                    var distanceToTargetPosition = Utils.Distance2DSqr(
                        transformC.ValueRO.Position, 
                        projectileC.ValueRO.TargetPosition);
                        
                    const float hitDistanceSqr = TargetTreshold * TargetTreshold;
                    if (distanceToTargetPosition < hitDistanceSqr)
                    {
                        if (projectileC.ValueRO.BounceCounter > 0)
                        {
                            var battleFieldC = SystemAPI.GetSingleton<BattleFieldComponent>();
                            
                            var bouncedDirection =
                                Utils.ComputeBounce(projectileC.ValueRO.Direction, projectileC.ValueRO.EdgeNormal);

                            var intersectionResult = Utils.CalculateIntersectionFromRectangleInside(
                                battleFieldC.MinCorner,
                                battleFieldC.MaxCorner,
                                Utils.To2D(transformC.ValueRO.Position),
                                Utils.To2D(bouncedDirection));
                            Debug.Assert(intersectionResult.HasIntersection);
                            
                            
                            projectileC.ValueRW.TargetPosition = Utils.To3D(intersectionResult.ExitPoint);
                            projectileC.ValueRW.EdgeNormal = Utils.To3D(intersectionResult.Normal);
                            
                            projectileC.ValueRW.BounceCounter--;
                        }
                        else
                        {
                            if (projectileC.ValueRO.AoeRadius > 0)
                            {
                                VisualEffectUtils.PlayEffect(ref state, ref ecb, projectileC.ValueRO.TargetPosition,
                                    EffectKeys.HitEffectAoeNormal);
                                AttackUtils.ApplyAoeDamage(
                                    ref state,
                                    ref ecb,
                                    ref aoeDamageEntityQuery,
                                    transformC.ValueRO.Position,
                                    new float3(1, 0, 1),
                                    projectileC.ValueRO.AttackerTeam,
                                    projectileC.ValueRO.AoeDamage,
                                    projectileC.ValueRO.AoeRadius,
                                    projectileC.ValueRO.KnockBack,
                                    Entity.Null
                                );
                            }

                            ecb.AddComponent<DestroyComponent>(projectileEntity);
                        }
                    }
                    // check if for any hit
                    else
                    {
                        // check all enemy entities around for hit
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
                            VisualEffectUtils.PlayEffect(ref state, ref ecb, localTransformLookup[otherEntity].Position, EffectKeys.HitEffectSmall);
                            AttackUtils.ApplyMeleeDamage(ref state, ref ecb, otherEntity,  projectileC.ValueRO.Damage, projectileC.ValueRO.KnockBack);

                            if (projectileC.ValueRO.AoeRadius > 0)
                            {
                                // make aoe damage on other units as well
                                VisualEffectUtils.PlayEffect(ref state, ref ecb, projectileC.ValueRO.TargetPosition, EffectKeys.HitEffectAoeNormal);
                        
                                AttackUtils.ApplyAoeDamage(
                                    ref state,
                                    ref ecb,
                                    ref aoeDamageEntityQuery,
                                    transformC.ValueRO.Position,
                                    new float3(1, 0, 1),
                                    projectileC.ValueRO.AttackerTeam,
                                    projectileC.ValueRO.AoeDamage,
                                    projectileC.ValueRO.AoeRadius,
                                    projectileC.ValueRO.KnockBack,
                                    // skip other entity, it already got direct hit
                                    otherEntity
                                );
                            }
                            
                            if (projectileC.ValueRO.PenetrationCounter == 0)
                            {
                                ecb.AddComponent<DestroyComponent>(projectileEntity);
                            }
                            else
                            {
                                // ad hit entity to list of hit entities to prevent next hit when projectiles continues flyes
                                projectileC.ValueRW.HitEntities.Add(otherEntity);
                                projectileC.ValueRW.PenetrationCounter--;
                            }
                            
                            // we hit just ones
                            break;
                        }
                    }
                }
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}