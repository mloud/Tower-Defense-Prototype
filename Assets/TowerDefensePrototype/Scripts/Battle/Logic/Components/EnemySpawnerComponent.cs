    using Unity.Collections;
    using Unity.Entities;
    using Unity.Mathematics;

    namespace CastlePrototype.Battle.Logic.Components
    {
        public struct EnemySpawnerComponent : IComponentData
        {
            public float3 spawnPosition; // Base spawn position
            public float3 spawnBox;
            public float spawnInterval; // Time between individual spawns
            public float waveInterval; // Time between waves
            public int enemiesPerWave; // How many enemies per wave
            public int totalWaves; // Total number of waves
            public FixedString64Bytes enemyId;
            public int currentWave;
            public int spawnedThisWave;
            public double lastSpawnTime;
            public double lastWaveTime;
        
            
            public bool currentWaveChanged;
        }
    }