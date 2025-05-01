using System.IO;
using TowerDefense.Data;
using TowerDefense.Managers.Simulation;
using Unity.Plastic.Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

namespace TowerDefense.Scripts.Data.Definitions.Editor
{
    public class PlayerSaveEditor
    {
        private const string SaveFilePath = "PlayerSaves";

        [MenuItem("Tools/Save Player Progress")]
        public static void SavePlayerPrefsToFile()
        {
            SimulationUtils.SavePlayerStateToFile();
        }

        [MenuItem("Tools/Load Player Progress")]
        public static void LoadPlayerPrefsFromFile()
        {
            // Open a file panel
            string path = EditorUtility.OpenFilePanel("Select Save File", Path.Combine(Application.dataPath,"PlayerSaves"), "pjson");
            SimulationUtils.SetPlayerStateFromFile(path);
        }
    }
}