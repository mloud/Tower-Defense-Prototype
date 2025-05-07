using TowerDefense.Managers;

namespace TowerDefense.Managers
{
    public class NewLevelBufferedEvent : BufferedEvent
    {
        public string HeroId { get; }
        public int Level { get; }
        public NewLevelBufferedEvent(int level, string heroId)
        {
            Type = (int)BufferedEventsIds.NewLevel;
            Level = level;
            HeroId = heroId;
        }
    }
}