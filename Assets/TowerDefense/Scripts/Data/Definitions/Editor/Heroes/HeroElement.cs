using System;
using System.Drawing;
using TowerDefense.Data.Definitions;
using TowerDefensePrototype.Scripts.Data.Definitions.Editor;
using UnityEngine.UIElements;
using FontStyle = UnityEngine.FontStyle;

namespace TowerDefense.Scripts.Data.Definitions.Editor.Heroes
{
    public class HeroElement : CustomElement
    {
        private HeroDefinition HeroDefinition  { get; }
        public HeroElement(HeroDefinition definition) => HeroDefinition = definition;
        private Action saveAction;
        protected override VisualElement CreateVisualElement()
        {
            var visualElement = VisualElementFactory.CreateScrollView(false, 250);
            return visualElement;
        }
        
        protected override void OnCreate()
        {
            VisualElement.Clear();
            saveAction = null;
            // name
            VisualElement.Add(VisualElementFactory.CreateLabel(HeroDefinition.UnitId, 15, UnityEngine.Color.red, FontStyle.Bold));
            VisualElement.Add(VisualElementFactory.CreateSeparator(2));

            // basic data
            VisualElement.Add(VisualElementFactory.CreateLabel("Stats", 15, UnityEngine.Color.yellow, FontStyle.Bold));
            saveAction += VisualElementFromClass.FillVisualElement(HeroDefinition, VisualElement);
            VisualElement.Add(VisualElementFactory.CreateSeparator(3));
            VisualElement.Add(VisualElementFactory.CreateLabel("Upgrades", 15, UnityEngine.Color.yellow, FontStyle.Bold));
            // upgrade

            var heroUpgradeElement = new HeroUpgradeElement(HeroDefinition.UpgradePath);
            VisualElement.Add(heroUpgradeElement.Create().VisualElement);
            saveAction += () => heroUpgradeElement.Save();
        }

        protected override void OnSave()
        {
            saveAction?.Invoke();
        }

        private void Refresh()
        {
            VisualElement.Clear();
            OnCreate();
        }
    }
}