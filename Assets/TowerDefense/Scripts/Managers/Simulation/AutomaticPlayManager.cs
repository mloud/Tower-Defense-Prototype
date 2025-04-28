using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using OneDay.Core;
using OneDay.Core.Debugging;
using OneDay.Core.Extensions;
using OneDay.Core.Modules.Sm;
using TowerDefense.Battle.Logic.Managers.Skills;
using TowerDefense.Scripts.Managers;
using TowerDefense.States;
using UnityEngine;
using Environment = System.Environment;

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
        ASkill SelectSkill(List<ASkill> proposedSkills);
        void ProcessBattleEnd(int stage, float battleProgress01, bool playerWon);
    }
    [LogSection("AutomaticPlay")]
    public class AutomaticPlayManager : MonoBehaviour, IAutomaticPlayManager, IService, ISimulationMode
    {
        public bool IsActive() => isActive;
        [SerializeField] private ManualWorldRunner manualWorldRunner;
        [SerializeField] private int timeScale;
        [SerializeField] private int stageToPlay;
        [SerializeField] private string FilenameWithResult = "Simulation Results.json";
        [SerializeField] private int gameSimulationsCount = 1;
        [SerializeField] private SimulationType simulationType;
        [SerializeField] private bool isActive;
        private bool waitingForBattle;
        private List<string> stringList = new();

        private SimulationResults results = new();
        private DateTime simulationTime;

        private int gameSimulationCounter;
        
        private Action<int, float, bool> customBattleEndProcessor;
        public UniTask Initialize()
        {
            simulationTime = DateTime.Now;
            return UniTask.CompletedTask;
        }

        public UniTask PostInitialize() => UniTask.CompletedTask;


        public async UniTask Play()
        {
            if (!IsActive())
                return;

            manualWorldRunner.SetManualWorldUpdaterActive(true);
            gameSimulationCounter++;
         
            await UniTask.WaitUntil(() => StateMachineEnvironment.Default.CurrentState is MenuState);
            await UniTask.WaitForSeconds(0.2f);
          
            async UniTask<bool> IsGameFinished()
            {
                int lastFinishedStageIndex = (await ServiceLocator.Get<IPlayerManager>().ProgressionGetter.GetProgression())
                    .LastFinishedStage;

                int totalStages = (await ServiceLocator.Get<IPlayerManager>().StageGetter.GetAllStageDefinitions())
                    .Count();
                return lastFinishedStageIndex >= totalStages - 1;
            }
   
            int maxBattles = 100;
            int battleCounts = 0;
            var startTime = DateTime.Now;
            while (!await IsGameFinished() && battleCounts < maxBattles)
            {
                await UniTask.WaitUntil(() => StateMachineEnvironment.Default.CurrentState is MenuState);
                await UniTask.WaitForSeconds(0.2f);
                waitingForBattle = true;
                int stage = (await ServiceLocator.Get<IPlayerManager>().ProgressionGetter.GetProgression())
                    .UnlockedStage;

                    StateMachineEnvironment.Default.SetStateAsync<GameState>(StateData.Create(("stage", stage)));
                    await UniTask.WaitUntil(() => waitingForBattle == false);
                    await UniTask.WaitForSeconds(0.2f);
             
                await TryToLevelUnit();
                battleCounts++;
            }

            if (battleCounts == maxBattles)
            {
                Debug.LogError("XXX game could not be finished");
            }
            else
            {
                Debug.Log($"XXX game simulation {gameSimulationCounter} / {gameSimulationsCount} finished in {DateTime.Now - startTime}");
                
                PlayerPrefs.DeleteAll();
                ServiceLocator.Get<IPlayerManager>().InitializePlayer();

                if (gameSimulationCounter < gameSimulationsCount)
                {
                    Play();
                }
            }
        }

        public ASkill SelectSkill(List<ASkill> proposedSkills)
        {
            var unlockHeroes = proposedSkills.Where(x => x.SkillType == SkillType.UnlockHero).ToList();
            if (unlockHeroes.Any())
            {
                return unlockHeroes.GetRandom();
            }

            proposedSkills.RemoveAll(x => x.SkillType == SkillType.IncreaseAttackDistance);
            
            return proposedSkills.GetRandom();
        }

        public void ProcessBattleEnd(int stage, float battleProgress01, bool playerWon)
        {
            if (customBattleEndProcessor != null)
            {
                customBattleEndProcessor(stage, battleProgress01, playerWon);
            }
            else
            {
                var battleResult = new BattleSimulationResult(stage, playerWon,
                    (int)Mathf.Round(battleProgress01 * 100));
                results.AddBattleResult(battleResult);
                waitingForBattle = false;
                StateMachineEnvironment.Default.SetStateAsync<MenuState>();

                string path = Path.Combine(
                    Application.dataPath, $"{FilenameWithResult}_{simulationTime:dd_HH-mm}.json");
                File.AppendAllText(path, JsonConvert.SerializeObject(battleResult) + Environment.NewLine);
                //if (playerWon)
               // {
              //      results.LogStageResults(stage,  FilenameWithResult, simulationTime);
               // }
            }
        }

        private async UniTask TryToLevelUnit()
        {
            var playerManager = ServiceLocator.Get<IPlayerManager>();
            var heroesInDeck  =(await playerManager.DeckGetter.GetHeroDeck()).Heroes;
            var heroIds = heroesInDeck.Keys.ToList();


            while (await playerManager.DeckGetter.CanLevelUpAnyHero())
            {
                stringList.Clear();
                foreach (var heroId in heroIds)
                {
                    bool canLevelUpHero = await playerManager.DeckGetter.CanLevelUpHero(heroId);
                    if (canLevelUpHero)
                    {
                        stringList.Add(heroId);
                    }
                }

                var rndHero = stringList.GetRandom();
                await playerManager.DeckGetter.LevelUpHero(rndHero);
            }
        }


        public bool IsSimulationTypeActive(SimulationType simulationType) 
            => IsActive() && (this.simulationType & simulationType) != 0;
    }
}