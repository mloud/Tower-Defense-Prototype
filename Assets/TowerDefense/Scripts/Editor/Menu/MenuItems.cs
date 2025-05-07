using TowerDefense.Scripts.Editor;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace TowerDefense.Editor.Menu
{
    public static class MenuItems
    {
        [MenuItem("TD/Clear Player Prefs", priority = 400)]
        public static void ClearPlayerPrefs()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
        }

        
        [MenuItem("TD/Run Main Scene",priority = 0)]
        public static void RunMainScene()
        {
            EditorSceneManager.OpenScene(EditorTools.GetApplicationSettings().PathToBootScene);
            EditorApplication.isPlaying = true;
        }
        
        [MenuItem("TD/Open Main Scene",priority = 1)]
        public static void OpenMainScene() => 
            EditorSceneManager.OpenScene(EditorTools.GetApplicationSettings().PathToBootScene);

        
        [MenuItem("TD/Open Visual Scene",priority = 2)]
        public static void OpenVisualScene() => 
            EditorSceneManager.OpenScene(EditorTools.GetApplicationSettings().PathToVisualScene);
    }
}