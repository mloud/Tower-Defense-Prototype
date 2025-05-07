using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using OneDay.Core;
using OneDay.Core.Extensions;
using OneDay.Core.Modules.Data;
using TowerDefense.Data;
using TowerDefense.Data.Definitions;
using TowerDefense.Data.Progress;
using TowerDefense.Managers;
using TowerDefense.Managers.Vallet;
using UnityEngine;

namespace TowerDefense.Managers
{
    public interface IStageGetter
    {
        UniTask<IEnumerable<StageDefinition>> GetAllStageDefinitions();
        UniTask<StageDefinition> GetStageDefinition(int index);
        UniTask<RuntimeStageReward> FinishBattle(int stage, float progression01, bool won);
    }
    
    public interface IStageSetter
    { }

    public interface IStagePlugin: IStageSetter, IStageGetter
    {}

    public class StagePlugin : IStagePlugin, IPlugin
    {
        private IDataManager dataManager;
        private IValetSetter valePlugin;
        private IDeckPlugin deckPlugin;
        private IProgressionPlugin progressionPlugin;
        
        public StagePlugin(IDataManager dataManager, IValetPlugin valePlugin, IDeckPlugin deckPlugin, IProgressionPlugin progressionPlugin)
        {
            this.dataManager = dataManager;
            this.valePlugin = valePlugin;
            this.deckPlugin = deckPlugin;
            this.progressionPlugin = progressionPlugin;
        } 
        
        public async UniTask<StageDefinition> GetStageDefinition(int stage) =>
            (await dataManager.GetAll<StageDefinition>()).ElementAt(stage);
        
        public async UniTask<IEnumerable<StageDefinition>> GetAllStageDefinitions() =>
            await dataManager.GetAll<StageDefinition>();
        
        
          public async UniTask<RuntimeStageReward> FinishBattle(int stage, float progression01, bool won)
        {
            var runtimeStageReward = new RuntimeStageReward();
            
            var stageDefinition = await GetStageDefinition(stage);
            var heroDeck = await deckPlugin.GetHeroDeck();
            int totalCardsToDistribute = (int)(stageDefinition.Reward.Cards * progression01);
            Debug.Log($"Cards to distribute {totalCardsToDistribute}");

            const int maxUnitsToDistribute = 2;
            int unitsToDistribute = Mathf.Min(maxUnitsToDistribute, heroDeck.Heroes.Count);
            var heroesToGiveCards = heroDeck.Heroes.Select(x => x.Key).ToList();
            
            for (int i = 0; i < unitsToDistribute; i++)
            {
                var randomHero = heroesToGiveCards.GetRandom();
                heroesToGiveCards.Remove(randomHero);
                int givenCardsToUnit = 0;
                // last unit get the rest of cards
                if (i == unitsToDistribute - 1)
                {
                    givenCardsToUnit = totalCardsToDistribute;
                }
                else
                {
                    // randomize number of cards for unit
                    int exactCardsToUnit = totalCardsToDistribute / (unitsToDistribute - i);
                    givenCardsToUnit = Math.Max(0,
                        UnityEngine.Random.Range(exactCardsToUnit - exactCardsToUnit / 2,
                            exactCardsToUnit + exactCardsToUnit / 2));
                }
                runtimeStageReward.AddCard(randomHero, givenCardsToUnit);
                totalCardsToDistribute -= givenCardsToUnit;
            }
            Debug.Assert(totalCardsToDistribute==0, "Not all cards we distributed");
            Debug.Log($"Cards were distributed:{JsonConvert.SerializeObject(runtimeStageReward.Cards)}");

            runtimeStageReward.AddCoins((int)Math.Round(stageDefinition.Reward.Coins * progression01));
            
            foreach (var reward in runtimeStageReward.Cards)
            {
                heroDeck.Heroes[reward.Key].CardsCount += reward.Value;
            }

            await deckPlugin.SaveHeroDeck(heroDeck);
            await valePlugin.AddCurrency(Currency.Coins, runtimeStageReward.Coins);
         

            // stage is finished
            if (won)
            {
                var progression = await progressionPlugin.GetProgression();
                var stagesDefinitions = await GetAllStageDefinitions();

                // we won again prev stage - dont do anything for now
                if (stage < progression.UnlockedStage)
                {
                    return runtimeStageReward;
                }

                // last stage played again
                if (stage == progression.LastFinishedStage)
                    return runtimeStageReward;

                if (stage > progression.LastFinishedStage)
                    progression.LastFinishedStage = stage;
                
                if (stage + 1 < stagesDefinitions.Count())
                {
                    progression.UnlockedStage++;
                }
     
           
                var playerProgressionDefinition = await progressionPlugin.GetPlayerProgressionDefinition();
                var xpNeededForNextLevel = playerProgressionDefinition.XpNeededToNextLevel[progression.Level];

                int prevLevel = progression.Level;
                
                progression.Xp += stageDefinition.Reward.Xp;
                if (progression.Xp >= xpNeededForNextLevel)
                {
                    progression.Xp -= xpNeededForNextLevel;
                    progression.Level++;
                    if (progression.Level < playerProgressionDefinition.HeroesUnlocks.Count)
                    {
                        var heroToUnlock = playerProgressionDefinition.HeroesUnlocks[progression.Level];

                        if (!string.IsNullOrEmpty(heroToUnlock))
                        {
                           heroDeck.Heroes.Add(heroToUnlock, new HeroProgress
                           {
                               Level = 1,
                               CardsCount = 0
                           });
                           await deckPlugin.SaveHeroDeck(heroDeck);
                           ServiceLocator.Get<IBufferedEventsManager>().Push(new NewLevelBufferedEvent( progression.Level, heroToUnlock));
                        }
                    }
                }

                int xpForNextLevel = progression.Level < playerProgressionDefinition.XpNeededToNextLevel.Count
                    ? playerProgressionDefinition.XpNeededToNextLevel[progression.Level]
                    : 0;
                await progressionPlugin.SaveProgression(progression);
                
                progressionPlugin.XpChanged?.Invoke(progression.Xp,xpForNextLevel, prevLevel, progression.Level);
            }
            
            return runtimeStageReward;
        }
    }
}