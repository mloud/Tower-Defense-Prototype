using CastlePrototype.Data.Definitions;
using CastlePrototype.Data.Progress;
using CastlePrototype.Managers;
using Cysharp.Threading.Tasks;
using OneDay.Core;
using UnityEngine;

namespace CastlePrototype.Ui.Components
{
    public class CanLevelUpAnyHeroFlag : MonoBehaviour
    {
        [SerializeField] private GameObject flagGo;

        public void Initialize() => ServiceLocator.Get<IPlayerManager>().OnHeroLeveledUp += OnHeroLeveledUp;

        private void OnHeroLeveledUp((HeroProgress progress, HeroDefinition definition) evt) => Refresh().Forget();

        public async UniTask Refresh()
        {
           bool canLevelUp = await  ServiceLocator.Get<IPlayerManager>().CanLevelUpAnyHero();
           flagGo.SetActive(canLevelUp);
        }
    }
}