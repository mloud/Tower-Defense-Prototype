using Unity.Entities;
using Unity.Mathematics;

namespace TowerDefense.Battle.Logic.Components
{
    public struct TargetComponent : IComponentData
    {
      //  public TargetingType TargetingType;
        public Entity Target;

        // this is flag for player weapon that could be controlled by player
        public bool ManualTargetingActive;
        public float3 TargetPosition;
    }
}