using CastlePrototype.States;
using OneDay.Core;
using OneDay.Core.Modules.Sm;
using OneDay.Core.Modules.Ui;
using UnityEngine;
using UnityEngine.UI;

namespace CastlePrototype.Scripts.Ui.Popups
{
    public class DefeatPopup : UiPopup
    {
        [SerializeField] private Button continueButton;

        private void Awake()
        {
            continueButton.onClick.AddListener(OnContinueClicked);
        }

        private void OnContinueClicked()
        {
            StateMachineEnvironment.Default.SetStateAsync<MenuState>();
            Close();
        }
    }
}