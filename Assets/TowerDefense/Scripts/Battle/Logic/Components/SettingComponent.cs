using Unity.Entities;
using Unity.Mathematics;

namespace TowerDefense.Battle.Logic.Components
{
    public struct SettingComponent : IComponentData
    {
        public float3 DistanceAxes;
        public float Radius;
        public float Width;
        public bool NeedsTouchToGetTargeted;
    }
}