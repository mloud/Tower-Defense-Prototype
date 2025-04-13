using CastlePrototype.Ui.Components;
using OneDay.Core.Modules.Ui;
using UnityEngine;
using UnityEngine.UI;

namespace CastlePrototype.Ui.Views
{
    public class MenuView : UiView
    {
        public Button PlayButton => playButton;
        public StageContainer StageContainer => stageContainer;

        [SerializeField] private Button playButton;
        [SerializeField] private StageContainer stageContainer;
    }
}