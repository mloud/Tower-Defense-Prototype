using OneDay.Core.Modules.Ui;
using UnityEngine;
using UnityEngine.UI;

namespace CastlePrototype.Ui.Views
{
    public class MenuView : UiView
    {
        public Button PlayButton => playButton;
        [SerializeField] private Button playButton;
    }
}