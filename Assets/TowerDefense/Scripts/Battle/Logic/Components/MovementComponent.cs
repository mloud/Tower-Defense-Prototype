using Unity.Entities;

namespace TowerDefense.Battle.Logic.Components
{
    public struct MovementComponent : IComponentData 
    {
        public float Speed;
        public float MaxSpeed;
    }
}