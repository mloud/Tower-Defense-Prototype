using CastlePrototype.Battle.Logic;
using CastlePrototype.Battle.Visuals;
using CastlePrototype.Battle.Visuals.Effects;
using CastlePrototype.Managers;
using CastlePrototype.Ui.Panels;
using CastlePrototype.Ui.Views;
using Core.Modules.Ui.Loading;
using Cysharp.Threading.Tasks;
using Meditation.States;
using OneDay.Core;
using OneDay.Core.Modules.Assets;
using OneDay.Core.Modules.Data;
using OneDay.Core.Modules.Pooling;
using OneDay.Core.Modules.Sm;
using OneDay.Core.Modules.Ui;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace CastlePrototype.States
{
    public class GameState : AState
    {
        private GameView view;
        private IDataManager dataManager;
        private IAssetManager assetManager;
        private IPoolManager poolManager;
        
        private VisualManager visualManager;
        private BattleController battleController;
        private BattlePooler battlePooler;
      
        
        public override UniTask Initialize()
        {
            dataManager = ServiceLocator.Get<IDataManager>();
            poolManager = ServiceLocator.Get<IPoolManager>();
            
            view = ServiceLocator.Get<IUiManager>().GetView<GameView>();
            view.BindAction(view.BackButton, OnBackButton);
            battlePooler = new BattlePooler();   
            return UniTask.CompletedTask;
        }

        public override async UniTask EnterAsync(StateData stateData = null)
        {
            int stageIndex = stateData.GetValue<int>("stage");
            
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            
            ServiceLocator.Get<IUiManager>().GetPanel<MainButtonPanel>().Hide(true);
            ServiceLocator.Get<IUiManager>().GetPanel<PlayerProfilePanel>().Hide(true);
            
            var loading = ServiceLocator.Get<ILoading>();
            loading.Show();
            await battlePooler.Pool(loading);
       
            var effectFactory = new PoolingEffectFactory(poolManager);
            var visualFactory = new PoolingVisualFactory(poolManager);
            
            visualManager = new VisualManager(visualFactory, effectFactory, view.GameUiPanel);
            battleController = new BattleController();
            visualManager.LoadEnvironment("environment_1");

            await battleController.InitializeBattle(stageIndex);
            loading.Hide();
            view.Show(true);
        }

        public override UniTask ExecuteAsync() => UniTask.CompletedTask;

        public override UniTask ExitAsync()
        {
            battleController.Dispose();
            visualManager.Dispose();
            battleController = null;
            visualManager = null;
            
            battlePooler.Clear();
            
            view.Hide(true);
            Screen.sleepTimeout = SleepTimeout.SystemSetting;

            return UniTask.CompletedTask;
        }

        private void OnBackButton()
        {
            StateMachine.SetStateAsync<MenuState>().Forget();
        }
       
    }
}