using CastlePrototype.Battle.Logic.Components;
using Unity.Collections;
using Unity.Entities;

namespace CastlePrototype.Battle.Logic.EcsUtils
{
    public static class QueryUtils
    {
        public static Entity GetRandomPlayerUnit(EntityManager entityManager)
        {
            var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<TeamComponent>(),
                ComponentType.ReadOnly<UnitComponent>()
            );

            // Create a list to store player entities
            var playerEntities = new NativeList<Entity>(Allocator.Temp);

            using var entities = query.ToEntityArray(Allocator.Temp);
            using var teamComponents = query.ToComponentDataArray<TeamComponent>(Unity.Collections.Allocator.Temp);

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