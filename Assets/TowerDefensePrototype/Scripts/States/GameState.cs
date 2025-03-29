using CastlePrototype.Battle;
using CastlePrototype.Battle.Logic;
using CastlePrototype.Battle.Visuals;
using CastlePrototype.Managers;
using CastlePrototype.Ui.Views;
using Cysharp.Threading.Tasks;
using Meditation.States;
using OneDay.Core;
using OneDay.Core.Modules.Assets;
using OneDay.Core.Modules.Data;
using OneDay.Core.Modules.Sm;
using OneDay.Core.Modules.Ui;


namespace CastlePrototype.States
{
    public class GameState : AState
    {
        private GameView view;
        private IDataManager dataManager;
        private IAssetManager assetManager;
     
        private VisualManager visualManager;
        private BattleController battleController;
      
        
        public override UniTask Initialize()
        {
            dataManager = ServiceLocator.Get<IDataManager>();
            view = ServiceLocator.Get<IUiManager>().GetView<GameView>();
            view.BindAction(view.BackButton, OnBackButton);
            
            return UniTask.CompletedTask;
        }

        public override async UniTask EnterAsync(StateData stateData = null)
        {
            visualManager = new VisualManager(view.VisualFactory, view.EffectFactory, view.GameUiPanel);
            battleController = new BattleController();
            
            visualManager.LoadEnvironment("environment_0");

            var heroDeck = await ServiceLocator.Get<IPlayerManager>().GetHeroDeck();
            await battleController.InitializeBattle();
          
            view.Show(true);
        }

        public override UniTask ExecuteAsync() => UniTask.CompletedTask;

        public override UniTask ExitAsync()
        {
            battleController.Dispose();
            battleController = null;
            visualManager.Dispose();
            visualManager = null;
            view.Hide(true);
            return UniTask.CompletedTask;
        }

        private void OnBackButton()
        {
            StateMachine.SetStateAsync<MenuState>().Forget();
        }
    }
}