using System.IO;
using TowerDefensePrototype.Scripts.Capture.Editor.TowerDefensePrototype.Scripts.Capture;
using UnityEditor;
using UnityEngine;

namespace TowerDefensePrototype.Scripts.Capture.Editor
{
    [CustomEditor(typeof(CaptureStage))]
    public class CaptureStageEditor : UnityEditor.Editor
    {
        private int cameraIndex = 0;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var captureStage = (CaptureStage)target;

            GUILayout.Space(10);
            GUILayout.Label("Capture Settings", EditorStyles.boldLabel);
            
            
            GUILayout.Space(10);
            GUILayout.Label("Capture Tools", EditorStyles.boldLabel);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Set Camera to Object")) captureStage.FocusAtObject(cameraIndex);
            cameraIndex = EditorGUILayout.IntField("Object Index", cameraIndex);
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("<-")) captureStage.FocusAtPrevObject();
            if (GUILayout.Button("->")) captureStage.FocusAtNextObject();
            GUILayout.EndHorizontal();
            
            GUILayout.Space(10);
            if (GUILayout.Button("Capture")) CaptureTransparentImage
                .CaptureTransparentScreenshot(Camera.main, Path.Combine(captureStage.CapturePath, captureStage.GetSelectedObjectName()+".png"), captureStage.CaptureWidth, captureStage.CaptureHeight);
        }
    }
}