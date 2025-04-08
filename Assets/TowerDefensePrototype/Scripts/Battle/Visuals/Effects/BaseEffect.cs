using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CastlePrototype.Battle.Visuals.Effects
{
    public abstract class BaseEffect : MonoBehaviour
    {
        public string Id => id;
        public bool IsScreenSpace => isScreenSpace;

        public Action OnFinishedAction;
        
        [SerializeField] private float delay;
        [SerializeField] private string id;
        [SerializeField] private bool isScreenSpace;
        [SerializeField] protected bool destroyAfterFinished;
        [SerializeField] private bool playOnEnable;

        private void OnEnable()
        {
            if (playOnEnable)
            {
                Play().Forget();
            }
        }

        public async UniTask Play(object data = null)
        {
            if (delay > 0)
            {
                await UniTask.WaitForSeconds(delay);
            }

            OnPlay(data);
        }
        
        protected abstract void OnPlay(object data = null);
    }
}