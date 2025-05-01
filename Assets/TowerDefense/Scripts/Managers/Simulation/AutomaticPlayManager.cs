using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using OneDay.Core;
using OneDay.Core.Debugging;
using OneDay.Core.Modules.Sm;
using TowerDefense.Battle.Logic.Managers.Skills;
using TowerDefense.Scripts.Managers;
using TowerDefense.States;
using UnityEngine;

namespace TowerDefense.Managers.Simulation
{
    [Flags]
    public enum SimulationType
    {
        WithoutVisuals = 1 << 0, // 2
    }
    
    public interface ISimulationMode
    {
        bool IsActive();
        bool IsSimulationTypeActive(SimulationType simulationFlag);
    }
    
    public interface IAutomaticPlayManager
    {
        UniTask Play();
        UniTask<ASkill> SelectSkill(List<ASkill> proposedSkills);
        void ProcessBattleEnd(int stage, float battleProgress01, bool playerWon);
    }
    [LogSection("AutomaticPlay")]
    public class AutomaticPlayManager : MonoBehaviour, IAutomaticPlayManager, IService, ISimulationMode
    {
        public bool IsActive() => isActive;
        [SerializeField] private ManualWorldRunner manualWorldRunner;
        [SerializeField] private ASimulation simulation;
        [SerializeField] private int timeScale;
        [SerializeField] private SimulationType simulationType;
        [SerializeField] private bool isActive;
      
        private SimulationResults results = new();
        
        public UniTask Initialize() => UniTask.CompletedTask;
        public UniTask PostInitialize() => UniTask.CompletedTask;

        public async UniTask Play()
        {
            if (!IsActive())
                return;

            manualWorldRunner.SetManualWorldUpdaterActive(true);

            if (simulation == null)
            {
                Debug.LogError("Simulation is not set");
                return;
            }
         
            await UniTask.WaitUntil(() => StateMachineEnvironment.Default.CurrentState is MenuState);
            await UniTask.WaitForSeconds(0.2f);
            Debug.Log($"XXX Simulation {simulation.GetType()} started");
            await simulation.Run();
            Debug.Log("XXX Simulation finished");
        }

        public async UniTask<ASkill> SelectSkill(List<ASkill> proposedSkills) =>
            await new SimulationTaskSelectSkill().Perform<ASkill>(proposedSkills);
        
        public void ProcessBattleEnd(int stage, float battleProgress01, bool playerWon)
        {
            if (simulation != null)
            {
                simulation.ProcessBattleEnd(stage, battleProgress01, playerWon);
            }
        }

        public bool IsSimulationTypeActive(SimulationType simulationType) 
            => IsActive() && (this.simulationType & simulationType) != 0;
    }
}