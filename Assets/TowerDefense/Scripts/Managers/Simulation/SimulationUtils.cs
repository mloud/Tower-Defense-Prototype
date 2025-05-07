using System.IO;
using Newtonsoft.Json.Linq;
using TowerDefense.Data;
using UnityEditor;
using UnityEngine;

namespace TowerDefense.Managers.Simulation
{
    public static class SimulationUtils
    {
        public static void SavePlayerStateToFile()
        {
            #if UNITY_EDITOR
            if (!PlayerPrefs.HasKey(TypeToDataKeyBinding.PlayerProgress))
            {
                EditorUtility.DisplayDialog("Player progress not found", "Run the game first to create player", "OK");
                return;
            }
            #endif
         
            var table = JArray.Parse(PlayerPrefs.GetString(TypeToDataKeyBinding.PlayerProgress));
            Debug.Assert(table.Count > 0);
            int stage = table[0]["UnlockedStage"]!.ToObject<int>();
            var fileName = $"PlayerSaves/PlayerAtStage_{stage}.pjson";

            var allPrefs = new JObject();
           
            foreach (var key in PlayerPrefsKeys())
            {
                if (PlayerPrefs.HasKey(key))
                {
                    var keyData = PlayerPrefs.GetString(key);
                    allPrefs.Add(key, keyData);
                }
            }

            var filePath = Path.Combine(Application.dataPath, fileName);
            File.WriteAllText(filePath, allPrefs.ToString());
            Debug.Log($"PlayerPrefs Progress: {filePath}");
        }
        public static void SetPlayerStateFromFile(string path)
        {
            var filePath = Path.Combine(Application.dataPath, path);
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                Debug.LogError("No valid file selected.");
                return;
            }

            string json = File.ReadAllText(filePath);
            JObject saveData;
            try
            {
                saveData = JObject.Parse(json);
            }
            catch
            {
                Debug.LogError("Invalid JSON file.");
                return;
            }

            foreach (var pair in saveData)
            {
                PlayerPrefs.SetString(pair.Key, pair.Value.ToString());
            }

            PlayerPrefs.Save();
            Debug.Log("PlayerPrefs loaded from file: " + filePath);
        }
        
        private static string[] PlayerPrefsKeys() =>
            new[]
            {
                TypeToDataKeyBinding.PlayerProgress,
                TypeToDataKeyBinding.HeroDeck,
                TypeToDataKeyBinding.Valet,
            };
    }
}