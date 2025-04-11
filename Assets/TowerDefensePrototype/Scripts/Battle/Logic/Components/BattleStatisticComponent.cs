using Unity.Entities;

namespace CastlePrototype.Battle.Logic.Components
{
    public struct BattleStatisticComponent : IComponentData
    {
        public int EnemiesKilled;
        public int TotalEnemies;
    }
}