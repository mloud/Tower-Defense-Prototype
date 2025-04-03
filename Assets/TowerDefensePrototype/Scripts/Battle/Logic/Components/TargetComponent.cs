using CastlePrototype.Data;
using Unity.Entities;

namespace CastlePrototype.Battle.Logic.Components
{
    public struct TargetComponent : IComponentData
    {
      //  public TargetingType TargetingType;
        public Entity Target;
    }
}