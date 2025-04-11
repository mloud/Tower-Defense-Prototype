using System.Collections.Generic;
using System.Linq;
using CastlePrototype.Data;
using CastlePrototype.Data.Definitions;
using CastlePrototype.Ui.Components;
using OneDay.Core.Modules.Ui;
using UnityEngine;

namespace CastlePrototype.Ui.Views
{
    public class LibraryView : UiView
    {
        [SerializeField] private CardPanel cardPanel;
        public void Refresh(HeroDeck heroDeck, List<HeroDefinition> heroDefinitions)
        {
            cardPanel.Prepare(heroDeck.Heroes.Count);
            int index = 0;
            foreach (var hero in heroDeck.Heroes)
            {
                cardPanel.Get(index++)
                    .Set(hero.Value, heroDefinitions.First(x=>x.UnitId == hero.Key))
                    .gameObject.SetActive(true);
                
            }
        }
    }
}