    using Unity.Collections;
    using Unity.Entities;
    using Unity.Mathematics;

    namespace CastlePrototype.Battle.Logic.Components
    {
        public struct Wave
        {
            public float time;
            public int enemiesCount;
            public FixedString64Bytes enemyId;
            public float spawnInterval;

            public Wave(float time, int enemiesCount, FixedString64Bytes enemyId, float spawnInterval)
            {
                this.time = time;
                this.enemiesCount = enemiesCount;
                this.enemyId = enemyId;
                this.spawnInterval = spawnInterval;
            }
    }
        
        public struct EnemySpawnerComponent : IComponentData
        {
            // definition
            public float3 spawnPosition; // Base spawn position
            public float3 spawnBox;
            
            
            // OLD
            public float spawnInterval; // Time between individual spawns
            public int totalWaves; // Total number of waves
            public FixedString64Bytes enemyId;
            
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