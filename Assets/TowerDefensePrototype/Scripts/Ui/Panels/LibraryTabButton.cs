using CastlePrototype.States;
using OneDay.Core.Modules.Sm;

namespace CastlePrototype.Ui.Panels
{
    public class LibraryTabButton : MainTabButton
    {
        protected override void OnSelectAction()
        {
            StateMachineEnvironment.Default.SetStateAsync<LibraryState>();
        }
    }
}