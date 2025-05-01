using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using OneDay.Core;
using OneDay.Core.Extensions;
using UnityEngine;

namespace TowerDefense.Managers.Simulation
{
    public class SimulationTaskLevelUp : ASimulationTask
    {
        private List<string> stringList = new();
        private List<string> leveledUpHeroes = new();
        public override async UniTask<T> Perform<T>(object input) where T: class
        {
            var playerManager = ServiceLocator.Get<IPlayerManager>();
            var heroesInDeck  =(await playerManager.DeckGetter.GetHeroDeck()).Heroes;
            var heroIds = heroesInDeck.Keys.ToList();
            
            leveledUpHeroes.Clear();
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
                Debug.Log($"XXX Hero {rndHero} leveled up {heroesInDeck[rndHero].Level - 1} -> {heroesInDeck[rndHero].Level}");
                leveledUpHeroes.Add(rndHero);
            }
            return leveledUpHeroes as T;
        }
    }
}