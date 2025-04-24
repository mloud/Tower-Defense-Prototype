using TMPro;
using TowerDefense.Battle.Events;
using UnityEngine;

namespace TowerDefense.Ui.Battle
{
    public class WaveCounterController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI currentWave;
        [SerializeField] private TextMeshProUGUI totalWaves;
        
        private void OnEnable() => WaveCounterChanged.Event.Subscribe(OnWaveChanged);

        private void OnDisable() => WaveCounterChanged.Event.UnSubscribe(OnWaveChanged);

        private void OnWaveChanged(int current, int total)
        {
            currentWave.text = $"{current + 1}".ToString();
            totalWaves.text = total.ToString();
        }
    }
}