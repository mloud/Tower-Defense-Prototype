using System;
using TowerDefense.Data.Definitions;
using TowerDefense.Data.Definitions.CastlePrototype.Data.Definitions;
using TowerDefensePrototype.Data.Definitions.Stages.Generator.Editor;
using TowerDefensePrototype.Scripts.Data.Definitions.Editor;
using TowerDefensePrototype.Scripts.Data.Definitions.Editor.OneDay.Core.Modules.Data;
using UnityEditor;
using UnityEngine;


namespace TowerDefense.Scripts.Data.Definitions.Editor.Heroes
{
    public class StagesEditor : DefinitionWindow<StageDefinition, StageDefinitionsTable>
    {
        protected override string DefinitionName() => "StageDefinitionsTable";
        protected override Type DefinitionType() => typeof(HeroDefinitionsTable);
        
        [MenuItem("TD/new Stages Definition Editor")]
        public static void OpenWindow()
        {
            var wnd = GetWindow<StagesEditor>();
            wnd.titleContent = new GUIContent($"Stages Editor");
        }
        
        protected override CustomElement CreateTableElement(StageDefinition definition, int index)
        {
            var stageElement = new StageElement(definition, index);
            stageElement.OnGenerateButtonAction = OnGenerate;
            return stageElement.Create();
        }

        private void OnGenerate(StageGeneratorConfig config, StageElement element, StageDefinition stageDefinition, bool waveTimingOnly )
        {
            if (config == null)
            {
                EditorUtility.DisplayDialog("Generator config is null", "Set config first", "OK");
                return;
            }

            if (waveTimingOnly)
            {
                new StageGenerator(new UnitStats()).ModifyByWaveTimingCurve(config, stageDefinition);
            }
            else
            {
                new StageGenerator(new UnitStats()).Generate(config, stageDefinition);
            }
            element.Refresh(stageDefinition);
        }
    }
    class UnitStats : IUnitStats
    {
        public float GetUnitHp(string unitId)
        {
            var enemyDefinitionsTable = TableLoader.Load<EnemyDefinitionsTable>();
            return enemyDefinitionsTable.Data.Find(x => x.UnitId == unitId).GetLeveledHeroStat(StatUpgradeType.Hp, 1);
        }

        public float GetUnitDps(string unitId)
        {
            var enemyDefinitionsTable = TableLoader.Load<EnemyDefinitionsTable>();
            return enemyDefinitionsTable.Data.Find(x => x.UnitId == unitId).GetLeveledHeroStat(StatUpgradeType.Damage, 1);
        }
    }
}