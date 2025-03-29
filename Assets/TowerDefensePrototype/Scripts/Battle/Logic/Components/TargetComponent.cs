using Unity.Entities;

namespace CastlePrototype.Battle.Logic.Components
{
    public struct TargetComponent : IComponentData
    {
        public Entity Target;
    }
}