using CastlePrototype.States;
using OneDay.Core.Modules.Sm;
using OneDay.Core.Modules.Ui;
using UnityEngine;
using UnityEngine.UI;

namespace CastlePrototype.Scripts.Ui.Popups
{
    public class VictoryPopup : UiPopup
    {
        public Button ClaimButton => claimButton;
        [SerializeField] private Button claimButton;
        
        private void Awake()
        {
            claimButton.onClick.AddListener(OnContinueClicked);
        }

        private void OnContinueClicked()
        {
            StateMachineEnvironment.Default.SetStateAsync<MenuState>();
            Close();
        }
    }
}