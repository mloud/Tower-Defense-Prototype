using System;
using System.Linq;
using CastlePrototype.Data;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using OneDay.Core.Extensions;
using UnityEngine;

namespace CastlePrototype.Managers
{
    public partial class PlayerManager
    {
        public async UniTask<RuntimeStageReward> FinishBattle(int stage, float progression01)
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
            if (progression01 == 1)
            {
                var progression = await GetProgression();
                var stagesDefinitions = await GetAllStageDefinitions();
                if (stage == progression.UnlockedStage && stage + 1 < stagesDefinitions.Count())
                {
                    progression.UnlockedStage++;
                }

                await SaveProgression(progression);
            }
            
            return runtimeStageReward;
        }
    }
}