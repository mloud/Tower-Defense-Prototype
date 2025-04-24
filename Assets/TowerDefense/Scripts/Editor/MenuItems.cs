using UnityEditor;
using UnityEngine;

namespace TowerDefensePrototype.Scripts
{
    public static class MenuItems
    {
        [MenuItem("TD/Clear Player Prefs")]
        public static void ClearPlayerPrefs()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
        }
    }
}