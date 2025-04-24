using UnityEditor;
using UnityEngine;

namespace TowerDefensePrototype.Scripts.Data.Definitions.Editor
{
    namespace OneDay.Core.Modules.Data
    {
        public static class TableLoader
        {
            public static T Load<T>() where T : ScriptableObject
            {
                string[] guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
                foreach (var guid in guids)
                {
                    var path = AssetDatabase.GUIDToAssetPath(guid);
                    var asset = AssetDatabase.LoadAssetAtPath<T>(path);
                    if (asset != null)
                    {
                        return asset;
                    }
                }

                Debug.LogWarning($"Asset of type {typeof(T)} not found in assets.");
                return null;
            }
        }
    }
}