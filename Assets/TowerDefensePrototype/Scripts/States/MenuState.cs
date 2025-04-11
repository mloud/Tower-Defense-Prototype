using System;
using System.Linq;
using CastlePrototype.Ui.Panels;
using CastlePrototype.Ui.Views;
using Cysharp.Threading.Tasks;
using Meditation.States;
using OneDay.Core;
using OneDay.Core.Modules.Data;
using OneDay.Core.Modules.Sm;
using OneDay.Core.Modules.Ui;

namespace CastlePrototype.States
{
    public class MenuState : AState
    {
        private MenuView view;
        private IDataManager dataManager;
        
        public override UniTask Initialize()
        {
            dataManager = ServiceLocator.Get<IDataManager>();
            view = ServiceLocator.Get<IUiManager>().GetView<MenuView>();
            view.BindAction(view.PlayButton, OnPlayClicked);
            
            return UniTask.CompletedTask;
        }

        public override async UniTask EnterAsync(StateData stateData = null)
        {
            ServiceLocator.Get<IUiManager>().GetPanel<MainButtonPanel>().Show(true);
            view.Show(true);
        }

        public override UniTask ExecuteAsync() => UniTask.CompletedTask;

        public override UniTask ExitAsync()
        {
            view.Hide(true);
            return UniTask.CompletedTask;
        }
        
        private void OnPlayClicked()
        {
            StateMachine.SetStateAsync<GameState>().Forget();
        }
    }
}