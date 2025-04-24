using System.Linq;
using Cysharp.Threading.Tasks;
using OneDay.Core.Modules.Data;
using TowerDefense.Data.Definitions;
using TowerDefense.Data.Progress;

namespace TowerDefense.Scripts.Managers
{
    public delegate void XpChangedDelegate(int newXp, int nextXpNeeded, int prevLevel, int currentLevel);
    public interface IProgressionGetter
    {
        XpChangedDelegate XpChanged { get; set; }
        
        UniTask<PlayerProgress> GetProgression();
        UniTask<PlayerProgressionDefinition> GetPlayerProgressionDefinition();
        UniTask<(int xp, int xpNextLevel, int level)> GetProgressionInfo();
    }
    
    public interface IProgressionSetter
    {
        UniTask SaveProgression(PlayerProgress progress);
    }

    public interface IProgressionPlugin : IProgressionGetter, IProgressionSetter
    { }
    
    public class ProgressionPlugin : IPlugin, IProgressionPlugin
    {
        public XpChangedDelegate XpChanged { get; set;  }
        
        private IDataManager dataManager;
     
        public ProgressionPlugin(IDataManager dataManager)
        {
            this.dataManager = dataManager;
        }
        public async UniTask<PlayerProgress> GetProgression()
            => (await dataManager.GetAll<PlayerProgress>()).FirstOrDefault();
        
        public async UniTask<(int xp, int xpNextLevel, int level)> GetProgressionInfo()
        {
            var progression = await GetProgression();
            var playerProgressionDef = await GetPlayerProgressionDefinition();
            return (progression.Xp, playerProgressionDef.XpNeededToNextLevel[progression.Level], progression.Level);
        }
    
        public async UniTask<PlayerProgressionDefinition> GetPlayerProgressionDefinition() =>
            (await dataManager.GetAll<PlayerProgressionDefinition>()).FirstOrDefault();
        
        public async UniTask SaveProgression(PlayerProgress progress) => 
            await dataManager.Actualize<PlayerProgress>(progress);
    }
}