using System;
using OneDay.Core.Modules.Ui.Components;
using TowerDefense.Battle.Events;
using UnityEngine;

namespace TowerDefense.Ui.Battle
{
    namespace CastlePrototype.Ui.Battle
    {
        public class BattleTimerController : MonoBehaviour
        {
            [SerializeField] private TimeSpanText timeSpanText;
            private void OnEnable() => BattleTimeChanged.Event.Subscribe(OnBattleTimeChanged);

            private void OnDisable() => BattleTimeChanged.Event.UnSubscribe(OnBattleTimeChanged);

            private void OnBattleTimeChanged(float time) => timeSpanText.Set(TimeSpan.FromSeconds(time));
        }
    }
}