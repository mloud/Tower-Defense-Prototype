using Unity.Entities;

namespace TowerDefense.Battle.Logic.Components
{
    public struct DestroyComponent : IComponentData
    {
        public float DestroyIn;
    }
}