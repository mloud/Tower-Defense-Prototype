using System;

namespace CastlePrototype.Managers
{
    public partial class PlayerManager
    {
        public Action<(int newXp, int nextXpNeeded, int prevLevel, int currentLevel)> OnXpChanged { get; set; }
    }
}
    