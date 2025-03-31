using Unity.Entities;
using Unity.Mathematics;

namespace CastlePrototype.Battle.Logic.Components
{
    public struct BattleFieldComponent : IComponentData
    {
        public float2 MinCorner;
        public float2 MaxCorner;
    }
}