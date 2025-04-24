using Cysharp.Threading.Tasks;
using OneDay.Core;
using OneDay.Core.Modules.Data;
using TowerDefense.Managers.Vallet;
using TowerDefense.Scripts.Managers;
using UnityEngine;

namespace TowerDefense.Managers
{
    public interface IPlayerManager
    {
        IValetGetter ValetGetter { get; }
        IStageGetter StageGetter { get; }
        IProgressionGetter ProgressionGetter { get; }
        IDeckGetter DeckGetter { get; }

        UniTask InitializePlayer();
    }

    public partial class PlayerManager : MonoBehaviour, IPlayerManager, IService
    {
        public IValetGetter ValetGetter { get; private set; }
        public IStageGetter StageGetter { get; private set; }
        public IProgressionGetter ProgressionGetter { get; private set; }
        public IDeckGetter DeckGetter { get; private set; }
        private IValetPlugin ValetPlugin { get; set; }
        private IStagePlugin StagePlugin { get; set; }
        private IProgressionPlugin ProgressionPlugin { get; set; }
        private IDeckPlugin DeckPlugin { get; set; }
        private IDataManager DataManager { get; set; }

        public UniTask Initialize()
        {
            DataManager = ServiceLocator.Get<IDataManager>();
            
            ValetGetter = ValetPlugin = new ValetGetter(DataManager);
            ProgressionGetter = ProgressionPlugin = new ProgressionPlugin(DataManager);
            DeckGetter = DeckPlugin = new DeckPlugin(DataManager);
            StageGetter = StagePlugin = new StagePlugin(DataManager, ValetPlugin, DeckPlugin, ProgressionPlugin);
            
            return UniTask.CompletedTask;
        }

        public UniTask PostInitialize() => UniTask.CompletedTask;
    }
}