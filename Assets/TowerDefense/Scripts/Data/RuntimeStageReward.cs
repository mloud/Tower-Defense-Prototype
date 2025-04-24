using System.Collections.Generic;

namespace TowerDefense.Data
{
    public class RuntimeStageReward
    {
        public Dictionary<string, int> Cards { get; } = new();

        public void AddCard(string unitId, int count)
        {
            Cards.Add(unitId, count);
        }
    }
}