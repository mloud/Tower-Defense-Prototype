using System;
using CastlePrototype.Battle.Logic.Components;
using CastlePrototype.Battle.Logic.Managers;
using CastlePrototype.Battle.Visuals;
using TowerDefensePrototype.Battle.Visuals.Effects;
using TowerDefensePrototype.Scripts.Battle.Logic.Managers.Units;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;


namespace CastlePrototype.Battle.Logic.Systems
{
    [DisableAutoCreation]
    public partial struct EnemySpawnerSystem : ISystem
    {
        private const int SpawnSquaresCount = 20;

        private const int MinEnemiesInMicroWave = 2;
        private const int MaxEnemiesInMicroWave = 5;

        // keep track where enemies were spawned to avoid their overlapping
        private FixedList128Bytes<bool> spawnSquares;
        
        public void OnCreate(ref SystemState state)
        {
            spawnSquares = new FixedList128Bytes<bool>();
            for (int i = 0; i < SpawnSquaresCount; i++)
                spawnSquares.Add(false);
            
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

                    int remainingEnemies = spawner.waves[spawner.currentWave].EnemiesCount - spawner.spawnedThisWave;
                    int spawnedEnemiesInTheSpawn = math.min(remainingEnemies, UnityEngine.Random.Range(MinEnemiesInMicroWave, MinEnemiesInMicroWave+1));
                    
                    // spawn next enemy line
                    if (spawner.elapsedTime - spawner.lastSpawnTime >= nextSpawnIntervalInWave)
                    {
                        spawner.lastSpawnTime = spawner.elapsedTime;
                        // mark all spawn squares as free
                        for (int i = 0; i < SpawnSquaresCount; i++)
                            spawnSquares[i] = false;
                        
                        
                        Debug.Assert(spawnedEnemiesInTheSpawn < SpawnSquaresCount);
                        
                        for (int i = 0; i < spawnedEnemiesInTheSpawn; i++)
                        {
                            float3 spawnPosition = float3.zero;
                            spawnPosition.z =spawner.spawnPosition.z + UnityEngine.Random.Range(-spawner.spawnBox.z / 2, +spawner.spawnBox.z / 2);
                            float spawnSquareSize = spawner.spawnBox.x / SpawnSquaresCount;
                            int randomSquareIndex = UnityEngine.Random.Range(0, SpawnSquaresCount);
                            for (int j = randomSquareIndex; j < randomSquareIndex + SpawnSquaresCount; j++)
                            {
                                int rndSpawnSquareIndex = j % SpawnSquaresCount;
                                if (spawnSquares[rndSpawnSquareIndex] == false)
                                {
                                    spawnPosition.x = spawner.spawnPosition.x - spawner.spawnBox.x / 2 +
                                                      rndSpawnSquareIndex * spawnSquareSize + spawnSquareSize / 2;
                                    // mark occupied
                                    spawnSquares[j % SpawnSquaresCount] = true;
                                    break;
                                }
                            }
                            WorldManagers.Get<UnitManager>(state.World).CreateEnemyUnit(ref ecb, spawnPosition, spawner.waves[spawner.currentWave].EnemyId.ToString());
                            VisualManager.Default.PlayEffect(EffectKeys.SpawnEffectEnemy, spawnPosition);
                            //
                            // SpawnEnemy(ref state, ref ecb,
                            //     new EnemySpawnerData
                            //     {
                            //         enemyId = spawner.waves[spawner.currentWave].EnemyId,
                            //         spawnPosition = spawner.spawnPosition,
                            //         spawnBox = spawner.spawnBox
                            //     });
                            spawner.spawnedThisWave++;
                        }
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