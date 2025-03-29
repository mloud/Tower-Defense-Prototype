using CastlePrototype.Battle.Logic.Components;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;


namespace CastlePrototype.Battle.Logic.Systems
{
    [DisableAutoCreation]
    public partial struct EnemySpawnerSystem : ISystem 
    {
        private NativeList<EnemySpawnerData> entitiesToCreate;
        
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EnemySpawnerComponent>();
            entitiesToCreate = new NativeList<EnemySpawnerData>(Allocator.Persistent);
        }

        public void OnDestroy(ref SystemState state)
        {
            if (entitiesToCreate.IsCreated)
                entitiesToCreate.Dispose();
        }
        
        public void OnUpdate(ref SystemState state)
        {
            var currentTime = SystemAPI.Time.ElapsedTime;

            if (!SystemAPI.HasSingleton<EnemySpawnerComponent>())
                return;
            
            var spawner = SystemAPI.GetSingleton<EnemySpawnerComponent>();
            
            // Stop if max waves reached
            if (spawner.currentWave >= spawner.totalWaves)
                return; 

            if (spawner.spawnedThisWave < spawner.enemiesPerWave)
            {
                if (currentTime - spawner.lastSpawnTime >= spawner.spawnInterval)
                {
                    spawner.lastSpawnTime = currentTime;
                    entitiesToCreate.Add(new EnemySpawnerData
                    {
                        enemyId = spawner.enemyId,
                        spawnPosition = spawner.spawnPosition,
                        spawnBox = spawner.spawnBox
                    });
                    spawner.spawnedThisWave++;
                }
            }
            else if (currentTime - spawner.lastWaveTime >= spawner.waveInterval)
            {
                spawner.currentWave++;
                spawner.currentWaveChanged = true;
                spawner.spawnedThisWave = 0;
                spawner.lastWaveTime = currentTime;
            }
 
            SystemAPI.SetSingleton(spawner);

            for (int i = 0; i < entitiesToCreate.Length; i++)
            {
                SpawnEnemy(ref state, ref entitiesToCreate.ElementAt(i));
            }

            entitiesToCreate.Clear();
        }

        private void SpawnEnemy(ref SystemState state, ref EnemySpawnerData spawnData)
        {
            var entity = state.EntityManager.CreateEntity();
            var localTransform = LocalTransform.FromPositionRotation(
                    Utils.GetRandomPosition(spawnData.spawnPosition, spawnData.spawnBox),
                    quaternion.identity);
            
            state.EntityManager.AddComponentData(entity, localTransform);
            state.EntityManager.AddComponentData(entity, new MovementComponent { Speed = 0.4f, MaxSpeed = 0.4f});
         
            // Create an EntityQuery for WallComponent
            state.EntityManager.AddComponentData(entity, new AttackComponent
            {
                AttackDamage = 1,
                AttackDistance = 1,
                AttackInterval = 2,
                TargetRange = 20,
                AttackType = AttackType.Melee
            });
            state.EntityManager.AddComponentData(entity, new SettingComponent { DistanceAxes = new float3(1,0,1) });
            state.EntityManager.AddComponentData(entity, new HpComponent { Hp = 2, MaxHp = 2});
            state.EntityManager.AddComponentData(entity, new TeamComponent {Team = Team.Enemy});
            state.EntityManager.AddComponentData(entity, new VisualComponent {VisualId = spawnData.enemyId});
        }
    }
    public struct EnemySpawnerData
    {
        public float3 spawnPosition;
        public float3 spawnBox;
        public FixedString64Bytes enemyId;
    }
}