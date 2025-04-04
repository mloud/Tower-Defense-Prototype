using CastlePrototype.Battle.Visuals;
using OneDay.Core.Modules.Ui;
using UnityEngine;

namespace CastlePrototype.Ui.Views
{
    public class GameView : UiView
    {
        public PrefabVisualFactory VisualFactory => visualFactory;
        public Transform GameUiPanel => gameUiPanel;


        [SerializeField] private PrefabVisualFactory visualFactory;
        [SerializeField] private Transform gameUiPanel;
    }
}