using System;
using CastlePrototype.Data.Definitions;
using CastlePrototype.Data.Progress;

namespace CastlePrototype.Managers
{
    public partial class PlayerManager
    {
        public Action<(int newXp, int nextXpNeeded, int prevLevel, int currentLevel)> OnXpChanged { get; set; }
        public Action<(HeroProgress progress, HeroDefinition definition)> OnHeroLeveledUp { get; set; }
    }
}
    