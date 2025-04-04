using UnityEngine;
using UnityEngine.UI;

namespace CastlePrototype.Battle.Visuals
{
    public class ProgressBar : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private bool isInCanvasSpace;
        [SerializeField] private Vector3 screenOffset;
        
        private Transform originalParent;
        private Camera mainCamera;
        private void Awake()
        {
            mainCamera = Camera.main;
        }
        
        public void SetProgress(float progress01)
        {
            image.fillAmount = progress01;
        }

        public void PlaceToCanvas(Transform originalParent)
        {
            this.originalParent = originalParent;
            transform.SetParent(VisualManager.Default.UiPanel);
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }

        public void ReturnFromCanvas()
        {
            transform.SetParent(originalParent);
        }

        private void LateUpdate()
        {
            if (originalParent == null)
                return;
            
            var pos = mainCamera.WorldToScreenPoint(originalParent.position) + screenOffset;
            pos.z = 0;
            transform.position = pos;
        }
    }
}