using CastlePrototype.Battle.Logic.Components;
using CastlePrototype.Battle.Logic.Managers;
using Unity.Entities;

namespace CastlePrototype.Battle.Logic.Systems
{
    public partial struct EventSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
            var eventsManager = WorldManagers.Get<BattleEventsManager>(state.World);

            foreach (var battleProgressionC in SystemAPI.Query<RefRW<BattleProgressionComponent>>())
            {
                if (battleProgressionC.ValueRO.BattlePointsUpdated)
                {
                    eventsManager.UpdateBattlePoints(battleProgressionC.ValueRO);
                    battleProgressionC.ValueRW.BattlePointsUpdated = false;
                }

                if (battleProgressionC.ValueRO.BattleTimeUpdated)
                {
                    eventsManager.UpdateBattleTime(battleProgressionC.ValueRO);
                    battleProgressionC.ValueRW.BattleTimeUpdated = false;
                }
            }
            
            foreach (var enemySpawnerC in SystemAPI.Query<RefRW<EnemySpawnerComponent>>())
            {
                if (enemySpawnerC.ValueRO.currentWaveChanged)
                {
                    eventsManager.UpdateWaveCounter(enemySpawnerC.ValueRO);
                    enemySpawnerC.ValueRW.currentWaveChanged = false;
                }
            }
            
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}