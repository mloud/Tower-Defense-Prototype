using TowerDefense.Battle.Logic.Managers;
using TowerDefense.Data;
using Cysharp.Threading.Tasks;
using OneDay.Core;
using OneDay.Core.Modules.Ui;
using TowerDefense.Ui.Popups;
using Unity.Entities;

namespace TowerDefensePrototype.Scripts.Battle.Logic.Managers.Ui
{
    public class UiHelperManager : WorldManager
    {
        public UiHelperManager(World world) : base(world)
        { }

        protected override async UniTask OnInitialize()
        { }

        public async UniTask OpenDefeatPopup(RuntimeStageReward runtimeStageReward)
        {
            await UniTask.WaitForSeconds(2.0f);
            ServiceLocator.Get<IUiManager>().OpenPopup<DefeatPopup>(UiParameter.Create(runtimeStageReward));
        }
        
        public async UniTask OpenVictoryPopup(RuntimeStageReward runtimeStageReward)
        {
            await UniTask.WaitForSeconds(2.0f);
            ServiceLocator.Get<IUiManager>().OpenPopup<VictoryPopup>(UiParameter.Create(runtimeStageReward));
        }
     
        protected override void OnRelease()
        { }
    }
}