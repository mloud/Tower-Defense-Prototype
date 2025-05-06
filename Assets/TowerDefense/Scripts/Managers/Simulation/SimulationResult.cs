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

        public List<string> UsedSkills { get; private set; }
        public BattleSimulationResult(int stage, bool won, int percentage, List<string> usedSkills)
        {
            Stage = stage;
            Won = won;
            Percentage = percentage;
            UsedSkills = usedSkills;
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
            var groupedBySimulationRun = battleResults.GroupBy(x => x.SimulationRun).ToList();
            if (!groupedBySimulationRun.Any())
            {
                Debug.LogWarning("No simulation results found.");
                return;
            }

            var jObject = new JObject();
            int cardsLeveledUp = 0;
            foreach (var simulation in groupedBySimulationRun)
            {
                var jData = new JObject
                {
                    { "NumberOfBattles", simulation.Count() },
                    { "BattleProgresses", new JArray(simulation.Select(r => r.Percentage)) },
                    { "CardsLeveledUp", new JArray(simulation.Select(r => r.CardsLeveledUp).ToList().SkipLast(1))},
                    { "UsedSkills", new JArray(new JArray(simulation.Select(r=>r.UsedSkills)))}
                };
                jObject.Add($"SimulationRun {simulation.Key}", jData);
                cardsLeveledUp += simulation.Select(r => r.CardsLeveledUp).ToList().SkipLast(1).Sum();
            }

            int totalBattles = groupedBySimulationRun.Sum(simulation => simulation.Count());
         
          

            jObject["Statistic"] = new JObject
            {
                ["Average battles needed to win"] = totalBattles / (float)groupedBySimulationRun.Count,
                ["Average cards leveled up between"] = cardsLeveledUp / (float)(totalBattles - battleResults.Count(x => !x.Won))
            };
            
            var json = jObject.ToString(Formatting.Indented);
            Debug.Log(json);

            string path = Path.Combine(
                Application.dataPath, $"Simulation_{Path.GetFileNameWithoutExtension(filename)}_{DateTime.Now:dd_HH-mm}.json");
            File.AppendAllText(path, json + Environment.NewLine);
            Debug.Log("XXX:" + json);
        }
    }
}