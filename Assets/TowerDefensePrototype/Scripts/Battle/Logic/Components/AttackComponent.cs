using Unity.Collections;
using Unity.Entities;

namespace CastlePrototype.Battle.Logic.Components
{

    public enum AttackType
    {
        Melee,
        Range,
        Aoe,
        RangeAoe
    }
    public struct AttackComponent : IComponentData 
    {
        // readonly data
        public float TargetRange;
        public float AttackDistance;
        public float AttackInterval;
        public float AttackDamage;
        // for direct AOE and RangeAoe
        public float AoeRadius;
        public FixedString64Bytes ProjectileVisualId;
        public float ProjectileSpeed;
        public AttackType AttackType;
        
        // runtime data
        public double LastAttackTime;
        public bool IsInAttackDistance;
        
        // for visual
        public bool PlayAttack;
    }
}