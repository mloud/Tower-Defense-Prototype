using CastlePrototype.Battle.Logic.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace CastlePrototype.Battle.Logic.EcsUtils
{
    public static class AttackUtils
    {
        public static void ApplyMeleeDamage(
            ref SystemState state, 
            ref EntityCommandBuffer ecb, 
            Entity entity, 
            float damage, 
            bool knockBack)
        {
            ecb.AddComponent(entity, new DamageComponent
            {
                Damage = damage,
                Knockback = knockBack
            });
        }
        
        public static void ShootProjectile(
            ref SystemState state,
            ref EntityCommandBuffer ecb, 
            in AttackComponent attackComponent, 
            Entity attackerEntity,
            Entity targetEntity,
            float3 attackerPosition, 
            in SettingComponent attackerSettingsC,
            Team attackerTeam,
            float3 targetPosition,
            float2 minFieldCoordinate,
            float2 maxFieldCoordinate)
        {
            var direction = targetPosition - attackerPosition;
            var edgeNormal = float2.zero;
            float3 projectileDestination3D = targetPosition;
   
            
            bool targetPositionOnEdge = false;
            // NOT AOE projectiles flies to the edge of playground
            if (attackComponent.AoeRadius <= 0)
            {
                var intersectionResult = Utils.CalculateIntersectionFromRectangleInside(
                    minFieldCoordinate,
                    maxFieldCoordinate,
                    new float2(attackerPosition.x, attackerPosition.z),
                    new float2(direction.x, direction.z));
                
                var projectileDestination2D = intersectionResult.ExitPoint;
                projectileDestination3D = Utils.To3D(projectileDestination2D);
                edgeNormal = intersectionResult.Normal;
                targetPositionOnEdge = true;
            }

            //rotate attacker towards target
            var targetDirection = Quaternion.LookRotation(direction);
            //attackerTransform.Rotation = targetDirection;
            var projectile = ecb.CreateEntity();
            ecb.AddComponent(projectile, new ProjectileComponent
            {  
                Target = targetEntity,
                TargetPosition = projectileDestination3D,
                Direction = math.normalize(direction),
                Speed = attackComponent.ProjectileSpeed,
                Damage = attackComponent.AttackDamage,
                AoeDamage = attackComponent.AoeDamage,
                AoeOnly = attackComponent.AoeOnly,
                AttackerTeam = attackerTeam,
                AoeRadius = attackComponent.AoeRadius,
                KnockBack = attackComponent.KnockBack,
                PenetrationCounter = attackComponent.Penetration,
                BounceCounter = targetPositionOnEdge ? attackComponent.Bounce : -1,
                EdgeNormal = Utils.To3D(edgeNormal),
            });

            ecb.AddComponent(projectile, new LocalTransform
            {
                Position = attackerPosition,
                Rotation = targetDirection,
                Scale = 1f
            });

            ecb.AddComponent(projectile, new VisualComponent { VisualId = attackComponent.ProjectileVisualId });
            ecb.AddComponent(projectile, new TeamComponent {Team =attackerTeam });
            ecb.AddComponent(projectile, new SettingComponent { DistanceAxes = new float3(1,0,1), Radius = 0});

        }
        
        
        public static void ApplyAoeDamage(
            ref SystemState state,
            ref EntityCommandBuffer ecb,
            ref EntityQuery aoeDamageEntityQuery,
            float3 attackerPosition,
            in SettingComponent attackerSettings,
            Team attackerTeam,
            float damage,
            float radius,
            bool knockBack,
            Entity skipEntity)
        {
            var aoeRadiusSqr = radius * radius;

        
            // Get component arrays from the query
            var hpArray = aoeDamageEntityQuery.ToComponentDataArray<HpComponent>(Allocator.Temp);
            var transformArray = aoeDamageEntityQuery.ToComponentDataArray<LocalTransform>(Allocator.Temp);
            var settingArray = aoeDamageEntityQuery.ToComponentDataArray<SettingComponent>(Allocator.Temp);
            var teamArray = aoeDamageEntityQuery.ToComponentDataArray<TeamComponent>(Allocator.Temp);
            var entities = aoeDamageEntityQuery.ToEntityArray(Allocator.Temp);

            for (int i = 0; i < entities.Length; i++)
            {
                // skip this entity
                if (entities[i] == skipEntity)
                    continue;
                
                // Check team
                if (teamArray[i].Team == attackerTeam)
                    continue;

                // Check HP
                if (hpArray[i].Hp <= 0)
                    continue;

                // Check distance
                var sqrDistance = Utils.VolumeDistanceSqr(
                    attackerPosition, 
                    attackerSettings,
                    transformArray[i].Position, 
                    settingArray[i]);

                if (sqrDistance < aoeRadiusSqr)
                {
                    ecb.AddComponent(entities[i], new DamageComponent { Damage = damage, Knockback = knockBack});
                }
            }

            // Dispose of the arrays
            hpArray.Dispose();
            transformArray.Dispose();
            settingArray.Dispose();
            teamArray.Dispose();
            entities.Dispose();
        }
    }
}