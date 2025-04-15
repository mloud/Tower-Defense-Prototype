using Unity.Entities;

namespace CastlePrototype.Battle.Logic.Components
{
    public struct TargetComponent : IComponentData
    {
      //  public TargetingType TargetingType;
        public Entity Target;

        // this is flag for player weapon that could be controlled by player
        public bool ManualTargetingActive;
    }
}