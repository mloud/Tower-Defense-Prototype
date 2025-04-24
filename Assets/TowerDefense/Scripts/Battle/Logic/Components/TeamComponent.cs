using Unity.Entities;

namespace TowerDefense.Battle.Logic.Components
{
    public enum Team
    {
        Player,
        Enemy
    }
    public struct TeamComponent : IComponentData
    {
        public Team Team;
    }
}