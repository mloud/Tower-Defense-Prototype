using System;
using System.Collections.Generic;
using System.Linq;
using CastlePrototype.Data;
using CastlePrototype.Data.Definitions;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using OneDay.Core;
using OneDay.Core.Debugging;
using OneDay.Core.Extensions;
using OneDay.Core.Modules.Data;
using UnityEngine;

namespace CastlePrototype.Managers
{
    public interface IPlayerManager
    {
        public UniTask InitializePlayer();
        public UniTask<IPlayerProgress> GetProgression();
        public UniTask<HeroDeck> GetHeroDeck();
        public UniTask<RuntimeStageReward> AddRewardForBattle(int stage, float progression01);
    }
    
    public class PlayerManager : MonoBehaviour, IPlayerManager, IService
    {
        private IDataManager dataManager;

        public UniTask Initialize()
        {
            dataManager = ServiceLocator.Get<IDataManager>();
            return UniTask.CompletedTask;
        }

        public UniTask PostInitialize() => UniTask.CompletedTask;
        
        public async UniTask<IPlayerProgress> GetProgression()
            => (await dataManager.GetAll<PlayerProgress>()).FirstOrDefault();

        public async UniTask<HeroDeck> GetHeroDeck() =>
            (await dataManager.GetAll<HeroDeck>()).FirstOrDefault();

        public async UniTask<StageDefinition> GetStateDefinition(int stage) =>
            (await dataManager.GetAll<StageDefinition>()).ElementAt(stage);
        
        public async UniTask<RuntimeStageReward> AddRewardForBattle(int stage, float progression01)
        {
            var runtimeStageReward = new RuntimeStageReward();
            runtimeStageReward.Cards = new Dictionary<string, int>();
            
            var stageDefinition = await GetStateDefinition(stage);
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
                runtimeStageReward.Cards.Add(randomHero, givenCardsToUnit);
                totalCardsToDistribute -= givenCardsToUnit;
            }
            Debug.Assert(totalCardsToDistribute==0, "Not all cards we distributed");
            Debug.Log($"Cards were distributed:{JsonConvert.SerializeObject(runtimeStageReward.Cards)}");
            return runtimeStageReward;
        }
        
        public async UniTask InitializePlayer()
        {
            await CreateProgressIfNeeded(()=>new Player
            {
                EquippedWeapon = "weapon"
            });
            
            await CreateProgressIfNeeded(()=>new PlayerProgress
            {
                Xp = 0,
                Level = 1
            });

            await CreateProgressIfNeeded(() => new WeaponDeck
            {
                Weapons = new Dictionary<string, WeaponProgress>
                {
                    {
                        "canon",
                        new WeaponProgress
                        {
                            CardsCount = 0,
                            Level = 1
                        }
                    }
                }
            });
            
            await CreateProgressIfNeeded(() => new HeroDeck
            {
                Heroes = new Dictionary<string, HeroProgress>()
                {
                    {"weapon", new HeroProgress
                    {
                        CardsCount = 0,
                        Level = 1
                    }},
                    {"soldier", new HeroProgress
                    {
                        CardsCount = 0,
                        Level = 1
                    }},
                    {"tank", new HeroProgress
                    {
                        CardsCount = 0,
                        Level = 1
                    }},
                    {"turret", new HeroProgress
                    {
                        CardsCount = 0,
                        Level = 1
                    }},
                    {"dron", new HeroProgress
                    {
                        CardsCount = 0,
                        Level = 1
                    }},
                    {"scorpion", new HeroProgress
                    {
                        CardsCount = 0,
                        Level = 1
                    }},
                }
            });
        }

        private async UniTask CreateProgressIfNeeded<T>(Func<T> progressFactory) where T : BaseDataObject
        {
            var progress = await ServiceLocator.Get<IDataManager>().GetAll<T>();
            if (!progress.Any())
            {
                await ServiceLocator.Get<IDataManager>().Add(progressFactory());
                D.LogInfo($"Creating progress for {typeof(T)}", this);
            }
            else
            {
                D.LogInfo($"Progress for {typeof(T)} found", this);
            }
        }
    }
}