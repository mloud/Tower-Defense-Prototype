using CastlePrototype.Battle.Logic.Components;
using CastlePrototype.Battle.Logic.EcsUtils;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace CastlePrototype.Battle.Logic.Systems
{
    [DisableAutoCreation]
    public partial struct AttackSystem : ISystem 
    {
        private ComponentLookup<LocalTransform> transformLookup;
        private ComponentLookup<SettingComponent> settingLookup;
        private ComponentLookup<HpComponent> hpLookup;
        private ComponentLookup<TeamComponent> teamLookup;

        private EntityQuery aoeDamageEntityQuery;
        
        public void OnCreate(ref SystemState state)
        {
            transformLookup = state.GetComponentLookup<LocalTransform>(true);
            settingLookup = state.GetComponentLookup<SettingComponent>(true);
            hpLookup = state.GetComponentLookup<HpComponent>(true);
            teamLookup = state.GetComponentLookup<TeamComponent>(true);
            aoeDamageEntityQuery = state.GetEntityQuery(
                ComponentType.ReadWrite<HpComponent>(),
                ComponentType.ReadOnly<LocalTransform>(),
                ComponentType.ReadOnly<SettingComponent>(),
                ComponentType.ReadOnly<TeamComponent>());
        }

        public void OnUpdate(ref SystemState state)
        {
            transformLookup.Update(ref state);
            settingLookup.Update(ref state);
            hpLookup.Update(ref state);
            teamLookup.Update(ref state);
            
            var currentTime = SystemAPI.Time.ElapsedTime;
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            foreach (var (attackC, transformC, targetC, settingC, teamC) in
                     SystemAPI.Query<
                         RefRW<AttackComponent>,
                         RefRW<LocalTransform>,
                         RefRW<TargetComponent>,
                         RefRW<SettingComponent>,
                         RefRW<TeamComponent>>())
            {
                var targetPositionC = transformLookup.GetRefRO(targetC.ValueRO.Target);
                var targetSettingsC = settingLookup.GetRefRO(targetC.ValueRO.Target);
               
                var sqrDistance = Utils.DistanceSqr(
                    transformC.ValueRO.Position, settingC.ValueRO.DistanceAxes,
                    targetPositionC.ValueRO.Position, targetSettingsC.ValueRO.DistanceAxes);
                
                if (sqrDistance < attackC.ValueRO.AttackDistance * attackC.ValueRO.AttackDistance)
                {
                    attackC.ValueRW.IsInAttackDistance = true;
                    if (currentTime - attackC.ValueRO.LastAttackTime > attackC.ValueRO.AttackInterval)
                    {
                        attackC.ValueRW.PlayAttack = true;
                        attackC.ValueRW.LastAttackTime = currentTime;

                        switch (attackC.ValueRO.AttackType)
                        {
                            // MELEE
                            case AttackType.Melee:
                                AttackUtils.ApplyMeleeDamage(
                                    ref state, 
                                    ref ecb,
                                    targetC.ValueRO.Target, 
                                    attackC.ValueRO.AttackDamage);
                                break;
                            //RANGE
                            case AttackType.Range:
                                AttackUtils.ShootProjectile(
                                    ref state, 
                                    ref ecb, 
                                    ref transformC.ValueRW, 
                                    targetC.ValueRO.Target, 
                                    transformC.ValueRO.Position,
                                    teamC.ValueRO.Team,
                                    targetPositionC.ValueRO.Position,
                                    attackC.ValueRO.AttackDamage,
                                    attackC.ValueRO.AoeRadius,
                                    attackC.ValueRO.ProjectileSpeed,
                                    attackC.ValueRO.ProjectileVisualId
                                    );
                                break;
                            case AttackType.Aoe:
                                AttackUtils.ApplyAoeDamage(
                                    ref state, 
                                    ref ecb,
                                    ref aoeDamageEntityQuery,
                                    transformC.ValueRO.Position, 
                                    settingC.ValueRO.DistanceAxes, 
                                    teamC.ValueRO.Team,
                                    attackC.ValueRO.AttackDamage, 
                                    attackC.ValueRO.AoeRadius);
                                break;
                            default:
                                Debug.Assert(false, $"Unknown attack type {attackC.ValueRO.AttackType}");
                                break;
                        }          
                    }
                }
                else
                {
                    attackC.ValueRW.IsInAttackDistance = false;
                }
            }
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}