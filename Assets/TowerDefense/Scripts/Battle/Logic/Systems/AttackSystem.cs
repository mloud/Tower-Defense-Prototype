using TowerDefense.Battle.Logic.Components;
using TowerDefense.Battle.Logic.EcsUtils;
using TowerDefense.Data;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace TowerDefense.Battle.Logic.Systems
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
          
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            foreach (var (attackC, transformC, targetC, settingC, teamC, attackerEntity) in
                     SystemAPI.Query<
                         RefRW<AttackComponent>,
                         RefRW<LocalTransform>,
                         RefRW<TargetComponent>,
                         RefRW<SettingComponent>,
                         RefRW<TeamComponent>>().WithEntityAccess())
            {
                bool isInAttackRange = false;
                bool isManualTargetingActive = state.EntityManager.HasComponent<ManualTargetingComponent>(attackerEntity);
                Entity targetEntity;
                float3 targetPosition = float3.zero;
                if (isManualTargetingActive)
                {
                    targetEntity = Entity.Null;
                    targetPosition = transformC.ValueRO.Position + math.forward(transformC.ValueRO.Rotation);
                    isInAttackRange = true;
                }
                else
                {
                    targetEntity = targetC.ValueRO.Target;
                    // spread attack does not need target
                    if (attackC.ValueRO.SecondaryAttackAngles.Length > 0)
                    {
                        isInAttackRange = true;
                        float angleDeg = attackC.ValueRO.SecondaryAttackAngles[0];
                        float angleRad = math.radians(angleDeg);

                        float3 forward = math.forward(quaternion.Euler(0, angleRad, 0));
                        targetPosition = transformC.ValueRO.Position + forward;
                    }
                    // some entity is targeted
                    else if (targetEntity != Entity.Null)
                    {
                        var targetPositionC = transformLookup.GetRefRO(targetEntity);
                        var targetSettingsC = settingLookup.GetRefRO(targetEntity);
                        targetPosition = targetPositionC.ValueRO.Position;

                        var sqrDistance = Utils.VolumeDistanceSqr(
                            transformC.ValueRO.Position, 
                            settingC.ValueRO,
                            targetPositionC.ValueRO.Position, 
                            targetSettingsC.ValueRO);
                        isInAttackRange = sqrDistance < attackC.ValueRO.AttackDistance * attackC.ValueRO.AttackDistance;
                    }
                    else
                    {
                        isInAttackRange = false;
                    }
                }

                if (isInAttackRange)
                {
                    targetC.ValueRW.TargetPosition = targetPosition;
                    attackC.ValueRW.IsInAttackDistance = true;

                    // update attack remaining times
                    double timeToAttack = float.MaxValue;
                    bool isMainAttack = true;
                    for (int i = 0; i < attackC.ValueRO.SecondaryAttackTimes.Length; i++)
                    {
                        attackC.ValueRW.SecondaryAttackTimes[i] -= SystemAPI.Time.DeltaTime;
                        timeToAttack = math.min(timeToAttack, attackC.ValueRW.SecondaryAttackTimes[i]);
                        if (timeToAttack <= 0)
                            isMainAttack = false;
                    }
                    attackC.ValueRW.NextMainAttackTime -= SystemAPI.Time.DeltaTime;
                    timeToAttack = math.min(timeToAttack, attackC.ValueRW.NextMainAttackTime);
                    // end of updating attack times
               
                    
                    if (!isManualTargetingActive && lookAtTargetLookup.HasComponent(attackerEntity))
                    {
                        const float timeBeforeLookAtTarget = 0.3f;
                        // starts to rotate towards target
                        lookAtTargetLookup.GetRefRW(attackerEntity).ValueRW.LookAtTarget = timeToAttack < timeBeforeLookAtTarget;
                    }

                    if (timeToAttack < attackC.ValueRO.AttackAnimDelay && !attackC.ValueRO.PlayAttackBlockedToNextAttack)
                    {
                        attackC.ValueRW.PlayAttackBlockedToNextAttack = true;
                        attackC.ValueRW.PlayAttack = true;
                    }
                    
                    if (timeToAttack < 0)
                    {
                        attackC.ValueRW.PlayAttackBlockedToNextAttack = false;
                        // prepare next attack(s)
                        if (isMainAttack)
                        {
                            attackC.ValueRW.NextMainAttackTime = attackC.ValueRO.Cooldown;
                            Debug.Assert(attackC.ValueRO.SecondaryAttackTimes.Length == 0, "There are unprocessed secondary attacks");
                            Debug.Assert(attackC.ValueRO.SecondaryAttackAngles.Length == 0, "There are unprocessed secondary attacks angles");

                            for (int i = 0; i < attackC.ValueRO.FireAgain; i++)
                            {
                                float secondaryAttackTimeDelay = attackC.ValueRO.FireAgainInterval;
                                attackC.ValueRW.SecondaryAttackTimes.Add((i+1) * secondaryAttackTimeDelay);
                            }

                            if (attackC.ValueRO.FireAgainSpread > 0)
                            {
                                var currentPosition = transformC.ValueRO.Position;
                                var currentDirection = targetPosition - currentPosition;
                                
                                float3 direction = math.normalize(currentDirection);
                                float yaw = math.atan2(direction.x, direction.z);
                                float yawDegrees = math.degrees(yaw);

                                float angleStep = yawDegrees;
                                if (attackC.ValueRO.FireAgain > 1)
                                {
                                    angleStep = (float)attackC.ValueRO.FireAgainSpread /
                                                (attackC.ValueRO.FireAgain - 1);
                                }

                                var startRotation = yawDegrees;
                                if (yawDegrees > 0)
                                    angleStep *= -1;
                                for (int i = 0; i < attackC.ValueRO.FireAgain; i++)
                                {
                                    var yRot = startRotation + i * angleStep;
                                    attackC.ValueRW.SecondaryAttackAngles.Add(yRot);
                                }
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
                                    attackC.ValueRO,
                                    attackerEntity,
                                    targetEntity, 
                                    transformC.ValueRO.Position,
                                    settingC.ValueRO,
                                    teamC.ValueRO.Team,
                                    targetPosition,
                                    battleFieldComponent.MinCorner,
                                    battleFieldComponent.MaxCorner);
                                break;
                            case AttackType.Aoe:
                                Debug.Assert(attackC.ValueRO.AttackDamage == 0, "Aoe attack should not have direct damage set");
                                AttackUtils.ApplyAoeDamage(
                                    ref state, 
                                    ref ecb,
                                    ref aoeDamageEntityQuery,
                                    transformC.ValueRO.Position, 
                                    settingC.ValueRO, 
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
                            if (attackC.ValueRO.SecondaryAttackAngles.Length > 0)
                            {
                                attackC.ValueRW.SecondaryAttackAngles.RemoveAt(0);
                            }
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