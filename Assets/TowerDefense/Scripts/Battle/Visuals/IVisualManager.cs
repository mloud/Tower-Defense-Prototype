using System;
using OneDay.Core.Modules.Ui;
using TowerDefense.Battle.Visuals.Effects;
using UnityEngine;

namespace TowerDefense.Battle.Visuals
{
    public interface IVisualManager : IDisposable
    {
        Camera MainCamera { get; }
        VisualObject LoadEnvironment(string environmentId);
        int TrackVisualObject(VisualObject visualObject);
        void UnTrackVisualObject(VisualObject visualObject);
        VisualObject OnUnitCreated(string id);
        Vector3 GetObjectPosition(string id);
        VisualObject GetVisualObject(int index);
        void DestroyVisualObject(int index);
        BaseEffect PlayEffect(string effectId, Vector3 position, object data = null);
        void PauseVisualObjects();
        void SetBattleMusicPlaying(bool isPlaying);
        void ShowDamage(int visualIndex, float damage);
        Transform UiPanel { get; }
    }
}