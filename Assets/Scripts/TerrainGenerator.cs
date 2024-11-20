using System;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    public enum TerrainType
    {
        Unoptimized,
        SingleMesh,
        Culled,
        Chunked,
    }

    [Space]
    [SerializeField] private TerrainType terrainType = TerrainType.Unoptimized;
    [SerializeField] private TerrainSettings terrainSettings;
    [SerializeField] private Material _material;

    private float _timeToGenerate;
    private CameraController _cameraController;

    public TerrainSettings TerrainSettings => terrainSettings;

    private void Start()
    {
        _cameraController = Camera.main.GetComponent<CameraController>();
        GenerateTerrain();
    }

    public void GenerateTerrain()
    {
        ClearTerrain();

        Terrain terrain = terrainType switch
        {
            TerrainType.Unoptimized => new UnoptimizedTerrain(transform, _material, terrainSettings),
            TerrainType.SingleMesh => new SingleMeshTerrain(transform, _material, terrainSettings),
            TerrainType.Culled => new CulledTerrain(transform, _material, terrainSettings),
            TerrainType.Chunked => new ChunkedTerrain(transform, _material, terrainSettings),
            _ => throw new ArgumentOutOfRangeException()
        };

        var watch = new System.Diagnostics.Stopwatch();
        watch.Start();

        terrain.GenerateTerrain();

        watch.Stop();
        _timeToGenerate = watch.ElapsedMilliseconds;
        Debug.Log($"Time to generate terrain: {_timeToGenerate}ms");

        Bounds terrainBounds = CalculateTerrainBounds();
        _cameraController.SetPosition(terrainBounds);
    }

    private Bounds CalculateTerrainBounds()
    {
        Bounds bounds = new(Vector3.zero, Vector3.zero);
        foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
        {
            bounds.Encapsulate(renderer.bounds);
        }
        return bounds;
    }

    private void ClearTerrain()
    {
        foreach (Transform child in transform)
            Destroy(child.gameObject);
    }

#if UNITY_EDITOR
    private void OnGUI()
    {
        GUI.Box(new Rect(0, 0, 220, 300), "");

        GUI.Label(new Rect(10, 100, 200, 20), $"Time to generate terrain: {_timeToGenerate}ms");

        terrainType = (TerrainType)GUI.SelectionGrid(new Rect(10, 120, 200, 60), (int)terrainType, Enum.GetNames(typeof(TerrainType)), 2);

        string chunkCountXString = terrainSettings.ChunkCount.x.ToString();
        string chunkCountYString = terrainSettings.ChunkCount.y.ToString();
        string chunkCountZString = terrainSettings.ChunkCount.z.ToString();

        chunkCountXString = GUI.TextField(new Rect(10, 185, 60, 20), chunkCountXString);
        chunkCountYString = GUI.TextField(new Rect(70, 185, 60, 20), chunkCountYString);
        chunkCountZString = GUI.TextField(new Rect(140, 185, 60, 20), chunkCountZString);

        if (chunkCountXString == "")
            chunkCountXString = "0";
        if (chunkCountYString == "")
            chunkCountYString = "0";
        if (chunkCountZString == "")
            chunkCountZString = "0";

        if (int.TryParse(chunkCountXString, out int chunkCountX) &&
            int.TryParse(chunkCountYString, out int chunkCountY) &&
            int.TryParse(chunkCountZString, out int chunkCountZ))
        {
            terrainSettings.ChunkCount = new Vector3Int(chunkCountX, chunkCountY, chunkCountZ);
        }
        if (GUI.Button(new Rect(10, 210, 200, 40), "Generate Terrain"))
            GenerateTerrain();
    }
#endif

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Vector3 chunkSize = terrainSettings.ChunkSize;
        float chunkRadius = Mathf.Sqrt(chunkSize.x * chunkSize.x + chunkSize.y * chunkSize.y + chunkSize.z * chunkSize.z) / 2f;

        foreach (Transform child in transform)
        {
            Vector3 center = child.position + terrainSettings.ChunkSize / 2;
            Gizmos.DrawWireSphere(center, chunkRadius);
        }
    }
}

[Serializable]
public struct TerrainSettings
{
    [Range(0, 1)]
    public float Density;
    public float NoiseScale;
    public Vector3Int ChunkSize;
    public Vector3Int ChunkCount;
}