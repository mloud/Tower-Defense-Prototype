using CastlePrototype.Managers;
using CastlePrototype.Scripts.Ui.Popups;
using CastlePrototype.Ui.Panels;
using CastlePrototype.Ui.Views;
using Cysharp.Threading.Tasks;
using Meditation.States;
using OneDay.Core;
using OneDay.Core.Modules.Sm;
using OneDay.Core.Modules.Ui;
using UnityEngine;

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
            ServiceLocator.Get<IUiManager>().GetPanel<PlayerProfilePanel>().Show(true);
            
            await view.StageContainer.Refresh();
            view.Show(true);

            var bufferedEvents = ServiceLocator.Get<IBufferedEventsManager>()
                .PopAll<PlayerManager.NewLevelBufferedEvent>((int)BufferedEventsIds.NewLevel);
            
            Debug.Assert(bufferedEvents == null || bufferedEvents.Count <= 1, "Only zero or one hero unlocked allowed for now");

            if (bufferedEvents != null && bufferedEvents.Count == 1)
            {
                UniTask.Create(async () =>
                {
                    await UniTask.WaitForSeconds(1.0f);
                    ServiceLocator.Get<IUiManager>().OpenPopup<NewLevelPopup>(UiParameter.Create(bufferedEvents[0]));
                }).Forget();
            }
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