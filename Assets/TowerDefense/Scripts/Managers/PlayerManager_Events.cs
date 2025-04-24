using System;
using TowerDefense.Data.Definitions;
using TowerDefense.Data.Progress;

namespace TowerDefense.Managers
{
    public partial class PlayerManager
    {
        public Action<(int newXp, int nextXpNeeded, int prevLevel, int currentLevel)> OnXpChanged { get; set; }
        public Action<(HeroProgress progress, HeroDefinition definition)> OnHeroLeveledUp { get; set; }
    }
}
    