using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

namespace TowerDefense.Managers.Simulation
{
    public class BattleSimulationResult
    {
        public int Stage { get; }
        public bool Won { get; }
        public int Percentage { get; }

        public BattleSimulationResult(int stage, bool won, int percentage)
        {
            Stage = stage;
            Won = won;
            Percentage = percentage;
        }
    }
    
    public class SimulationResults
    {
        private List<BattleSimulationResult> battleResults = new();

        public void AddBattleResult(BattleSimulationResult result)
        {
            battleResults.Add(result);
        }

        public void LogStageResults(int stage, string filename, DateTime simulationTime)
        {
            var stageResults = battleResults.Where(x => x.Stage == stage).ToList();
            if (stageResults.Any())
            {
                string json = JsonConvert.SerializeObject(new
                {
                    Stage = stage,
                    NumberOfBattles =  stageResults.Count,
                    BattleProgresses =  stageResults.Select(x => x.Percentage).ToList()
                }, Formatting.None);
                Debug.Log(json);
                
                string path = Path.Combine(
                    Application.dataPath, $"{filename}_{simulationTime:dd_HH-mm}.json");
                File.AppendAllText(path, json + Environment.NewLine);
                Debug.Log("XXX:" + json);          
            }
            else
            {
                Debug.Log($"No result for stage {stage} found");
            }
        }
    }
}