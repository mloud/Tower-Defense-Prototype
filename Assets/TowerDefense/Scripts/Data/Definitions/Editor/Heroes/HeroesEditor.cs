using System;
using TowerDefense.Data.Definitions;
using TowerDefensePrototype.Scripts.Data.Definitions.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace TowerDefense.Scripts.Data.Definitions.Editor.Heroes
{
    public class HeroesEditor : DefinitionWindow<HeroDefinition, HeroDefinitionsTable>
    {
        protected override string DefinitionName() => "HeroDefinitionsTable";
        protected override Type DefinitionType() => typeof(HeroDefinitionsTable);
        
        [MenuItem("TD/Heroes Definition Editor")]
        public static void OpenWindow()
        {
            var wnd = GetWindow<HeroesEditor>();
            wnd.titleContent = new GUIContent($"Heroes Editor");
        }
        
        protected override CustomElement CreateTableElement(HeroDefinition definition)
        {
            var element = new HeroElement(definition).Create();
            return element;
        }
    }
}