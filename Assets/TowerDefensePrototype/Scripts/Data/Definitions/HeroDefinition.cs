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
        public float AttackDelay;

        [Header("Unit type stats")] 
        public AttackType AttackType;
        public bool CreatedBySkill;
      
        // BASIC STATS
        [Header("Basic stats")]
        public float ProjectileSpeed;
        public float MoveSpeed;

        [SerializeField] float Cooldown;
        [SerializeField] float Damage;
        [SerializeField] float AttackDistance;
        [SerializeField] float TargetRange;
        [SerializeField] float Hp;
     
        // SPECIFIC
        [Header("Specific stats")]
        public bool Knockback;
        public bool KnockbackResistent;
        public float FireAgainInterval = 0.5f;

        [SerializeField] int Bounce;
        [SerializeField] int Penetration;
        [SerializeField] int Fireagain;
        [SerializeField] int FireAgainSpread;

       
        // AOE - either projectile or direct AOE      
        [Header("AOE stats")]
        public bool AoeOnly;
        [SerializeField] float AoeRadius;
        [SerializeField] float AoeDamage;
      
        [Header("Level upgrades")] 
        public HeroLevelUpgradePath UpgradePath;

        public float GetLeveledHeroStat(StatUpgradeType upgradeType, int level)
        {
            if (level == 1)
                return GetBaseHeroStat(upgradeType);
            return GetBaseHeroStat(upgradeType) + GetUpgradedStat(upgradeType, level);
        }

        public int GetCardsNeededToLevelUp(int currentLevel)
        {
            if (UpgradePath.StatsUpgrades.Count >= currentLevel)
                return UpgradePath.StatsUpgrades[currentLevel - 1].CardsRequired;

            return -1;
        }

        public bool IsMaxLevel(int currentLevel) => currentLevel > UpgradePath.StatsUpgrades.Count;
        
        private float GetUpgradedStat(StatUpgradeType upgradeType, int level)
        {
            if (level < 2)
                throw new ArgumentException("Level is expected to start at 2");

            int upgradeIndex = level - 2;
            if (upgradeIndex >= UpgradePath.StatsUpgrades.Count)
                throw new ArgumentException($"Level exceeded number of upgrade levels {level}");
            float upgradedValueSum = 0;
            for (int i = 0; i <= upgradeIndex; i++)
            {
                if (UpgradePath.StatsUpgrades[i].StatUpgradeType == upgradeType)
                {
                    upgradedValueSum += UpgradePath.StatsUpgrades[i].Value;
                }
            }

            return upgradedValueSum;
        }
        
        private float GetBaseHeroStat(StatUpgradeType upgradeType)
        {
            return upgradeType switch
            {
                StatUpgradeType.Bounce => Bounce,
                StatUpgradeType.Cooldown => Cooldown,
                StatUpgradeType.Damage => Damage,
                StatUpgradeType.Hp => Hp,
                StatUpgradeType.FireAgain => Fireagain,
                StatUpgradeType.AoeRadius => AoeRadius,
                StatUpgradeType.Penetration => Penetration,
                StatUpgradeType.AoeDamage => AoeDamage,
                StatUpgradeType.TargetRange => TargetRange,
                StatUpgradeType.AttackDistance => AttackDistance,
                StatUpgradeType.FireAgainSpread => FireAgainSpread,

                _ => throw new ArgumentOutOfRangeException(nameof(upgradeType), upgradeType, null)
            };
        }
    }
}