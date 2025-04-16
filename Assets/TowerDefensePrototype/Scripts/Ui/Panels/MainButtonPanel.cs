using System.Collections.Generic;
using System.Linq;
using CastlePrototype.Ui.Components;
using Cysharp.Threading.Tasks;
using OneDay.Core.Modules.Ui;
using UnityEngine;


namespace CastlePrototype.Ui.Panels
{
    public class MainButtonPanel : UiPanel
    {
        [SerializeField] private Transform buttonContainer;
        [SerializeField] private List<MainTabButton> buttons;
        [SerializeField] private MainTabButton defaultTabButton;
        [SerializeField] private CanLevelUpAnyHeroFlag canLevelUpAnyHeroFlag;

        public override async UniTask Initialize()
        {
            canLevelUpAnyHeroFlag.Initialize();
            await canLevelUpAnyHeroFlag.Refresh();
        }

        public override async UniTask Show(bool useSmooth, float speedMultiplier = 1.0f)
        {
            await base.Show(useSmooth, speedMultiplier);
            await canLevelUpAnyHeroFlag.Refresh();
        }
        
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