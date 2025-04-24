using System.Collections.Generic;

namespace TowerDefense.Managers
{
    public interface IBufferedEvents
    {
        BufferedEvent PopFirst<T>(int type) where T : BufferedEvent;
        List<T> PopAll<T>(int type) where T : BufferedEvent;
        void Push(BufferedEvent bufferedEvent);

    }
    public class BufferedEvents : IBufferedEvents
    {
        private List<BufferedEvent> bufferedEventsList = new();

        public BufferedEvent PopFirst<T>(int type) where T: BufferedEvent
        {
            for (int i = 0; i < bufferedEventsList.Count; i++)
            {
                if (bufferedEventsList[i].Type == type)
                {
                    var evt = bufferedEventsList[i];
                    
                    bufferedEventsList.RemoveAt(i);
                    return (T)evt;
                }
            }

            return default;
        }

        public List<T> PopAll<T>(int type) where T: BufferedEvent
        {
            List<T> result = null;
            for (int i =  bufferedEventsList.Count - 1; i>= 0; i--)
            {
                if (bufferedEventsList[i].Type == type)
                {
                    result ??= new List<T>();
                    result.Add((T)bufferedEventsList[i]);
                    bufferedEventsList.RemoveAt(i);
                }
            }
            result?.Reverse();
            return result;
        }

        public void Push(BufferedEvent bufferedEvent)
        {
            bufferedEventsList.Add(bufferedEvent);
        }
    }
}