using OneDay.Core.Modules.Sm;
using TowerDefense.States;

namespace TowerDefense.Ui.Panels
{
    public class LibraryTabButton : MainTabButton
    {
        protected override void OnSelectAction()
        {
            StateMachineEnvironment.Default.SetStateAsync<LibraryState>();
        }
    }
}