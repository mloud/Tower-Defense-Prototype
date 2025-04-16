using System;
using System.Collections.Generic;
using CastlePrototype.Battle.Logic.Components;
using CastlePrototype.Battle.Logic.Managers.Skills;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace CastlePrototype.Battle.Logic.EcsUtils
{
    public static class QueryUtils
    {

        public static Entity GetEntityForSkill(EntityManager entityManager, SkillType skillType) =>
            skillType switch
            {
                SkillType.UnlockHero => Entity.Null,
                SkillType.IncreaseHp => GetPlayerBarricade(entityManager),
                SkillType.IncreaseAttackDistance => GetRandomPlayerUnit(entityManager, true),
                _ => GetRandomPlayerUnit(entityManager, false)
            };

        public static Entity GetPlayerBarricade(EntityManager entityManager)
        {
            var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<BarricadeComponent>(),
                ComponentType.ReadOnly<HpComponent>()
            );
            
            using var entities = query.ToEntityArray(Allocator.Temp);
            Debug.Assert(entities.Length == 1, "There should exactly one barricade");
            return entities[0];
        }
        
        public static Entity GetRandomPlayerUnit(EntityManager entityManager, bool excludeWeapon)
        {
            // var query = entityManager.CreateEntityQuery(
            //     ComponentType.ReadOnly<TeamComponent>(),
            //     ComponentType.ReadOnly<UnitComponent>(),
            //     ComponentType.ReadOnly<AttackComponent>()
            // );
            var requiredComponents = new List<ComponentType>
            {
                ComponentType.ReadOnly<TeamComponent>(),
                ComponentType.ReadOnly<UnitComponent>(),
                ComponentType.ReadOnly<AttackComponent>()
            };

            var queryDesc = new EntityQueryDesc
            {
                All = requiredComponents.ToArray(),
                None = excludeWeapon 
                    ? new[] { ComponentType.ReadOnly<WeaponComponent>() }
                    : Array.Empty<ComponentType>()
            };

            var query = entityManager.CreateEntityQuery(queryDesc);
          
            // Create a list to store player entities
            var playerEntities = new NativeList<Entity>(Allocator.Temp);

            using var entities = query.ToEntityArray(Allocator.Temp);
            using var teamComponents = query.ToComponentDataArray<TeamComponent>(Allocator.Temp);

            for (int i = 0; i < entities.Length; i++)
            {
                if (teamComponents[i].Team == Team.Player)
                {
                    playerEntities.Add(entities[i]);
                }
            }

            // If no player entities exist, return Entity.Null
            if (playerEntities.Length == 0)
            {
                playerEntities.Dispose();
                return Entity.Null;
            }

            // Select a random player entity
            var randomIndex = UnityEngine.Random.Range(0, playerEntities.Length);
            var selectedEntity = playerEntities[randomIndex];

            playerEntities.Dispose();
            return selectedEntity;
        }
    }
}