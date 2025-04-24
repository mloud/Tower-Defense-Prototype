using System;
using Cysharp.Threading.Tasks;
using OneDay.Core.Modules.Ui.Components;
using TowerDefense.Data.Definitions;
using TowerDefense.Data.Progress;

namespace TowerDefense.Ui.Components
{
    public class StatsPanel : ContentPanel<StatWidget>
    {
        public async UniTask Initialize(HeroProgress heroProgress, HeroDefinition heroDefinition)
        {
            int statsCount = Enum.GetNames(typeof(StatUpgradeType)).Length;
            Prepare(statsCount);
            for (int i = 0; i < statsCount; i++)
            {
                var currentStat = heroDefinition.GetLeveledHeroStat((StatUpgradeType)i, heroProgress.Level);
                
                float? nextStat = heroDefinition.IsMaxLevel(heroProgress.Level) 
                    ? null : heroDefinition.GetLeveledHeroStat((StatUpgradeType)i, heroProgress.Level + 1);

                var statItem = Get(i);
                if (currentStat == 0 || nextStat == null || nextStat.Value == 0)
                {
                    statItem.gameObject.SetActive(false);
                }
                else
                {
                    if (nextStat.Value - currentStat == 0)
                        nextStat = null;
                    statItem.Set((StatUpgradeType)i, currentStat, nextStat);
                }
            }
        }
    }
}