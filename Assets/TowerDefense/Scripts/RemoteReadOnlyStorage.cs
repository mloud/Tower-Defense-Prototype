using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Firebase.RemoteConfig;
using Newtonsoft.Json;
using OneDay.Core.Debugging;
using UnityEngine;


namespace OneDay.Core.Modules.Data
{
    public class RemoteReadOnlyStorage : IStorage
    {
        private Dictionary<Type, string> typeToKeyBindings = new();
        private Dictionary<string, object> Cache { get; } = new();

        private bool DevelopMode { get; }
        public RemoteReadOnlyStorage(bool developMode)
        {
            DevelopMode = developMode;
        }
        
        public void RegisterTypeToKeyBinding<T>(string key)
        {
            typeToKeyBindings.Add(typeof(T), key);
        }

        public UniTask<int> Add<T>(T data) where T : IDataObject =>
            throw new NotSupportedException("Saving to scriptable object storage is not allowed");

        public UniTask<bool> Actualize<T>(T data) where T : IDataObject =>
            throw new NotSupportedException("Actualize to scriptable object storage is not allowed");

        public async UniTask<T> Get<T>(int id) where T : IDataObject
        {
            var storageContent = await LoadStorage<T>();
            return storageContent.Data.Find(x => x.Id == id);
        }

        public async UniTask<IEnumerable<T>> GetAll<T>() where T : IDataObject
        {
            var storageContent = await LoadStorage<T>();
            return storageContent.Data;
        }

        public UniTask Remove<T>(int id) where T : IDataObject =>
            throw new NotSupportedException("Remove from scriptable object storage is not allowed");

        public UniTask RemoveAll<T>() =>
            throw new NotSupportedException("Remove all from scriptable object storage is not allowed");


        private string GetStorageNameForType<T>() => typeToKeyBindings[typeof(T)];

        private async UniTask<SerializableObjectTable<T>> LoadStorage<T>() where T : IDataObject
        {
            var storageName = GetStorageNameForType<T>();

            if (Cache.TryGetValue(storageName, out var value))
            {
                return (SerializableObjectTable<T>)value;
            }
            
            var result = await Fetch<T>(storageName);
            Cache.Add(storageName, result);
            return result;
        }
        
        private async UniTask<SerializableObjectTable<T>> Fetch<T>(string key) where T: IDataObject
        {
            try
            {
                var fetchTask = DevelopMode
                    ? FirebaseRemoteConfig.DefaultInstance.FetchAsync(TimeSpan.Zero)
                    : FirebaseRemoteConfig.DefaultInstance.FetchAsync();
                await fetchTask;
               
                bool activated = await FirebaseRemoteConfig.DefaultInstance.ActivateAsync();
                if (!activated)
                {
                    Debug.LogError("Could not activate FireBase remote config");
                }
               
                if (!fetchTask.IsCompletedSuccessfully)
                {
                    D.LogError($"Fetch failed for {key}", this);
                    return default;
                }

                var json = FirebaseRemoteConfig.DefaultInstance.GetValue(key).StringValue;

                if (!string.IsNullOrEmpty(json))
                {
                    return JsonConvert.DeserializeObject<SerializableObjectTable<T>>(json);
                }

                D.LogError($"Remote Config value for {key} is null or empty", this);
            }
            catch (Exception e)
            {
                D.LogError($"Exception during Remote Config fetch for {key}: {e}", this);
            }

            return default;
        }

        public class SerializableObjectTable<T>:  ITable<T> where T: IDataObject
        {
            public List<T> Data { get; set; }
        }
    }
}