using System;

namespace TowerDefense.Data.Definitions
{
    public partial class HeroDefinition
    {
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
        
        public int GetCoinsNeededToLevelUp(int currentLevel)
        {
            if (UpgradePath.StatsUpgrades.Count >= currentLevel)
                return UpgradePath.StatsUpgrades[currentLevel - 1].CoinsRequired;

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