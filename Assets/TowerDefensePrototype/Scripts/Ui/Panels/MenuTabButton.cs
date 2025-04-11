using CastlePrototype.States;
using OneDay.Core.Modules.Sm;

namespace CastlePrototype.Ui.Panels
{
    public class MenuTabButton : MainTabButton
    {
        protected override void OnSelectAction()
        {
            StateMachineEnvironment.Default.SetStateAsync<MenuState>();
        }
    }
}