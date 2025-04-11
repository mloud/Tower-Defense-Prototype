using System.Linq;
using CastlePrototype.Data;
using CastlePrototype.Data.Definitions;
using CastlePrototype.Managers;
using CastlePrototype.Ui.Panels;
using CastlePrototype.Ui.Views;
using Cysharp.Threading.Tasks;
using Meditation.States;
using OneDay.Core;
using OneDay.Core.Modules.Data;
using OneDay.Core.Modules.Sm;
using OneDay.Core.Modules.Ui;

namespace CastlePrototype.States
{
    public class LibraryState : AState
    {
        private LibraryView view;
        
        public override UniTask Initialize()
        {
            view = ServiceLocator.Get<IUiManager>().GetView<LibraryView>();
            view.OnLevelUp = heroId => LevelUpHero(heroId).Forget();
            return UniTask.CompletedTask;
        }
        
        public override async UniTask EnterAsync(StateData stateData = null)
        {
            ServiceLocator.Get<IUiManager>().GetPanel<MainButtonPanel>().Show(true);
            ServiceLocator.Get<IPlayerManager>().OnHeroLeveledUp += OnHeroLeveledUp;
            
            var heroDeck = await ServiceLocator.Get<IPlayerManager>().GetHeroDeck();
            var heroDefinitions = (await ServiceLocator.Get<IDataManager>().GetAll<HeroDefinition>()).ToList();
            await view.Initialize(heroDeck, heroDefinitions);
            
            view.Show(true);
        }

        private void OnHeroLeveledUp((HeroProgress progress, HeroDefinition definition) evt)
        {
            view.RefreshCard(evt.progress, evt.definition).Forget();
        }

        public override UniTask ExecuteAsync() => UniTask.CompletedTask;

        public override UniTask ExitAsync()
        {
            ServiceLocator.Get<IPlayerManager>().OnHeroLeveledUp -= OnHeroLeveledUp;
            view.Hide(true);
            return UniTask.CompletedTask;
        }

        private async UniTask LevelUpHero(string heroId)
        {
            var result = await ServiceLocator.Get<IPlayerManager>().LevelUpHero(heroId);
            // if (result.Item1)
            // {
            //     await view.RefreshCard(result.Item2, result.Item3);
            // }
        }
    }
}