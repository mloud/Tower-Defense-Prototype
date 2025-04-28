using Core.Modules.Ui.Loading;
using Cysharp.Threading.Tasks;
using Meditation.States;
using OneDay.Core;
using OneDay.Core.Modules.Assets;
using OneDay.Core.Modules.Pooling;
using OneDay.Core.Modules.Sm;
using OneDay.Core.Modules.Ui;
using TowerDefense.Battle.Logic;
using TowerDefense.Battle.Visuals;
using TowerDefense.Battle.Visuals.Effects;
using TowerDefense.Managers;
using TowerDefense.Managers.Simulation;
using TowerDefense.Ui.Panels;
using TowerDefense.Ui.Views;
using UnityEngine;


namespace TowerDefense.States
{
    public class GameState : AState
    {
        private GameView view;
        private IAssetManager assetManager;
        private IPoolManager poolManager;
        
        private IVisualManager visualManager;
        private BattleController battleController;
        private BattlePooler battlePooler;
      
        
        public override UniTask Initialize()
        {
            poolManager = ServiceLocator.Get<IPoolManager>();
            
            view = ServiceLocator.Get<IUiManager>().GetView<GameView>();
            view.BindAction(view.BackButton, OnBackButton);
            battlePooler = new BattlePooler();   
            return UniTask.CompletedTask;
        }

        public override async UniTask EnterAsync(StateData stateData = null)
        {
            int stageIndex = stateData.GetValue<int>("stage");
            var stageDefinition = await ServiceLocator.Get<IPlayerManager>().StageGetter.GetStageDefinition(stageIndex);
            var heroDeck = await ServiceLocator.Get<IPlayerManager>().DeckGetter.GetHeroDeck();
            ILoading loading = null;

            ServiceLocator.Get<IUiManager>().GetPanel<MainButtonPanel>().Hide(true);
            ServiceLocator.Get<IUiManager>().GetPanel<PlayerProfilePanel>().Hide(true);

            loading = ServiceLocator.Get<ILoading>();
            loading.Show();


            if (ServiceLocator.Get<ISimulationMode>().IsSimulationTypeActive(SimulationType.WithoutVisuals))
            {
                visualManager = new FooVisualManager();
                VisualManager.Default = visualManager;
            }
            else
            {
                Screen.sleepTimeout = SleepTimeout.NeverSleep;
                await battlePooler.Pool(loading, stageDefinition, heroDeck);
                var effectFactory = new PoolingEffectFactory(poolManager);
                var visualFactory = new PoolingVisualFactory(poolManager);

                visualManager = new VisualManager(visualFactory, effectFactory, view.GameUiPanel, view);
                visualManager.LoadEnvironment(stageDefinition.StageVisualKey);
            }

            battleController = new BattleController();

            await battleController.InitializeBattle(stageIndex);
            loading?.Hide();

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