using OneDay.Core.Modules.Data;

namespace CastlePrototype.Data
{
    public interface IPlayerProgress
    {
        public int Xp { get; }
        public int Level { get; }
    }
    
    public class PlayerProgress : BaseDataObject, IPlayerProgress
    {
        public int Xp { get; set; }
        public int Level { get; set; }
    }
}