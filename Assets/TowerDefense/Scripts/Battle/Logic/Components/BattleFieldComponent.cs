using Unity.Entities;
using Unity.Mathematics;

namespace TowerDefense.Battle.Logic.Components
{
    public struct BattleFieldComponent : IComponentData
    {
        public float2 MinCorner;
        public float2 MaxCorner;
    }
}