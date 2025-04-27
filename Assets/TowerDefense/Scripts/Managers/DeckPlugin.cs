using System.Linq;
using Cysharp.Threading.Tasks;
using OneDay.Core.Modules.Data;
using TowerDefense.Data.Definitions;
using TowerDefense.Data.Progress;
using TowerDefense.Managers.Vallet;
using UnityEngine;

namespace TowerDefense.Scripts.Managers
{
    public delegate void CurrencyChangedDelegate(HeroProgress progress, HeroDefinition definition);
  
    public interface IDeckGetter
    {
        CurrencyChangedDelegate OnHeroLeveledUp { get; set; }
        UniTask<HeroDeck> GetHeroDeck();
        UniTask<HeroDefinition> GetHeroDefinition(string heroId);
        UniTask<bool> CanLevelUpHero(string heroId);
        UniTask<(HeroProgress progress, HeroDefinition definition)> GetUnlockedHero(string heroId);
        UniTask<bool> CanLevelUpAnyHero();
        UniTask<(bool, HeroProgress, HeroDefinition)> LevelUpHero(string heroId);
    }
    
    public interface IDeckSetter
    { 
        UniTask SaveHeroDeck(HeroDeck heroDeck);
    }
    
    public interface IDeckPlugin: IDeckGetter, IDeckSetter
    {}
    
    public class DeckPlugin : IDeckPlugin, IPlugin
    {
        public CurrencyChangedDelegate OnHeroLeveledUp { get; set; }
        
        private IDataManager dataManager;
        protected IValetPlugin valetPlugin;
     
        public DeckPlugin(IDataManager dataManager, IValetPlugin valetPlugin)
        {
            this.dataManager = dataManager;
            this.valetPlugin = valetPlugin;
        }

        public async UniTask<HeroDeck> GetHeroDeck() =>
            (await dataManager.GetAll<HeroDeck>()).FirstOrDefault();

        public async UniTask<HeroDefinition> GetHeroDefinition(string heroId) =>
            (await dataManager.GetAll<HeroDefinition>()).FirstOrDefault(x => x.UnitId == heroId);
        
        public async UniTask<bool> CanLevelUpHero(string heroId)
        {
            var heroDeck = await GetHeroDeck();
            var heroProgress = heroDeck.Heroes[heroId];
            var heroDefinition = await GetHeroDefinition(heroId);
            if (heroDefinition.IsMaxLevel(heroProgress.Level))
                return false;
            
            int cardsNeeded = heroDefinition.GetCardsNeededToLevelUp(heroProgress.Level);
            if (cardsNeeded > heroProgress.CardsCount)
                return false;
            var coinsHas = await valetPlugin.GetCurrency(Currency.Coins);
            int coinsNeeded = heroDefinition.GetCoinsNeededToLevelUp(heroProgress.Level);
            if (coinsNeeded > coinsHas)
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
        
        public async UniTask<(HeroProgress progress, HeroDefinition definition)> GetUnlockedHero(string heroId)
        {
            var heroDefinition = await GetHeroDefinition(heroId);
            var heroProgress = (await GetHeroDeck()).Heroes[heroId];
            return (heroProgress, heroDefinition);
        }
        
        public async UniTask<(bool, HeroProgress, HeroDefinition)> LevelUpHero(string heroId)
        {
            var heroDeck = await GetHeroDeck();
            var heroProgress = heroDeck.Heroes[heroId];
            var heroDefinition = await GetHeroDefinition(heroId);

            int cardsNeeded = heroDefinition.GetCardsNeededToLevelUp(heroProgress.Level);

            if (cardsNeeded > heroProgress.CardsCount)
            {
                Debug.Assert(false, "Not enough cards to level up");
                return (false,heroProgress, heroDefinition);
            }

            if (heroDefinition.IsMaxLevel(heroProgress.Level))
            {
                Debug.Assert(false, "Already maxed");
                return (false,heroProgress, heroDefinition);
            }

            int coinsNeeded = heroDefinition.GetCoinsNeededToLevelUp(heroProgress.Level);
            var coinsHas = await valetPlugin.GetCurrency(Currency.Coins);
            if (coinsNeeded > coinsHas)
            {
                Debug.Assert(false, "Not enough coins");
                return (false,heroProgress, heroDefinition);
            }

            heroProgress.Level++;
            heroProgress.CardsCount -= cardsNeeded;

            valetPlugin.SpendCurrency(Currency.Coins, coinsNeeded);
            await SaveHeroDeck(heroDeck);

            OnHeroLeveledUp?.Invoke(heroProgress, heroDefinition);
            
            return (true, heroProgress, heroDefinition);
        }
        
        public async UniTask SaveHeroDeck(HeroDeck heroDeck) =>
            await dataManager.Actualize<HeroDeck>(heroDeck);
    }
}