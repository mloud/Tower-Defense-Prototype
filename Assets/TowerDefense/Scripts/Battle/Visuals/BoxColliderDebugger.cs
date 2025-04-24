namespace TowerDefense.Battle.Visuals
{
    using UnityEngine;

    [ExecuteAlways]
    public class BoxColliderDebugger : MonoBehaviour
    {
        public Color debugColor = Color.green;

        private void OnDrawGizmos()
        {
            var boxCollider = GetComponent<BoxCollider>();
            if (boxCollider == null) return;

            Gizmos.color = debugColor;

            // Match BoxCollider transform
            var cubeTransform = Matrix4x4.TRS(
                boxCollider.transform.position,
                boxCollider.transform.rotation,
                boxCollider.transform.lossyScale
            );

            Gizmos.matrix = cubeTransform;
            Gizmos.DrawWireCube(boxCollider.center, boxCollider.size);

            // Reset matrix
            Gizmos.matrix = Matrix4x4.identity;
        }
    }
}