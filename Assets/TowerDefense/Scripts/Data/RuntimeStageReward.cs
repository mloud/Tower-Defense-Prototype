using System.Collections.Generic;

namespace TowerDefense.Data
{
    public class RuntimeStageReward
    {
        public Dictionary<string, int> Cards { get; } = new();
        public int Coins { get; private set; }

        public void AddCard(string unitId, int count)
        {
            if (!Cards.TryAdd(unitId, count))
            {
                Cards[unitId] += count;
            }
        }

        public void AddCoins(int coins)
        {
            Coins += coins;
        }
    }
}