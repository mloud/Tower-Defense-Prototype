using System;
using TowerDefense.Data.Definitions;
using TowerDefensePrototype.Scripts.Data.Definitions.Editor;
using UnityEngine.UIElements;

namespace TowerDefense.Scripts.Data.Definitions.Editor.Heroes
{
    public class HeroUpgradeElement : CustomElement
    {
        private HeroLevelUpgradePath LevelUpgradePath { get; }
        private Action saveAction;
        public HeroUpgradeElement(HeroLevelUpgradePath levelUpgradePath)
        {
            LevelUpgradePath = levelUpgradePath;
        }
        
        protected override VisualElement CreateVisualElement()
        {
            var visualElement = new VisualElement();
            return visualElement;
        }

        protected override void OnCreate()
        {
            saveAction = null;
            
            var listElement = new ListElement<StatUpgrade>(LevelUpgradePath.StatsUpgrades, "", "Level");
            VisualElement.Add(listElement.Create().VisualElement);
            saveAction += ()=>listElement.Save();
        }

        protected override void OnSave()
        {
            saveAction?.Invoke();
        }
    }
}