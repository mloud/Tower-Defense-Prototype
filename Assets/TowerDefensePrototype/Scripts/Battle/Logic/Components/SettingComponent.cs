using Unity.Entities;
using Unity.Mathematics;

namespace CastlePrototype.Battle.Logic.Components
{
    public struct SettingComponent : IComponentData
    {
        public float3 DistanceAxes;
    }
}