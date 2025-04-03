using CastlePrototype.Battle.Logic.Components;
using CastlePrototype.Battle.Logic.EcsUtils;
using CastlePrototype.Data;
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
        private ComponentLookup<LookAtTargetComponent> lookAtTargetLookup;


        private EntityQuery aoeDamageEntityQuery;
        
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BattleFieldComponent>();
            transformLookup = state.GetComponentLookup<LocalTransform>(true);
            settingLookup = state.GetComponentLookup<SettingComponent>(true);
            hpLookup = state.GetComponentLookup<HpComponent>(true);
            teamLookup = state.GetComponentLookup<TeamComponent>(true);
            lookAtTargetLookup = state.GetComponentLookup<LookAtTargetComponent>();

            
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
            lookAtTargetLookup.Update(ref state);
            
            var currentTime = SystemAPI.Time.ElapsedTime;
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            foreach (var (attackC, transformC, targetC, settingC, teamC, entity) in
                     SystemAPI.Query<
                         RefRW<AttackComponent>,
                         RefRW<LocalTransform>,
                         RefRW<TargetComponent>,
                         RefRW<SettingComponent>,
                         RefRW<TeamComponent>>().WithEntityAccess())
            {
                var targetPositionC = transformLookup.GetRefRO(targetC.ValueRO.Target);
                var targetSettingsC = settingLookup.GetRefRO(targetC.ValueRO.Target);
               
                var sqrDistance = Utils.DistanceSqr(
                    transformC.ValueRO.Position, settingC.ValueRO.DistanceAxes,
                    targetPositionC.ValueRO.Position, targetSettingsC.ValueRO.DistanceAxes);
                
                if (sqrDistance < attackC.ValueRO.AttackDistance * attackC.ValueRO.AttackDistance)
                {
                    attackC.ValueRW.IsInAttackDistance = true;

                    double attackTime = attackC.ValueRO.NextMainAttackTime;
                    bool isMainAttack = true;
                    //resolve attack time (secondary or main)
                    for (int i = 0; i < attackC.ValueRO.SecondaryAttackTimes.Length; i++)
                    {
                        if (currentTime > attackC.ValueRO.SecondaryAttackTimes[i])
                        {
                            attackTime = attackC.ValueRO.SecondaryAttackTimes[i];
                            isMainAttack = false;
                            break;
                        }
                    }

                    if (lookAtTargetLookup.HasComponent(entity))
                    {
                        const float timeBeforeLookAtTarget = 0.3f;
                        // starts to rotate towards target
                        lookAtTargetLookup.GetRefRW(entity).ValueRW.LookAtTarget = currentTime > attackTime - timeBeforeLookAtTarget;
                    }

                    
                    if (currentTime > attackTime)
                    {
                        attackC.ValueRW.PlayAttack = true;
                        // prepare next attack(s)
                        if (isMainAttack)
                        {     
                            attackC.ValueRW.LastAttackTime = currentTime;
                            attackC.ValueRW.NextMainAttackTime = currentTime + attackC.ValueRO.Cooldown;
                            Debug.Assert(attackC.ValueRO.SecondaryAttackTimes.Length == 0, "There are unprocessed secondary attacks");

                            for (int i = 0; i < attackC.ValueRO.FireAgain; i++)
                            {
                                double secondaryAttackTimeDelay = 0.5f;
                                attackC.ValueRW.SecondaryAttackTimes.Add(currentTime+(i+1) * secondaryAttackTimeDelay);
                            }
                        }
                        
                        switch (attackC.ValueRO.AttackType)
                        {
                            // MELEE
                            case AttackType.Melee:
                                AttackUtils.ApplyMeleeDamage(
                                    ref state, 
                                    ref ecb,
                                    targetC.ValueRO.Target, 
                                    attackC.ValueRO.AttackDamage, 
                                    attackC.ValueRO.KnockBack);
                                break;
                            //RANGE
                            case AttackType.Range:

                                var battleFieldComponent = SystemAPI.GetSingleton<BattleFieldComponent>();
                                
                                AttackUtils.ShootProjectile(
                                    ref state, 
                                    ref ecb, 
                                    ref transformC.ValueRW, 
                                    targetC.ValueRO.Target, 
                                    transformC.ValueRO.Position,
                                    teamC.ValueRO.Team,
                                    targetPositionC.ValueRO.Position,
                                    battleFieldComponent.MinCorner,
                                    battleFieldComponent.MaxCorner,
                                    attackC.ValueRO.AttackDamage,
                                    attackC.ValueRO.AoeDamage,
                                    attackC.ValueRO.AoeRadius,
                                    attackC.ValueRO.ProjectileSpeed,
                                    attackC.ValueRO.KnockBack,
                                    attackC.ValueRO.AoeOnly,
                                    attackC.ValueRO.Penetration,
                                    attackC.ValueRO.Bounce,
                                    attackC.ValueRO.ProjectileVisualId
                                    );
                                break;
                            case AttackType.Aoe:
                                Debug.Assert(attackC.ValueRO.AttackDamage == 0, "Aoe attack should not have direct damage set");
                                AttackUtils.ApplyAoeDamage(
                                    ref state, 
                                    ref ecb,
                                    ref aoeDamageEntityQuery,
                                    transformC.ValueRO.Position, 
                                    settingC.ValueRO.DistanceAxes, 
                                    teamC.ValueRO.Team,
                                    attackC.ValueRO.AoeDamage, 
                                    attackC.ValueRO.AoeRadius,
                                    attackC.ValueRO.KnockBack,
                                    Entity.Null);
                                break;
                            default:
                                Debug.Assert(false, $"Unknown attack type {attackC.ValueRO.AttackType}");
                                break;
                        }

                        if (!isMainAttack)
                        {
                            attackC.ValueRW.SecondaryAttackTimes.RemoveAt(0);
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