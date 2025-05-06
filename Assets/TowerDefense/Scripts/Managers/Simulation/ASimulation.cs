using System.Collections.Generic;
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

        public void ProcessBattleEnd(int stage, float battleProgress01, bool playerWon, List<string> usedSkills)
        {
            OnProcessBattleEnd(stage, battleProgress01, playerWon, usedSkills);
        }
        protected abstract UniTask OnRun();
        protected abstract void OnProcessBattleEnd(int stage, float battleProgress01, bool playerWon, List<string> usedSkills);
    }
}