using Cysharp.Threading.Tasks;
using Meditation.States;
using OneDay.Core.Modules.Sm;

namespace TowerDefense.States
{
    public class BootState : AState
    {
        public override UniTask Initialize() => UniTask.CompletedTask;
        
        public override UniTask EnterAsync(StateData stateData = null)
        {
            StateMachine.SetStateAsync<MenuState>().Forget();
            return UniTask.CompletedTask;
        }

        public override UniTask ExecuteAsync() => UniTask.CompletedTask;
        public override UniTask ExitAsync() => UniTask.CompletedTask;
    }
}