using Unity.Collections;
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
        public bool KnockBack;
       
        // runtime data
        public Entity Target;
        public float3 TargetPosition;
        public int PenetrationCounter;

        public FixedList512Bytes<Entity> HitEntities;
    }
}