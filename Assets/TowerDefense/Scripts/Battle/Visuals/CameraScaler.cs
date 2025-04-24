using UnityEngine;

namespace TowerDefense.Battle.Visuals
{
    public class CameraScaler : MonoBehaviour
    {
        [SerializeField] private float minScreenWidth;
        [SerializeField] private float defaultOrthoSize = 8;

        private Camera mainCamera;
        
        private void Awake()
        {
            mainCamera = Camera.main;
            float aspect = (float)Screen.width / Screen.height;
            float screenSizeInWorldUnits = aspect * 2 * mainCamera.orthographicSize;

            if (screenSizeInWorldUnits < minScreenWidth)
            {
                mainCamera.orthographicSize = minScreenWidth / (2 * aspect);
            }
            else
            {
                mainCamera.orthographicSize = defaultOrthoSize;
            }
        }
    }
}