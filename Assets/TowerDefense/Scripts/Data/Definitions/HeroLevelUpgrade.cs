using System;
using System.Collections.Generic;

namespace TowerDefense.Data.Definitions
{
    public enum StatUpgradeType
    {
        Cooldown,
        Damage,
        Bounce,
        FireAgain,
        Penetration,
        AoeRadius,
        AoeDamage,
        Hp,
        TargetRange,
        AttackDistance,
        FireAgainSpread
    }

    [Serializable]
    public class StatUpgrade
    {
        public int CardsRequired;
        public int CoinsRequired;
        public StatUpgradeType StatUpgradeType;
        public float Value;
    }
  
    [Serializable]
    public class HeroLevelUpgradePath
    {
        public List<StatUpgrade> StatsUpgrades;
    }
}