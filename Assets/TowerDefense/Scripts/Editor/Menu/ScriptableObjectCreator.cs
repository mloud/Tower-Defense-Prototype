using System.Reflection;
using TowerDefense.Data.Definitions;
using TowerDefense.Data.Definitions.CastlePrototype.Data.Definitions;
using UnityEditor;
using UnityEngine;

namespace TowerDefense.Editor.Menu
{
    public class ScriptableObjectCreator
    {
        [MenuItem("TD/Create/Enemy Definitions Table",priority = 300)]
        public static void CreateEnemyDefinitionScriptableObject() => CreateMyScriptableObject<EnemyDefinitionsTable>();

        [MenuItem("TD/Create/Hero Definitions Table",priority = 301)]
        public static void CreateHeroDefinitionScriptableObject() => CreateMyScriptableObject<HeroDefinitionsTable>();
        
        [MenuItem("TD/Create/Player Progression Definition Table",priority = 302)]
        public static void CreatePlayerProgressionDefinitionTable() => CreateMyScriptableObject<PlayerProgressionDefinitionTable>();
        
        [MenuItem("TD/Create/Stage Definition Table",priority = 303)]
        public static void CreateStageDefinitionTable() => CreateMyScriptableObject<StageDefinitionsTable>();
        
        [MenuItem("TD/Create/Application Settings",priority = 304)]
        public static void CreateApplicationSettings() => CreateMyScriptableObject<ApplicationSettings>();
       
        private static void CreateMyScriptableObject<T>() where T: ScriptableObject
        {
        
            
            var attribute = typeof(T).GetCustomAttribute<CreateAssetMenuAttribute>();
            var fileName = attribute.fileName;
            
            var asset = ScriptableObject.CreateInstance<T>();

            var path = EditorUtility.SaveFilePanelInProject(
                $"Save {fileName}",
                fileName,
                "asset",
                "Choose location to save");

            if (string.IsNullOrEmpty(path)) return;
            
            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
        }
    }
}