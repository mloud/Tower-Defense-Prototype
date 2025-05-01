using Cysharp.Threading.Tasks;
using UnityEngine;

namespace TowerDefense.Managers.Simulation
{
    public abstract class ASimulationTask
    {
        public abstract UniTask<T> Perform<T>(object input) where T : class;
    }
}