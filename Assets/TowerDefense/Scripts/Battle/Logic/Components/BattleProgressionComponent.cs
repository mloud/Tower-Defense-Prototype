using Unity.Entities;

namespace TowerDefense.Battle.Logic.Components
{
    public struct BattleProgressionComponent : IComponentData
    {
        public int BattlePoints;
        public int BattlePointsNeeded;
        public float Timer;
        public bool BattlePointsUpdated;
        public bool BattleTimeUpdated;
        public int Stage;
    }
}