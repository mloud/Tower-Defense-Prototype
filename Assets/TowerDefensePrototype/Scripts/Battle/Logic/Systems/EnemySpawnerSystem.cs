using CastlePrototype.Battle.Logic.Components;
using CastlePrototype.Battle.Logic.Managers;
using CastlePrototype.Battle.Visuals;
using TowerDefensePrototype.Battle.Visuals.Effects;
using TowerDefensePrototype.Scripts.Battle.Logic.Managers.Units;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;

namespace CastlePrototype.Battle.Logic.Systems
{
    [DisableAutoCreation]
    public partial struct EnemySpawnerSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EnemySpawnerComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            if (!SystemAPI.HasSingleton<EnemySpawnerComponent>())
                return;
            var deltaTime = SystemAPI.Time.DeltaTime;
            var spawner = SystemAPI.GetSingleton<EnemySpawnerComponent>();
        
            // Stop if max waves reached
            if (spawner.currentWave >= spawner.waves.Length)
                return; 

            var ecb = new EntityCommandBuffer(Allocator.Temp);
            spawner.elapsedTime += deltaTime;

            
            if (spawner.elapsedTime > spawner.waves[spawner.currentWave].Time)
            {
                if (spawner.spawnedThisWave < spawner.waves[spawner.currentWave].EnemiesCount)
                {
                    float nextSpawnIntervalInWave = spawner.waves[spawner.currentWave].SpawnInterval;
                    
                    
                    // spawn next enemy
                    if (spawner.elapsedTime - spawner.lastSpawnTime >= nextSpawnIntervalInWave)
                    {
                        spawner.lastSpawnTime = spawner.elapsedTime;
                        SpawnEnemy(ref state, ref ecb,
                            new EnemySpawnerData
                        {
                            enemyId = spawner.waves[spawner.currentWave].EnemyId,
                            spawnPosition = spawner.spawnPosition,
                            spawnBox = spawner.spawnBox
                        });
                        spawner.spawnedThisWave++;
                    }
                }
                else if (spawner.currentWave < spawner.waves.Length - 1 && spawner.elapsedTime > spawner.waves[spawner.currentWave + 1].Time)
                {
                    spawner.currentWave++;
                    spawner.currentWaveChanged = true;
                    spawner.spawnedThisWave = 0;
                }
            }
           
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
            SystemAPI.SetSingleton(spawner);
        }

        private void SpawnEnemy(ref SystemState state, ref EntityCommandBuffer ecb, EnemySpawnerData spawnData)
        {
            var position = Utils.GetRandomPosition(spawnData.spawnPosition, spawnData.spawnBox);
            WorldManagers.Get<UnitManager>(state.World).CreateEnemyUnit(ref ecb, position, spawnData.enemyId.ToString());
            VisualManager.Default.PlayEffect(EffectKeys.SpawnEffectEnemy, position);
        }
    }
    public struct EnemySpawnerData
    {
        public float3 spawnPosition;
        public float3 spawnBox;
        public FixedString64Bytes enemyId;
    }
}