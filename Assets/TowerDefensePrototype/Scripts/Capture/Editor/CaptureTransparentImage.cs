namespace TowerDefensePrototype.Scripts.Capture.Editor
{
#if UNITY_EDITOR
    using System.IO;
    using UnityEngine;

    namespace TowerDefensePrototype.Scripts.Capture
    {
        public static class CaptureTransparentImage 
        {
            public static void CaptureTransparentScreenshot(Camera camera, string path, int width = 1920, int height = 1080)
            {
                var rt = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
                var tex = new Texture2D(width, height, TextureFormat.RGBA32, false);

                var originalClearFlags = camera.clearFlags;
                var originalBackgroundColor = camera.backgroundColor;
                var originalTargetTexture = camera.targetTexture;

                camera.clearFlags = CameraClearFlags.SolidColor;
                camera.backgroundColor = new Color(0, 0, 0, 0); // transparent
                camera.targetTexture = rt;

                camera.Render();

                RenderTexture.active = rt;
                tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
                tex.Apply();

                camera.clearFlags = originalClearFlags;
                camera.backgroundColor = originalBackgroundColor;
                camera.targetTexture = originalTargetTexture;
                RenderTexture.active = null;

                byte[] bytes = tex.EncodeToPNG();
                File.WriteAllBytes(path, bytes);

                Debug.Log($"Saved screenshot to: {path}");

                Object.DestroyImmediate(rt);
                Object.DestroyImmediate(tex);
            }
        }
    }
#endif
}