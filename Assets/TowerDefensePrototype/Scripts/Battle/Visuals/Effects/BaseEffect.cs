using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
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
                StartCoroutine(Play());
            }
        }

        public IEnumerator Play(object data = null)
        {
            if (delay > 0)
            {
                yield return new WaitForSeconds(delay);
            }
         
            OnPlay(data);
        }
        
        protected abstract void OnPlay(object data = null);
    }
}