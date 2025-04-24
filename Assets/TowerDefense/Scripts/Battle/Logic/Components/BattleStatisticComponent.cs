using Unity.Entities;

namespace TowerDefense.Battle.Logic.Components
{
    public struct BattleStatisticComponent : IComponentData
    {
        public int EnemiesKilled;
        public int TotalEnemies;
    }
}