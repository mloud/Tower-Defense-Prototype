using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CastlePrototype.Battle.Visuals.Effects
{
    public abstract class BaseEffect : MonoBehaviour
    {
        [SerializeField] private float delay;
        public bool IsScreenSpace => isScreenSpace;
        [SerializeField] private bool isScreenSpace;
        public string Id => id;
        [SerializeField] private string id;

        public async UniTask Play(object data = null)
        {
            await UniTask.WaitForSeconds(delay);
            OnPlay(data);
        }
        
        protected abstract void OnPlay(object data = null);
    }
}