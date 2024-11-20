using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AverageFPS))]
public class AverageFPSEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        AverageFPS averageFPS = (AverageFPS)target;

        GUILayout.Space(10);

        GUI.enabled = Application.isPlaying;

        if (GUILayout.Button("Log Average FPS"))
            averageFPS.StartLoggingFPS();

        GUI.enabled = true;
    }
}