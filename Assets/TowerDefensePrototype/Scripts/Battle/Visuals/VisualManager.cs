using System;
using System.Collections.Generic;
using System.Linq;
using CastlePrototype.Battle.Visuals.Effects;
using OneDay.Core.Extensions;
using UnityEngine;
using Object = UnityEngine.Object;


namespace CastlePrototype.Battle.Visuals
{
    public class VisualManager : IDisposable
    {
        private IVisualFactory VisualFactory { get; }
        private IEffectFactory EffectFactory { get; }
        private Dictionary<int, VisualObject> VisualObjectsByIndex { get; }
        private Dictionary<string, List<VisualObject>> VisualObjectsById { get; }
        public Transform UiPanel { get; private set; }
        
        private int visualCounter;
        public static VisualManager Default { get; private set; }
        
        public VisualManager(IVisualFactory visualFactory, IEffectFactory effectFactory, Transform uiPanel)
        {
            visualCounter = -1;
            Default = this;
            VisualFactory = visualFactory;
            EffectFactory = effectFactory;
            VisualObjectsByIndex = new Dictionary<int, VisualObject>();
            VisualObjectsById = new Dictionary<string, List<VisualObject>>();
            UiPanel = uiPanel;
        }
        
        public VisualObject LoadEnvironment(string environmentId) => 
            VisualFactory.Create<VisualObject>(environmentId);
     
        public void Dispose()
        {
            VisualObjectsByIndex.Values.ForEach(x => Object.Destroy(x.gameObject));
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
            visualObject.Initialize();
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
            var effectFound = visualObject.PlayEffect("Die");
            visualObject.Destroy(effectFound ? 2.0f : 0.0f);
        }

        public void PlayEffect(string effectId, Vector3 position)
        {
            var effect = EffectFactory.Create<BaseEffect>(effectId);
            effect.transform.position = position + new Vector3(0, 0.3f, 0);
            effect.Play();
        }
    }
}