using System;
using OneDay.Core.Modules.Data;
using UnityEngine;

namespace CastlePrototype.Data.Definitions
{
    [Serializable]
    public class HeroDefinition : BaseDataObject
    {
        // ID
        [Header("Ids")]
        public string UnitId;
        public string VisualId;
        public string ProjectileVisualId;

        [Header("Unit type stats")] 
        public AttackType AttackType;
        // BASIC STATS
        [Header("Basic stats")]
        public float ProjectileSpeed;
        public float Cooldown;
        public float Damage;
        public float AttackDistance;
        public float TargetRange;
        public float MoveSpeed;
        public float Hp;
     
        // SPECIFIC
        [Header("Specific stats")]
        public int Bounce;
        public int Penetration;
        public int Fireagain;
        public bool Knockback;
    
        // AOE - either projectile or direct AOE      
        [Header("AOE stats")]
        public float AoeRadius;
        public float AoeDamage;
        public bool AoeOnly;
    }
}