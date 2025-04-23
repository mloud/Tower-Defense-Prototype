using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CastlePrototype.Battle.Visuals
{
    public class TrailRendererCleaner : MonoBehaviour
    {
        [SerializeField] private TrailRenderer trailRenderer;
        private static WaitForSeconds _waitForSeconds = new WaitForSeconds(0.1f);
        private void OnEnable()
        {
            trailRenderer.Clear();
            trailRenderer.enabled = false;
            StartCoroutine(StartEmitting());
        }

        private IEnumerator StartEmitting()
        {
            yield return _waitForSeconds;
            trailRenderer.enabled = true;
            trailRenderer.Clear();
        }
    }
}