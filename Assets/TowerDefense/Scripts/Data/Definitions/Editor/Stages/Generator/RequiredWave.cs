using System;

namespace TowerDefensePrototype.Data.Definitions.Stages.Generator.Editor
{
    [Serializable]
    public class RequiredWave
    {
        public int WaveIndex;
        public int EnemyCount;
        public int SpawnInterval;
        public string EnemyId;
    }
}