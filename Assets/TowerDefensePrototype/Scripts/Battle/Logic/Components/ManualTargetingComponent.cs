using Unity.Entities;
using Unity.Mathematics;

namespace CastlePrototype.Battle.Logic.Components
{
    public struct ManualTargetingComponent : IComponentData
    {
        public float2 Direction;
    }
}