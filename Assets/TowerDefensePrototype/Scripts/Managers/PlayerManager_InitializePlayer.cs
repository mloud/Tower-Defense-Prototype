using System;
using System.Collections.Generic;
using System.Linq;
using CastlePrototype.Data.Progress;
using Cysharp.Threading.Tasks;
using OneDay.Core;
using OneDay.Core.Debugging;
using OneDay.Core.Modules.Data;

namespace CastlePrototype.Managers
{
    public partial class PlayerManager
    {
         public async UniTask InitializePlayer()
        {
            await CreateProgressIfNeeded(()=>new PlayerProgress
            {
                Xp = 0,
                Level = 1
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
                    {"barricade", new HeroProgress
                    {
                        CardsCount = 0,
                        Level = 1
                    }}
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