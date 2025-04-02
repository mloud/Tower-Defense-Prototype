using System;

namespace CastlePrototype.Data.Definitions
{
    [Serializable]
    public class WaveDefinition
    {
        public float Time;
        public int EnemiesCount;
        public string EnemyId;
        public float SpawnInterval;
    }
}