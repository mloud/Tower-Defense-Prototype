using OneDay.Core.Modules.Ui;
using TowerDefense.Ui.Components;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense.Ui.Views
{
    public class MenuView : UiView
    {
        public Button PlayButton => playButton;
        public StageContainer StageContainer => stageContainer;

        [SerializeField] private Button playButton;
        [SerializeField] private StageContainer stageContainer;
    }
}