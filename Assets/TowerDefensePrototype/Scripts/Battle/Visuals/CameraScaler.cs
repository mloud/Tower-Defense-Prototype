using UnityEngine;

namespace CastlePrototype.Battle.Visuals
{
    public class CameraScaler : MonoBehaviour
    {
        [SerializeField] private float minScreenWidth;
        [SerializeField] private float defaultOrthoSize = 8;

        private void Awake()
        {
            float aspect = (float)Screen.width / Screen.height;
            float screenSizeInWorldUnits = aspect * 2 * Camera.main.orthographicSize;

            if (screenSizeInWorldUnits < minScreenWidth)
            {
                Camera.main.orthographicSize = minScreenWidth / (2 * aspect);
            }
            else
            {
                Camera.main.orthographicSize = defaultOrthoSize;
            }
        }
    }
}