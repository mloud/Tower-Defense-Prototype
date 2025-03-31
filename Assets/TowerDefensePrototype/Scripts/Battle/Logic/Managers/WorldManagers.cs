using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using OneDay.Core.Extensions;
using Unity.Entities;
using UnityEngine;

namespace CastlePrototype.Battle.Logic.Managers
{
    public static class WorldManagers
    {
        private static Dictionary<World, Dictionary<Type, IWorldManager>> Worlds = new();

        private static World defaultWord;
        private static Dictionary<Type, IWorldManager> defaultWorldManagers;

        public static World DefaultWorld
        {
            get => defaultWord;
            set
            {
                Debug.Assert(value != null);
                if (!Worlds.ContainsKey(value))
                {
                    Worlds.Add(value, new Dictionary<Type, IWorldManager>());
                }
                defaultWorldManagers = Worlds[value];
            }
        }

        public static async UniTask Initialize(World world)
        {
            foreach (var worldManager in Worlds[world].Values)
            {
                await worldManager.Initialize();
            }
        }
        
        public static void Register<T>(World world, T manager) where T: IWorldManager
        {
            if (!Worlds.ContainsKey(world))
            {
                Worlds.Add(world, new Dictionary<Type, IWorldManager>());   
            }
            
            Worlds[world].Add(typeof(T), manager);
        }

        public static T Get<T>()
        {
            Debug.Assert(defaultWord != null, "Assign default world first");
            defaultWorldManagers.TryGetValue(typeof(T), out var manager);
            return (T)manager;
        }
        public static T Get<T>(World world)
        {
            if (!Worlds.ContainsKey(world))
                return default;

            Worlds[world].TryGetValue(typeof(T), out var manager);
            return (T)manager;
        }

        public static void Clear(World world)
        {
            Worlds[world].Values.ForEach(x => x.Dispose());
            Worlds.Remove(world);
        }
    }
}