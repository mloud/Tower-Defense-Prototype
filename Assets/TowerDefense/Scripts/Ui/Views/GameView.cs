using OneDay.Core.Modules.Ui;
using TowerDefense.Battle.Visuals;
using UnityEngine;

namespace TowerDefense.Ui.Views
{
    public class GameView : UiView
    {
        public PrefabVisualFactory VisualFactory => visualFactory;
        public Transform GameUiPanel => gameUiPanel;


        [SerializeField] private PrefabVisualFactory visualFactory;
        [SerializeField] private Transform gameUiPanel;
    }
}