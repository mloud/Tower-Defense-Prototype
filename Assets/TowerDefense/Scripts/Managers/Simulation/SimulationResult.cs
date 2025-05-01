using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace TowerDefense.Managers.Simulation
{
    public class BattleSimulationResult
    {
        public int SimulationRun { get; set; }
        public int BattleRun { get; set; }
        public int Stage { get; }
        public bool Won { get; }
        public int Percentage { get; }
        public int CardsLeveledUp { get; private set; }

        public BattleSimulationResult(int stage, bool won, int percentage)
        {
        
            Stage = stage;
            Won = won;
            Percentage = percentage;
        }

        public void AddCardsLevelUp(int count) => CardsLeveledUp = count;
    }
    
    public class SimulationResults
    {
        public IReadOnlyList<BattleSimulationResult> All => battleResults;
        private List<BattleSimulationResult> battleResults = new();

        public void Add(BattleSimulationResult result)
        {
            if (result == null) throw new ArgumentNullException(nameof(result));
            battleResults.Add(result);
        }

        public BattleSimulationResult this[int index] => battleResults[index];


        public void LogStageResults(string filename)
        {
            var groupedBySimulationRun = battleResults.GroupBy(x => x.SimulationRun);
            if (!groupedBySimulationRun.Any())
            {
                Debug.LogWarning("No simulation results found.");
                return;
            }

            var jObject = new JObject();
            foreach (var group in groupedBySimulationRun)
            {
                var jData = new JObject
                {
                    { "NumberOfBattles", group.Count() },
                    { "BattleProgresses", new JArray(group.Select(r => r.Percentage)) },
                    { "CardsLeveledUp", new JArray(group.Select(r => r.CardsLeveledUp)) }
                };
                jObject.Add($"SimulationRun {group.Key}", jData);
            }

            var json = jObject.ToString(Formatting.Indented);
            Debug.Log(json);

            string path = Path.Combine(
                Application.dataPath, $"Simulation_{Path.GetFileNameWithoutExtension(filename)}_{DateTime.Now:dd_HH-mm}.json");
            File.AppendAllText(path, json + Environment.NewLine);
            Debug.Log("XXX:" + json);
        }
    }
}