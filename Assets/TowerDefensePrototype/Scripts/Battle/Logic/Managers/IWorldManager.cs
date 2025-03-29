using System;
using Unity.Entities;

namespace CastlePrototype.Battle.Logic.Managers
{
    public interface IWorldManager : IDisposable
    { }

    public abstract class WorldManager : IWorldManager
    {
        protected World AttachedToWorld { get; private set; }

        protected WorldManager(World world)
        {
            AttachedToWorld = world;
        }
        
        public void Dispose()
        {
            OnRelease();
        }

        protected abstract void OnRelease();
    }
}