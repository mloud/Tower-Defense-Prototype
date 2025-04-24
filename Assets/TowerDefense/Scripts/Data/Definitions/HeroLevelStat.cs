using UnityEngine;

namespace TowerDefense.Data.Definitions
{
    [System.Serializable]
    public class HeroLevel
    {
        [Header("Basic stats")]
        public float AttackDelay;
        public float Cooldown;
        public float Damage;
        public float AttackDistance;
        public float TargetRange;
        public float Hp;
     
        // SPECIFIC
        [Header("Specific stats")]
        public int Bounce;
        public int Penetration;
        public int Fireagain;
     
        // AOE - either projectile or direct AOE      
        [Header("AOE stats")]
        public float AoeRadius;
        public float AoeDamage;
    }
}