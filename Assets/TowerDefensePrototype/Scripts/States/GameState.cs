using CastlePrototype.Battle.Logic;
using CastlePrototype.Battle.Visuals;
using CastlePrototype.Battle.Visuals.Effects;
using CastlePrototype.Managers;
using CastlePrototype.Ui.Views;
using Cysharp.Threading.Tasks;
using Meditation.States;
using OneDay.Core;
using OneDay.Core.Modules.Assets;
using OneDay.Core.Modules.Data;
using OneDay.Core.Modules.Pooling;
using OneDay.Core.Modules.Sm;
using OneDay.Core.Modules.Ui;
using TowerDefensePrototype.Battle.Visuals.Effects;


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
      
        
        public override UniTask Initialize()
        {
            dataManager = ServiceLocator.Get<IDataManager>();
            poolManager = ServiceLocator.Get<IPoolManager>();
            
            view = ServiceLocator.Get<IUiManager>().GetView<GameView>();
            view.BindAction(view.BackButton, OnBackButton);
            
            return UniTask.CompletedTask;
        }

        public override async UniTask EnterAsync(StateData stateData = null)
        {
            await PoolEffects();    
            var effectFactory = new PoolingEffectFactory(poolManager);
            
            visualManager = new VisualManager(view.VisualFactory, effectFactory, view.GameUiPanel);
            battleController = new BattleController();
            
            visualManager.LoadEnvironment("environment_0");

            var heroDeck = await ServiceLocator.Get<IPlayerManager>().GetHeroDeck();
            await battleController.InitializeBattle();
          
            view.Show(true);
        }

        public override UniTask ExecuteAsync() => UniTask.CompletedTask;

        public override UniTask ExitAsync()
        {
            ReleasePooledEffects();
            
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

        private async UniTask PoolEffects()
        {
            await poolManager.PreloadAsync(EffectKeys.HitEffectSmall, 50);
            await poolManager.PreloadAsync(EffectKeys.HitEffectAoeNormal, 10);
            await poolManager.PreloadAsync(EffectKeys.HpDamageText, 20);
        }

        private void ReleasePooledEffects()
        {
            poolManager.ClearPool(EffectKeys.HitEffectSmall);
            poolManager.ClearPool(EffectKeys.HitEffectAoeNormal);
            poolManager.ClearPool(EffectKeys.HpDamageText);
        }
    }
}