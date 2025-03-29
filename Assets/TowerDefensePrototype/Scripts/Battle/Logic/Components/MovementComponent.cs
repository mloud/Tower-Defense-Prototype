using Unity.Entities;

namespace CastlePrototype.Battle.Logic.Components
{
    public struct MovementComponent : IComponentData 
    {
        public float Speed;
        public float MaxSpeed;
    }
}