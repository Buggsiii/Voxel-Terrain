using UnityEngine;

public class UnoptimizedTerrain : Terrain
{
    public UnoptimizedTerrain(Transform parent, Material material, TerrainSettings terrainSettings) : base(parent, material, terrainSettings)
    { }

    public override void GenerateTerrain()
    {
        Vector3Int size = _terrainSettings.ChunkSize * _terrainSettings.ChunkCount;
        Vector3Int offset = new(size.x / 2, size.y / 2, size.z / 2);

        Transform unoptimizedTerrain = new GameObject("UnoptimizedTerrain").transform;
        unoptimizedTerrain.SetParent(_parent);

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                for (int z = 0; z < size.z; z++)
                {
                    Vector3Int pos = new Vector3Int(x, y, z) - offset;
                    if (!IsVoxelAt(pos.x, pos.y, pos.z))
                        continue;

                    Mesh mesh = new();
                    Vector3[] vertices = new Vector3[24] {
                        // top      (+y)
                        new(0.0f, 1.0f, 0.0f),
                        new(1.0f, 1.0f, 0.0f),
                        new(1.0f, 1.0f, 1.0f),
                        new(0.0f, 1.0f, 1.0f),
                        // bottom   (-y)
                        new(0.0f, 0.0f, 0.0f),
                        new(1.0f, 0.0f, 0.0f),
                        new(1.0f, 0.0f, 1.0f),
                        new(0.0f, 0.0f, 1.0f),
                        // right    (+x)
                        new(1.0f, 0.0f, 0.0f),
                        new(1.0f, 0.0f, 1.0f),
                        new(1.0f, 1.0f, 1.0f),
                        new(1.0f, 1.0f, 0.0f),
                        // left     (-x)
                        new(0.0f, 0.0f, 0.0f),
                        new(0.0f, 0.0f, 1.0f),
                        new(0.0f, 1.0f, 1.0f),
                        new(0.0f, 1.0f, 0.0f),
                        // back     (+z)
                        new(0.0f, 0.0f, 1.0f),
                        new(0.0f, 1.0f, 1.0f),
                        new(1.0f, 1.0f, 1.0f),
                        new(1.0f, 0.0f, 1.0f),
                        // forward  (-z)
                        new(0.0f, 0.0f, 0.0f),
                        new(0.0f, 1.0f, 0.0f),
                        new(1.0f, 1.0f, 0.0f),
                        new(1.0f, 0.0f, 0.0f),
                    };

                    int[] triangles = new int[36] {
                        0, 3, 1, 1, 3, 2, // top (+y)
                        4, 5, 7, 5, 6, 7, // bottom (-y)
                        8, 11, 9, 9, 11, 10, // right (+x)
                        12, 13, 15, 13, 14, 15, // left (-x)
                        16, 19, 17, 17, 19, 18, // back (+z)
                        20, 21, 23, 21, 22, 23, // forward (-z)
                    };

                    mesh.vertices = vertices;
                    mesh.triangles = triangles;
                    mesh.RecalculateNormals();

                    GameObject cube = new($"Cube ({x}, {y}, {z})");
                    cube.transform.SetParent(unoptimizedTerrain);
                    cube.transform.localPosition = pos;
                    cube.AddComponent<MeshFilter>().mesh = mesh;
                    cube.AddComponent<MeshRenderer>().material = _material;
                }
            }
        }
    }
}
