using Unity.Entities;

namespace CastlePrototype.Battle.Logic.Components
{
    public struct LookAtTargetComponent : IComponentData
    {
        public bool LookAtTarget;
    }
}