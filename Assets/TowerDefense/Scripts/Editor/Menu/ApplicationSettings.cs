using UnityEngine;

namespace TowerDefense.Editor.Menu
{
    [CreateAssetMenu(fileName = "ApplicationSettings", menuName = "TD/Data/ApplicationSettings", order = 1)]

    public class ApplicationSettings : ScriptableObject
    {
        public string PathToBootScene;
        public string PathToVisualScene;
    }
}