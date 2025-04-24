using TowerDefense.Battle.Visuals;
using UnityEngine;

namespace TowerDefensePrototype.Scripts.Capture
{
    public class VisualObjectNameProvider :AObjectNameProvider
    {
        public override string GetObjectName(GameObject go)
        {
            return go.GetComponent<VisualObject>().Id;
        }
    }
}