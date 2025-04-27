using System.IO;
using Newtonsoft.Json;
using OneDay.Core.Modules.Data;
using UnityEngine;


namespace TowerDefense.Data.Definitions
{
    public class DefinitionTable<T> : ScriptableObjectTable<T> where T: IDataObject
    {
#if UNITY_EDITOR
        private void OnValidate()
        {
            var json = Serialize();
            
            string assetName = name;
            string folder = Path.Combine(Application.dataPath, "SerializedData");

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            string filePath = Path.Combine(folder, assetName + ".json");

            File.WriteAllText(filePath, json);
            Debug.Log($"[StageDefinitionsTable] Auto-saved JSON to: {filePath}");
        }

        public void Load(string json)
        {
            Data.Clear();
            JsonConvert.PopulateObject(json, this);
        }

        public string Serialize() =>JsonConvert.SerializeObject(new { Data });
#endif
    }
}