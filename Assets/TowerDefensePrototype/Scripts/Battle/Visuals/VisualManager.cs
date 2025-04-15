using System;
using System.Collections.Generic;
using System.Linq;
using CastlePrototype.Battle.Visuals.Effects;
using Cysharp.Threading.Tasks;
using OneDay.Core;
using OneDay.Core.Extensions;
using OneDay.Core.Modules.Audio;
using UnityEngine;


namespace CastlePrototype.Battle.Visuals
{
    public class VisualManager : IDisposable
    {
        public Camera MainCamera;
        
        private IVisualFactory VisualFactory { get; }
        private IEffectFactory EffectFactory { get; }
        private Dictionary<int, VisualObject> VisualObjectsByIndex { get; }
        private Dictionary<string, List<VisualObject>> VisualObjectsById { get; }
        public Transform UiPanel { get; private set; }
        
        public static VisualManager Default { get; private set; }
        
        private int visualCounter;
        private Camera mainCamera;
        
        public VisualManager(IVisualFactory visualFactory, IEffectFactory effectFactory, Transform uiPanel)
        {
            MainCamera = Camera.main;
            visualCounter = -1;
            Default = this;
            VisualFactory = visualFactory;
            EffectFactory = effectFactory;
            VisualObjectsByIndex = new Dictionary<int, VisualObject>();
            VisualObjectsById = new Dictionary<string, List<VisualObject>>();
            UiPanel = uiPanel;
            mainCamera = Camera.main;
        }
        
        public VisualObject LoadEnvironment(string environmentId) => 
            VisualFactory.Create<VisualObject>(environmentId);
     
        public void Dispose()
        {
            foreach (var key in VisualObjectsByIndex.Keys.ToList())
            {
                VisualObjectsByIndex[key].Die( true, ()=>VisualFactory.Release(VisualObjectsByIndex[key]));
            }
            VisualObjectsByIndex.Clear();
            VisualObjectsById.Clear();
            Default = null;
        }

        public int TrackVisualObject(VisualObject visualObject)
        {
            visualCounter++;
            VisualObjectsByIndex.Add(visualCounter, visualObject);

            if (VisualObjectsById.TryGetValue(visualObject.Id, out var list))
            {
                list.Add(visualObject);
            }
            else
            {
                list = new List<VisualObject> { visualObject };
                VisualObjectsById.Add(visualObject.Id, list);
            }
            
            return visualCounter;
        }

        public void UnTrackVisualObject(VisualObject visualObject)
        {
            VisualObjectsByIndex.Remove(visualObject.Index);
            if (VisualObjectsById.TryGetValue(visualObject.Id, out var list))
            {
                list.Remove(visualObject);
            }
        }

        public VisualObject OnUnitCreated(string id)
        {
            var visualObject = VisualFactory.Create<VisualObject>(id);
            return visualObject;
        }

        public Vector3 GetObjectPosition(string id)
        {
            var visualObject = VisualObjectsById[id].First(x => x.Id == id);
            Debug.Assert(visualObject != null, $"No such visual object found id:{id}");
            return visualObject.transform.position;
        }

        public VisualObject GetVisualObject(int index) => VisualObjectsByIndex[index];

        public void DestroyVisualObject(int index)
        {
            var visualObject = GetVisualObject(index);
            visualObject.Die(false,()=>VisualFactory.Release(visualObject));
        }

        public BaseEffect PlayEffect(string effectId, Vector3 position, object data = null)
        {
            var effect = EffectFactory.Create<BaseEffect>(effectId);

            if (effect.IsScreenSpace)
            {
                effect.transform.SetParent(Default.UiPanel);
                var screenPosition = mainCamera.WorldToScreenPoint(position);
                screenPosition.z = 0;
                effect.transform.position = screenPosition;
            }
            else
            {
                effect.transform.position = position + new Vector3(0, 0.3f, 0);
            }

            effect.OnFinishedAction = ()=>EffectFactory.Release(effect);
            effect.Play(data).Forget();
            return effect;
        }


        public void PauseVisualObjects()
        {
            
            foreach (var visualObject in VisualObjectsByIndex.Values.ToList())
            {
                visualObject.SetMoveSpeed(0);
            }
        }
        public void SetBattleMusicPlaying(bool isPlaying)
        {
            if (isPlaying)
            {
                ServiceLocator.Get<IAudioManager>().PlayMusic("BattleMusic");
            }
            else
            {
                ServiceLocator.Get<IAudioManager>().StopMusic();
            }
        }
    }
}