using Unity.Entities;
using Unity.Mathematics;

namespace TowerDefense.Battle.Logic.Components
{
    public struct ManualTargetingComponent : IComponentData
    {
        public float2 Direction;
    }
}