using UnityEngine;

namespace CastlePrototype.Battle.Visuals
{
    public class TrailRendererCleaner : MonoBehaviour
    {
        [SerializeField] private TrailRenderer trailRenderer;
        private void OnEnable()
        {
            trailRenderer.Clear();
            trailRenderer.enabled = true;
        }

        private void OnDisable()
        {
            trailRenderer.Clear();
            trailRenderer.enabled = false;
        }
    }
}