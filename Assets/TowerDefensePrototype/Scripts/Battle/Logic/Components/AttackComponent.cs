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
        public bool AoeOnly;
        
        // specific
        public int Penetration;
        public int FireAgain;
        public int Bounce;
        public bool KnockBack;
        public float FireAgainInterval;
        
        // runtime data
        public double LastAttackTime;
        public double NextMainAttackTime;

        public FixedList512Bytes<double> SecondaryAttackTimes;
        public bool IsInAttackDistance;
        // for visual
        public bool PlayAttack;
    }
}