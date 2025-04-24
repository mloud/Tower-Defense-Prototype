using OneDay.Core.Modules.Data;

namespace TowerDefense.Data.Progress
{
    public interface IPlayerProgress
    {
        public int Xp { get; set; }
        public int Level { get; set; }
        public int UnlockedStage { get; set; }
    }
    
    public class PlayerProgress : BaseDataObject, IPlayerProgress
    {
        public int Xp { get; set; }
        public int Level { get; set; }
        public int UnlockedStage { get; set; }
        public int LastFinishedStage { get; set; } = -1;
    }
}