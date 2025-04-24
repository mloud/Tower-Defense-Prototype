using System;
using System.Collections.Generic;
using System.Linq;
using CastlePrototype.Data.Progress;
using CastlePrototype.Data.Definitions;
using CastlePrototype.Ui.Components;
using Cysharp.Threading.Tasks;
using OneDay.Core.Modules.Ui;
using UnityEngine;

namespace CastlePrototype.Ui.Views
{
    public class LibraryView : UiView
    {
        public Action<string> OnLevelUp;
        
        [SerializeField] private CardPanel cardPanel;

        public async UniTask RefreshCard(HeroProgress heroProgress, HeroDefinition heroDefinition)
        {
            var card = cardPanel.Items.First(x => x.Id == heroDefinition.UnitId);
            await card.Set(heroProgress, heroDefinition);
            card.gameObject.SetActive(true);
            card.OnLevelUp = _=>OnLevelUp(heroDefinition.UnitId);
        }
        public async UniTask Initialize(HeroDeck heroDeck, List<HeroDefinition> heroDefinitions)
        {
            cardPanel.Prepare(heroDeck.Heroes.Count);
            int index = 0;
            foreach (var (heroId, heroProgress) in heroDeck.Heroes)
            {
                var card = cardPanel.Get(index++);
                await card.Set(heroProgress, heroDefinitions.First(x=>x.UnitId == heroId));
                card.gameObject.SetActive(true);
                card.OnLevelUp = _=>OnLevelUp(heroId);
            }
        }
    }
}