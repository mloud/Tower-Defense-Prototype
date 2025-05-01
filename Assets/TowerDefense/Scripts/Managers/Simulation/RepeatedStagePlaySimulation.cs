using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using OneDay.Core;
using OneDay.Core.Modules.Sm;
using TowerDefense.States;
using UnityEngine;

namespace TowerDefense.Managers.Simulation
{
    public class RepeatedStagePlaySimulation : ASimulation
    {
        [SerializeField] private string playerProgressFilePath;
        [SerializeField] private int stageRepetitionCount = 2;
        
        private bool waitingForBattleEnd;
        private bool stageFinished;

        private SimulationResults simulationResults;
        
        protected override async UniTask OnRun()
        {
            simulationResults = new SimulationResults();
            for (int i = 0; i < stageRepetitionCount; i++)
            {
                SimulationUtils.SetPlayerStateFromFile(playerProgressFilePath);
                stageFinished = false;
                int battleCounter = 0;
              
                
                while (!stageFinished)
                {
                    waitingForBattleEnd = true;
                    await UniTask.WaitUntil(() => StateMachineEnvironment.Default.CurrentState is MenuState);
                    await UniTask.WaitForSeconds(0.2f);
                    int stage = (await ServiceLocator.Get<IPlayerManager>().ProgressionGetter.GetProgression())
                        .UnlockedStage;

                    StateMachineEnvironment.Default.SetStateAsync<GameState>(StateData.Create(("stage", stage)));
                    await UniTask.WaitUntil(() => waitingForBattleEnd == false);
                    await UniTask.WaitForSeconds(0.2f);
                    StateMachineEnvironment.Default.SetStateAsync<MenuState>();
                    var leveledUpHeroes = await new SimulationTaskLevelUp().Perform<List<string>>(null);
                    simulationResults[simulationResults.All.Count - 1].SimulationRun = i;
                    simulationResults[simulationResults.All.Count - 1].BattleRun = battleCounter;
                    simulationResults[simulationResults.All.Count- 1].AddCardsLevelUp(leveledUpHeroes.Count);
                    battleCounter++;
                }
                Debug.Log($"XXX === Stage simulation finished after {battleCounter} battles");
            }
            simulationResults.LogStageResults($"Simulation_{playerProgressFilePath}.");

        }
     

        protected override void OnProcessBattleEnd(int stage, float battleProgress01, bool playerWon)
        {
            waitingForBattleEnd = false;
            stageFinished = playerWon;
            Debug.Log($"XXX battle in {stage} ended with progress: {battleProgress01}");
            simulationResults.Add(new BattleSimulationResult(stage, playerWon, (int)Mathf.Round(battleProgress01 * 100)));
        }
    }
}