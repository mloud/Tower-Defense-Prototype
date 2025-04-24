using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using OneDay.Core;
using OneDay.Core.Extensions;
using TowerDefense.Data;
using TowerDefense.Data.Progress;
using UnityEngine;

namespace TowerDefense.Managers
{
    public partial class PlayerManager
    {
        public class NewLevelBufferedEvent : BufferedEvent
        {
            public string HeroId { get; }
            public int Level { get; }
            public NewLevelBufferedEvent(int level, string heroId)
            {
                Type = (int)BufferedEventsIds.NewLevel;
                Level = level;
                HeroId = heroId;
            }
        }
        
        public async UniTask<RuntimeStageReward> FinishBattle(int stage, float progression01, bool won)
        {
            var runtimeStageReward = new RuntimeStageReward();
            
            var stageDefinition = await GetStageDefinition(stage);
            var heroDeck = await GetHeroDeck();
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


            foreach (var reward in runtimeStageReward.Cards)
            {
                heroDeck.Heroes[reward.Key].CardsCount += reward.Value;
            }

            await SaveHeroDeck(heroDeck);


            // stage is finished
            if (won)
            {
                var progression = await GetProgression();
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
     
           
                var playerProgressionDefinition = await GetPlayerProgressionDefinition();
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
                           await SaveHeroDeck(heroDeck);
                           ServiceLocator.Get<IBufferedEventsManager>().Push(new NewLevelBufferedEvent( progression.Level, heroToUnlock));
                        }
                    }
                }

                int xpForNextLevel = progression.Level < playerProgressionDefinition.XpNeededToNextLevel.Count
                    ? playerProgressionDefinition.XpNeededToNextLevel[progression.Level]
                    : 0;
                await SaveProgression(progression);
                OnXpChanged?.Invoke((progression.Xp,xpForNextLevel, prevLevel, progression.Level));
            }
            
            return runtimeStageReward;
        }
    }
}