using CastlePrototype.Battle.Logic.Components;
using CastlePrototype.Data;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;

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

            
            if (spawner.elapsedTime > spawner.waves[spawner.currentWave].time)
            {
                if (spawner.spawnedThisWave < spawner.waves[spawner.currentWave].enemiesCount)
                {
                    if (spawner.elapsedTime - spawner.lastSpawnTime >= spawner.spawnInterval)
                    {
                        spawner.lastSpawnTime = spawner.elapsedTime;
                        SpawnEnemy(ref state, ref ecb,
                            new EnemySpawnerData
                        {
                            enemyId = spawner.waves[spawner.currentWave].enemyId,
                            spawnPosition = spawner.spawnPosition,
                            spawnBox = spawner.spawnBox
                        });
                        spawner.spawnedThisWave++;
                    }
                }
                else if (spawner.currentWave < spawner.waves.Length - 1 && spawner.elapsedTime > spawner.waves[spawner.currentWave + 1].time)
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
            var entity = ecb.CreateEntity();
            var localTransform = LocalTransform.FromPositionRotation(
                    Utils.GetRandomPosition(spawnData.spawnPosition, spawnData.spawnBox),
                    quaternion.identity);
            
            ecb.AddComponent(entity, localTransform);
            ecb.AddComponent(entity, new MovementComponent { Speed = 0.4f, MaxSpeed = 0.4f});
         
            // Create an EntityQuery for WallComponent
            ecb.AddComponent(entity, new AttackComponent
            {
                AttackDamage = 1,
                AttackDistance = 1,
                Cooldown = 2,
                TargetRange = 20,
                AttackType = AttackType.Melee
            });
            ecb.AddComponent(entity, new SettingComponent { DistanceAxes = new float3(1,0,1) });
            ecb.AddComponent(entity, new HpComponent { Hp = 2, MaxHp = 2});
            ecb.AddComponent(entity, new TeamComponent {Team = Team.Enemy});
            ecb.AddComponent(entity, new VisualComponent {VisualId = spawnData.enemyId});
        }
    }
    public struct EnemySpawnerData
    {
        public float3 spawnPosition;
        public float3 spawnBox;
        public FixedString64Bytes enemyId;
    }
}