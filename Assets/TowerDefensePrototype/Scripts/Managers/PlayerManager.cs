using System;
using System.Collections.Generic;
using System.Linq;
using CastlePrototype.Data;
using Cysharp.Threading.Tasks;
using OneDay.Core;
using OneDay.Core.Debugging;
using OneDay.Core.Modules.Data;
using UnityEngine;

namespace CastlePrototype.Managers
{
    public interface IPlayerManager
    {
        public UniTask InitializePlayer();
        public UniTask<IPlayerProgress> GetProgression();
        public UniTask<HeroDeck> GetHeroDeck();
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
        
        public async UniTask InitializePlayer()
        {
            await CreateProgressIfNeeded(()=>new Player
            {
                EquippedWeapon = "canon"
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