using System;
using System.Collections.Generic;

namespace CastlePrototype.Data.Definitions
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
        AttackDistance
    }

    [Serializable]
    public class StatUpgrade
    {
        public int CardsRequired;
        public StatUpgradeType StatUpgradeType;
        public float Value;
    }
  
    [Serializable]
    public class HeroLevelUpgradePath
    {
        public List<StatUpgrade> StatsUpgrades;
    }
}