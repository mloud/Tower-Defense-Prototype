using Unity.Collections;
using Unity.Entities;

namespace CastlePrototype.Battle.Logic.Components
{
    public struct TargetedComponent : IComponentData
    {
        public FixedList512Bytes<Entity> Targeters;
    }
}