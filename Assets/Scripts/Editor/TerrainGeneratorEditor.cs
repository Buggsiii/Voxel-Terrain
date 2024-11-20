using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TerrainGenerator))]
public class TerrainGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        TerrainGenerator terrainGenerator = (TerrainGenerator)target;

        GUILayout.Space(10);

        GUI.enabled = Application.isPlaying;

        if (GUILayout.Button("Generate Terrain"))
            terrainGenerator.GenerateTerrain();

        GUI.enabled = true;
    }
}