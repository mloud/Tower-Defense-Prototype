using Unity.Entities;
using Unity.Mathematics;

namespace CastlePrototype.Battle.Logic.Components
{
    public struct ProjectileComponent : IComponentData
    {
        // settings
        public float Speed;
        public float Damage;
        public float AoeRadius;
        public Team AttackerTeam;
        
        
        // runtime data
        public Entity Target;
        public float3 TargetPosition;
    }
}