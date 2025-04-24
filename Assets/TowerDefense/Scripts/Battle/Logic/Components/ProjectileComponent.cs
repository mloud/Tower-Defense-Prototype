using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace TowerDefense.Battle.Logic.Components
{
    public struct ProjectileComponent : IComponentData
    {
        // settings
        public float Speed;
        public float Damage;
        public float AoeRadius;
        public float AoeDamage;
        public Team AttackerTeam;
        public bool KnockBack;
        public bool AoeOnly;
       
        // runtime data
        public Entity Target;
        public float3 Direction;
        public float3 TargetPosition;
        public int PenetrationCounter;
        
        // edge bounce data
        public float3 EdgeNormal;
        public int BounceCounter;

        public FixedList512Bytes<Entity> HitEntities;
    }
}