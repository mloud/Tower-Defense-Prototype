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
            ref LocalTransform attackerTransform,
            Entity targetEntity,
            float3 attackerPosition, 
            Team attackerTeam,
            float3 targetPosition,
            float2 minFieldCoordinate,
            float2 maxFieldCoordinate,
            float damage,
            float aoeRadius, 
            float projectileSpeed,
            bool knockBack,
            int penetrations,
            FixedString64Bytes visualId)
        {
            var direction = targetPosition - attackerPosition;

            float3 projectileDestination3D = targetPosition;

            // NOT AOE projectiles flies to the edge of playground
            if (aoeRadius <= 0)
            {
                var projectileDestination2D = Utils.CalculateIntersectionFromRectangleInside(
                    minFieldCoordinate,
                    maxFieldCoordinate,
                    new float2(attackerPosition.x, attackerPosition.z),
                    new float2(direction.x, direction.z)).ExitPoint;
                projectileDestination3D = new float3(projectileDestination2D.x, 0, projectileDestination2D.y);
            }

            //rotate attacker towards target
            var targetDirection = Quaternion.LookRotation(direction);
            attackerTransform.Rotation = targetDirection;
                            
            var projectile = ecb.CreateEntity();
            ecb.AddComponent(projectile, new ProjectileComponent
            {
                Target = targetEntity,
                TargetPosition = projectileDestination3D,
                Speed = projectileSpeed,
                Damage = damage,
                AttackerTeam = attackerTeam,
                AoeRadius = aoeRadius,
                KnockBack = knockBack,
                PenetrationCounter = penetrations
            });

            ecb.AddComponent(projectile, new LocalTransform
            {
                Position = attackerPosition,
                Rotation = targetDirection,
                Scale = 1f
            });

            ecb.AddComponent(projectile, new VisualComponent { VisualId = visualId });
            
            ecb.AddComponent(projectile, new TeamComponent {Team =attackerTeam });
        }
        
        
        public static void ApplyAoeDamage(
            ref SystemState state,
            ref EntityCommandBuffer ecb,
            ref EntityQuery aoeDamageEntityQuery,
            float3 attackerPosition,
            float3 attackerDistanceAxes,
            Team attackerTeam,
            float damage,
            float radius,
            bool knockBack)
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
                // Check team
                if (teamArray[i].Team == attackerTeam)
                    continue;

                // Check HP
                if (hpArray[i].Hp <= 0)
                    continue;

                // Check distance
                var sqrDistance = Utils.DistanceSqr(
                    attackerPosition, attackerDistanceAxes,
                    transformArray[i].Position, settingArray[i].DistanceAxes);

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