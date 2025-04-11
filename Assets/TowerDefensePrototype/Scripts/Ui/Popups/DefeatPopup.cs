using CastlePrototype.States;
using OneDay.Core.Modules.Sm;
using UnityEngine;
using UnityEngine.UI;

namespace CastlePrototype.Scripts.Ui.Popups
{
    public class DefeatPopup : AfterBattlePopup
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