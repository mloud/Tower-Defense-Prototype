using System.Collections.Generic;
using System.Linq;
using CastlePrototype.Data;
using CastlePrototype.Data.Definitions;
using Cysharp.Threading.Tasks;

namespace CastlePrototype.Managers
{
    public partial class PlayerManager
    {
        public async UniTask<PlayerProgress> GetProgression()
            => (await dataManager.GetAll<PlayerProgress>()).FirstOrDefault();
        
        public async UniTask<(int xp, int xpNextLevel, int level)> GetProgressionInfo()
        {
            var progression = await GetProgression();
            var playerProgressionDef = await GetPlayerProgressionDefinition();
            return (progression.Xp, playerProgressionDef.XpNeededToNextLevel[progression.Level], progression.Level);
        }
        
        public async UniTask<HeroDeck> GetHeroDeck() =>
            (await dataManager.GetAll<HeroDeck>()).FirstOrDefault();

        public async UniTask<HeroDefinition> GetHeroDefinition(string heroId) =>
            (await dataManager.GetAll<HeroDefinition>()).FirstOrDefault(x => x.UnitId == heroId);

        public async UniTask<StageDefinition> GetStageDefinition(int stage) =>
            (await dataManager.GetAll<StageDefinition>()).ElementAt(stage);

        public async UniTask<PlayerProgressionDefinition> GetPlayerProgressionDefinition() =>
            (await dataManager.GetAll<PlayerProgressionDefinition>()).FirstOrDefault();

        public async UniTask<IEnumerable<StageDefinition>> GetAllStageDefinitions() =>
            await dataManager.GetAll<StageDefinition>();
   
        public async UniTask<(HeroProgress progress, HeroDefinition definition)> GetUnlockedHero(string heroId)
        {
            var heroDefinition = await GetHeroDefinition(heroId);
            var heroProgress = (await GetHeroDeck()).Heroes[heroId];
            return (heroProgress, heroDefinition);
        }

    }
}