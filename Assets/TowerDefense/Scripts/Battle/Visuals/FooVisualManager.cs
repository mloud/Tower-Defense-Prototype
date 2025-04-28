using TowerDefense.Battle.Visuals.Effects;
using UnityEngine;

namespace TowerDefense.Battle.Visuals
{
    public class FooVisualManager : IVisualManager
    {
        public void Dispose()
        { }

        public Camera MainCamera => throw new System.NotImplementedException();

        public VisualObject LoadEnvironment(string environmentId)
        {
            return null;
        }

        public int TrackVisualObject(VisualObject visualObject)
        {
            return -1;
        }

        public void UnTrackVisualObject(VisualObject visualObject)
        { }

        public VisualObject OnUnitCreated(string id) => throw new System.NotImplementedException();

        public Vector3 GetObjectPosition(string id) => throw new System.NotImplementedException();

        public VisualObject GetVisualObject(int index) => throw new System.NotImplementedException();

        public void DestroyVisualObject(int index)
        {}

        public BaseEffect PlayEffect(string effectId, Vector3 position, object data = null) => null;
        
        public void PauseVisualObjects()
        { }

        public void SetBattleMusicPlaying(bool isPlaying)
        { }

        public void ShowDamage(int visualIndex, float damage)
        { }

        public Transform UiPanel => throw new System.NotImplementedException();
    }
}