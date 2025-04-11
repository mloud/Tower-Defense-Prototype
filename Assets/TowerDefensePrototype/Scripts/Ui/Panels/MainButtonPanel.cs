using System.Collections.Generic;
using System.Linq;
using OneDay.Core.Modules.Ui;
using UnityEngine;


namespace CastlePrototype.Ui.Panels
{
    public class MainButtonPanel : UiPanel
    {
        [SerializeField] private Transform buttonContainer;
        [SerializeField] private List<MainTabButton> buttons;

        [SerializeField] private MainTabButton defaultTabButton;
        private void Awake()
        {
            buttons = buttonContainer.GetComponentsInChildren<MainTabButton>(true).ToList();

            if (defaultTabButton != null)
            {
                defaultTabButton.SetSelected(true, callSelectAction:false);
            }

            buttons.ForEach(x => x.OnClick = OnButtonClicked);
        }

        private void OnButtonClicked(MainTabButton clickedButton)
        {
            // already selected
            if (clickedButton.Selected)
                return;

            clickedButton.SetSelected(true, callSelectAction:true);
            foreach (var button in buttons)
            {
                if (clickedButton != button)
                {
                    button.SetSelected(false, true);
                }
            }
        }
    }
}