using UnityEngine;

namespace TowerDefensePrototype.Scripts.Capture
{
    public abstract class AObjectNameProvider : MonoBehaviour
    {
        public abstract string GetObjectName(GameObject go);
    }
}