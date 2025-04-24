using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using OneDay.Core;
using UnityEngine;

namespace TowerDefense.Managers
{
    public interface IBufferedEventsManager : IBufferedEvents
    { }
    public class BufferedEventsManager : MonoBehaviour, IBufferedEventsManager, IService
    {
        private BufferedEvents bufferedEvents;
        
        public UniTask Initialize()
        {
            Debug.Assert(bufferedEvents == null);
            bufferedEvents = new BufferedEvents();
            return UniTask.CompletedTask;
        }

        public UniTask PostInitialize() => UniTask.CompletedTask;
        
        public BufferedEvent PopFirst<T>(int type) where T : BufferedEvent => bufferedEvents.PopFirst<T>(type);
        public List<T> PopAll<T>(int type) where T : BufferedEvent => bufferedEvents.PopAll<T>(type);
        public void Push(BufferedEvent bufferedEvent) => bufferedEvents.Push(bufferedEvent);
    }
}