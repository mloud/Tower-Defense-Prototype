using TowerDefense.Data;
using Unity.Collections;
using Unity.Entities;

namespace TowerDefense.Battle.Logic.Components
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
        public float AttackAnimDelay;
        public bool AoeOnly;
        // time before next attack anim ca be played
      
        
        // specific
        public int Penetration;
        public int FireAgain;
        public int Bounce;
        public bool KnockBack;
        public float FireAgainInterval;
        public int FireAgainSpread;
        
        // runtime data
        public double NextMainAttackTime;

        public FixedList512Bytes<float> SecondaryAttackTimes;
        public FixedList512Bytes<float> SecondaryAttackAngles;

        public bool IsInAttackDistance;
        // for visual
        public bool PlayAttack;
        public bool PlayAttackBlockedToNextAttack;
    }
}