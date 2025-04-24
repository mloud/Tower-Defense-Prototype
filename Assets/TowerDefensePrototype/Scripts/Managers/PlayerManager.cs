using System;
using System.Collections.Generic;
using CastlePrototype.Data;
using CastlePrototype.Data.Definitions;
using CastlePrototype.Data.Progress;
using Cysharp.Threading.Tasks;
using OneDay.Core;
using OneDay.Core.Modules.Data;
using UnityEngine;

namespace CastlePrototype.Managers
{
    public interface IPlayerManager
    {
        Action<(HeroProgress progress, HeroDefinition definition)> OnHeroLeveledUp { get; set; } 
        Action<(int newXp, int nextXpNeeded, int prevLevel, int currentLevel)> OnXpChanged { get; set; }
        
        UniTask InitializePlayer();
        UniTask<PlayerProgress> GetProgression();
        UniTask<HeroDeck> GetHeroDeck();
        UniTask<RuntimeStageReward> FinishBattle(int stage, float progression01, bool won);
        UniTask<(bool, HeroProgress, HeroDefinition)> LevelUpHero(string heroId);
        UniTask<bool> CanLevelUpHero(string heroId);
        UniTask<bool> CanLevelUpAnyHero();
        UniTask<HeroDefinition> GetHeroDefinition(string heroId);
        UniTask<(HeroProgress progress, HeroDefinition definition)> GetUnlockedHero(string heroId);
        UniTask<IEnumerable<StageDefinition>> GetAllStageDefinitions();
        UniTask<StageDefinition> GetStageDefinition(int index);
        UniTask<PlayerProgressionDefinition> GetPlayerProgressionDefinition();
        UniTask<(int xp, int xpNextLevel, int level)> GetProgressionInfo();
    }

    public partial class PlayerManager : MonoBehaviour, IPlayerManager, IService
    {
        private IDataManager dataManager;

        public UniTask Initialize()
        {
            dataManager = ServiceLocator.Get<IDataManager>();
            return UniTask.CompletedTask;
        }

        public UniTask PostInitialize() => UniTask.CompletedTask;

        public async UniTask SaveProgression(PlayerProgress progress) => 
            await dataManager.Actualize<PlayerProgress>(progress);

        public async UniTask SaveHeroDeck(HeroDeck heroDeck) =>
            await dataManager.Actualize<HeroDeck>(heroDeck);


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
        
        public async UniTask<bool> CanLevelUpAnyHero()
        {
            var heroDeck = await GetHeroDeck();
            foreach (var hero in heroDeck.Heroes)
            {
                if (await CanLevelUpHero(hero.Key))
                    return true;
            }

            return false;
        }
    }
}