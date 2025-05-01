using Cysharp.Threading.Tasks;
using UnityEngine;

namespace TowerDefense.Managers.Simulation
{
    public abstract class ASimulation : MonoBehaviour
    {
        public async UniTask Run()
        {
            await OnRun();
        }

        public void ProcessBattleEnd(int stage, float battleProgress01, bool playerWon)
        {
            OnProcessBattleEnd(stage, battleProgress01, playerWon);
        }
        protected abstract UniTask OnRun();
        protected abstract void OnProcessBattleEnd(int stage, float battleProgress01, bool playerWon);
    }
}