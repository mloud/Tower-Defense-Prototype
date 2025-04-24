using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace TowerDefense.Battle.Logic.Components
{
    public struct TriggerVisualEffectComponent : IComponentData
    {
        public FixedString64Bytes EffectId;
        public float3 Position;
    }
}