using System;
using UnityEngine;

namespace TowerDefensePrototype.Scripts.Capture
{
    [ExecuteInEditMode]
    public class CaptureStage : MonoBehaviour
    {
        public int CaptureWidth = 1000;
        public int CaptureHeight = 1000;
        public string CapturePath;


        [SerializeField] private AObjectNameProvider objectNameProvider;
        [SerializeField] private Camera camera;
        [SerializeField] private Transform container;
        [SerializeField] private float frameWidth;
        [SerializeField] private GameObject focusedGameObject;

        private void Update()
        {
            if (container == null)
                return;
            
            for (int i = 0; i < container.childCount; i++)
            {
                var child = container.GetChild(i);
                child.localPosition = new Vector3(i * frameWidth, 0, 0);
            }

            var index = GetFocusedIndex();
            if (index >= 0 && index < container.childCount)
            {
                focusedGameObject = container.GetChild(index).gameObject;
            }
        }

        public void FocusAtObject(int index)
        {
            if (index < 0 || index >= container.childCount)
                throw new ArgumentException("Object index out of range");

            var boxCollider = container.GetChild(index).GetComponent<BoxCollider>();
            if (boxCollider == null)
            {
                Debug.LogError("Object needs to have box collider to center properly");
            }
            
            var cameraTr = camera.transform;
            var position = cameraTr.position;
            position.x = container.position.x + index * frameWidth;
            position.y = boxCollider != null ? boxCollider.bounds.center.y : 0;
            cameraTr.position = position;
        }

        public int GetFocusedIndex()
        {
            var index = (int)Math.Round((camera.transform.position.x - container.position.x) / frameWidth);
            return index;
        }

        public void FocusAtNextObject()
        {
            int nextIndex = GetFocusedIndex() + 1;
            FocusAtObject(nextIndex >= container.childCount ? 0 : nextIndex);
        }
        public void FocusAtPrevObject()
        {
            int prevIndex = GetFocusedIndex() - 1;
            FocusAtObject(prevIndex < 0 ? container.childCount - 1 : prevIndex);
        }

        public string GetSelectedObjectName()
        {
            var index = GetFocusedIndex();
            if (index < 0 || index >= container.childCount)
                throw new ArgumentException("Object index out of range");

            return objectNameProvider.GetObjectName(container.GetChild(index).gameObject);
        }
    }
}