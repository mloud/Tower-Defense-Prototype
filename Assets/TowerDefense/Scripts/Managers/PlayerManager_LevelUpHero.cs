using TowerDefense.Data;
using Cysharp.Threading.Tasks;
using TowerDefense.Data.Definitions;
using TowerDefense.Data.Progress;
using UnityEngine;

namespace TowerDefense.Managers
{
    public partial class PlayerManager
    {
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

            heroProgress.Level++;
            heroProgress.CardsCount -= cardsNeeded;

            await SaveHeroDeck(heroDeck);

            OnHeroLeveledUp?.Invoke((heroProgress, heroDefinition));
            
            return (true, heroProgress, heroDefinition);
        }
    }
}