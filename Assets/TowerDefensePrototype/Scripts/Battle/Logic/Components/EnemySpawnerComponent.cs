    using Unity.Collections;
    using Unity.Entities;
    using Unity.Mathematics;

    namespace CastlePrototype.Battle.Logic.Components
    {
        public struct Wave
        {
            public float Time;
            public int EnemiesCount;
            public FixedString64Bytes EnemyId;
            public float SpawnInterval;

            public Wave(float time, int enemiesCount, FixedString64Bytes enemyId, float spawnInterval)
            {
                Time = time;
                EnemiesCount = enemiesCount;
                EnemyId = enemyId;
                SpawnInterval = spawnInterval;
            }
    }
        
        public struct EnemySpawnerComponent : IComponentData
        {
            // definition
            public float3 spawnPosition; // Base spawn position
            public float3 spawnBox;
               
            // NEW
            public FixedList4096Bytes<Wave> waves;
            
            
            // runtime data
            public int currentWave;
            public int spawnedThisWave;
            public double lastSpawnTime;
            public bool currentWaveChanged;
            public double elapsedTime;
        }
    }