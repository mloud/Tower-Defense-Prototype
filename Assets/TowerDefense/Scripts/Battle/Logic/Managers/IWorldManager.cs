using System;
using Cysharp.Threading.Tasks;
using Unity.Entities;

namespace TowerDefense.Battle.Logic.Managers
{
    public interface IWorldManager : IDisposable
    {
        UniTask Initialize();
    }

    public abstract class WorldManager : IWorldManager
    {
        protected World AttachedToWorld { get; private set; }
        protected WorldManager(World world) => AttachedToWorld = world;
        public async UniTask Initialize() => await OnInitialize();
        public void Dispose() => OnRelease();

        protected abstract void OnRelease();
        protected virtual UniTask OnInitialize() => UniTask.CompletedTask;

    }
}