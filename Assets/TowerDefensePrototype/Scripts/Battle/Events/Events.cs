using System;

namespace CastlePrototype.Battle.Events
{
    public class BattleEvent<T>
    {
        public event Action<T> OnEvent;

        public void Invoke(T value)
        {
            OnEvent?.Invoke(value);
        }
        
        public void Subscribe(Action<T> handler)
        {
            OnEvent -= handler;
            OnEvent += handler;
        }
        
        public void UnSubscribe(Action<T> handler)
        {
            OnEvent -= handler;
        }

        public void Clear() => OnEvent = null;
    }
    
    public class BattleEvent<T1, T2>
    {
        public event Action<T1, T2> OnEvent;

        public void Invoke(T1 value1, T2 value2)
        {
            OnEvent?.Invoke(value1, value2);
        }

        public void Subscribe(Action<T1, T2> handler)
        {
            OnEvent -= handler;
            OnEvent += handler;
        }
        
        public void UnSubscribe(Action<T1, T2> handler)
        {
            OnEvent -= handler;
        }
        
        public void Clear() => OnEvent = null;
    }
}