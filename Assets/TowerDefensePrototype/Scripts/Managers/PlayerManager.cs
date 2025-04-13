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
        Action<(HeroProgress progress, HeroDefinition definition)> OnHeroLeveledUp { get; set; }
        UniTask InitializePlayer();
        UniTask<PlayerProgress> GetProgression();
        UniTask<HeroDeck> GetHeroDeck();
        UniTask<RuntimeStageReward> FinishBattle(int stage, float progression01);
        UniTask<(bool, HeroProgress, HeroDefinition)> LevelUpHero(string heroId);
        UniTask<bool> CanLevelUpHero(string heroId);
        UniTask<HeroDefinition> GetHeroDefinition(string heroId);
        UniTask<(HeroProgress progress, HeroDefinition definition)> GetUnlockedHero(string heroId);
        UniTask<IEnumerable<StageDefinition>> GetAllStageDefinitions();
    }

    public partial class PlayerManager : MonoBehaviour, IPlayerManager, IService
    {
        public Action<(HeroProgress progress, HeroDefinition definition)> OnHeroLeveledUp { get; set; }
        
        private IDataManager dataManager;

        public UniTask Initialize()
        {
            dataManager = ServiceLocator.Get<IDataManager>();
            return UniTask.CompletedTask;
        }

        public UniTask PostInitialize() => UniTask.CompletedTask;

        public async UniTask<PlayerProgress> GetProgression()
            => (await dataManager.GetAll<PlayerProgress>()).FirstOrDefault();

        public async UniTask SaveProgression(PlayerProgress progress) => 
            await dataManager.Actualize<PlayerProgress>(progress);


        public async UniTask<HeroDeck> GetHeroDeck() =>
            (await dataManager.GetAll<HeroDeck>()).FirstOrDefault();

        public async UniTask<HeroDefinition> GetHeroDefinition(string heroId) =>
            (await dataManager.GetAll<HeroDefinition>()).FirstOrDefault(x => x.UnitId == heroId);

        public async UniTask SaveHeroDeck(HeroDeck heroDeck) =>
            await dataManager.Actualize<HeroDeck>(heroDeck);


        public async UniTask<StageDefinition> GetStageDefinition(int stage) =>
            (await dataManager.GetAll<StageDefinition>()).ElementAt(stage);

        public async UniTask<IEnumerable<StageDefinition>> GetAllStageDefinitions() =>
            await dataManager.GetAll<StageDefinition>();


        public async UniTask<(HeroProgress progress, HeroDefinition definition)> GetUnlockedHero(string heroId)
        {
            var heroDefinition = await GetHeroDefinition(heroId);
            var heroProgress = (await GetHeroDeck()).Heroes[heroId];
            return (heroProgress, heroDefinition);
        }

        public async UniTask<bool> CanLevelUpHero(string heroId)
        {
            var heroDeck = await GetHeroDeck();
            var heroProgress = heroDeck.Heroes[heroId];
            var heroDefinition = await GetHeroDefinition(heroId);
            int cardsNeeded = heroDefinition.GetCardsNeededToLevelUp(heroProgress.Level);
            if (cardsNeeded > heroProgress.CardsCount)
                return false;
            if (heroDefinition.IsMaxLevel(heroProgress.Level))
                return false;
            return true;
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
                    {"barricade", new HeroProgress
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