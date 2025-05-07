using TowerDefense.Editor.Menu;
using UnityEditor;
using UnityEngine;

namespace TowerDefense.Scripts.Editor
{
    public class EditorTools
    {
        public static T[] LoadAllScriptableObjects<T>() where T : ScriptableObject
        {
            var guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
            var assets = new T[guids.Length];

            for (int i = 0; i < guids.Length; i++)
            {
                var path = AssetDatabase.GUIDToAssetPath(guids[i]);
                assets[i] = AssetDatabase.LoadAssetAtPath<T>(path);
            }

            return assets;
        }

        public static ApplicationSettings GetApplicationSettings()
        {
            var settings = EditorTools.LoadAllScriptableObjects<ApplicationSettings>();
            switch (settings.Length)
            {
                case 0:
                    EditorUtility.DisplayDialog("No ApplicationSettings asset found", $"Create first {nameof(ApplicationSettings)}", "OK");
                    return null;
                case > 1:
                    EditorUtility.DisplayDialog("More ApplicationSettings assets found", $"Delete unused of type {nameof(ApplicationSettings)}", "OK");
                    return null;
                default:
                    return settings[0];
            }
        }
    }
}