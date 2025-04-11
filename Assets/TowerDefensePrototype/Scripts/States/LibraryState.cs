using CastlePrototype.Ui.Views;
using Cysharp.Threading.Tasks;
using Meditation.States;
using OneDay.Core;
using OneDay.Core.Modules.Sm;
using OneDay.Core.Modules.Ui;

namespace CastlePrototype.States
{
    public class LibraryState : AState
    {
        private LibraryView view;
        
        public override UniTask Initialize()
        {
            view = ServiceLocator.Get<IUiManager>().GetView<LibraryView>();
            return UniTask.CompletedTask;
        }
        
        public override async UniTask EnterAsync(StateData stateData = null)
        {
            view.Show(true);
        }

        public override UniTask ExecuteAsync() => UniTask.CompletedTask;

        public override UniTask ExitAsync()
        {
            view.Hide(true);
            return UniTask.CompletedTask;
        }
    }
}