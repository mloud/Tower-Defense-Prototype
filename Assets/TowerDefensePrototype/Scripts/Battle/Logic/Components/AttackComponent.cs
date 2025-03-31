using CastlePrototype.Data;
using Unity.Collections;
using Unity.Entities;

namespace CastlePrototype.Battle.Logic.Components
{
    public struct AttackComponent : IComponentData 
    {
        // IDS
        public FixedString64Bytes ProjectileVisualId;
        
        // BASE STATS
        public AttackType AttackType;
        public float TargetRange;
        public float AttackDistance;
        public float Cooldown;
        public float AttackDamage;
        public float AoeRadius;
        public float AoeDamage;
        public float ProjectileSpeed;
        
        // specific
        public int Penetration;
        public int Fireagain;
        public int Bounce;
        public bool Knockback;
        
        // runtime data
        public double LastAttackTime;
        public double NextMainAttackTime;

        public FixedList512Bytes<double> SecondaryAttackTimes;
        public bool IsInAttackDistance;
        // for visual
        public bool PlayAttack;
    }
}