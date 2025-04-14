using CastlePrototype.Ui.Panels;
using CastlePrototype.Ui.Views;
using Cysharp.Threading.Tasks;
using Meditation.States;
using OneDay.Core;
using OneDay.Core.Modules.Sm;
using OneDay.Core.Modules.Ui;

namespace CastlePrototype.States
{
    public class MenuState : AState
    {
        private MenuView view;
        
        public override UniTask Initialize()
        {
            view = ServiceLocator.Get<IUiManager>().GetView<MenuView>();
            view.BindAction(view.PlayButton, OnPlayClicked);
            
            return UniTask.CompletedTask;
        }

        public override async UniTask EnterAsync(StateData stateData = null)
        {
            ServiceLocator.Get<IUiManager>().GetPanel<MainButtonPanel>().Show(true);
            await view.StageContainer.Refresh();
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
            int stage = view.StageContainer.SelectedStageIndex;
            StateMachine.SetStateAsync<GameState>(StateData.Create(("stage", stage))).Forget();
        }
    }
}