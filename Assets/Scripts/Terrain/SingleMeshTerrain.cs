using System.Collections.Generic;
using UnityEngine;

public class SingleMeshTerrain : Terrain
{
    public SingleMeshTerrain(Transform parent, Material material, TerrainSettings terrainSettings) : base(parent, material, terrainSettings)
    { }

    public override void GenerateTerrain()
    {
        Vector3Int size = _terrainSettings.ChunkSize * _terrainSettings.ChunkCount;
        Vector3Int offset = new(size.x / 2, size.y / 2, size.z / 2);
        Mesh mesh = new();
        List<Vector3> vertices = new();
        List<int> triangles = new();

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                for (int z = 0; z < size.z; z++)
                {
                    Vector3Int pos = new Vector3Int(x, y, z) - offset;
                    if (!IsVoxelAt(pos.x, pos.y, pos.z))
                        continue;

                    vertices.AddRange(new Vector3[]
                    {
                        // top      (+y)
                        new(pos.x, pos.y + 1, pos.z),
                        new(pos.x + 1, pos.y + 1, pos.z),
                        new(pos.x + 1, pos.y + 1, pos.z + 1),
                        new(pos.x, pos.y + 1, pos.z + 1),
                        // bottom   (-y)
                        new(pos.x, pos.y, pos.z),
                        new(pos.x + 1, pos.y, pos.z),
                        new(pos.x + 1, pos.y, pos.z + 1),
                        new(pos.x, pos.y, pos.z + 1),
                        // right    (+x)
                        new(pos.x + 1, pos.y, pos.z),
                        new(pos.x + 1, pos.y, pos.z+1),
                        new(pos.x + 1, pos.y + 1, pos.z+1),
                        new(pos.x + 1, pos.y + 1, pos.z),
                        // left     (-x)
                        new(pos.x, pos.y, pos.z),
                        new(pos.x, pos.y, pos.z + 1),
                        new(pos.x, pos.y + 1, pos.z + 1),
                        new(pos.x, pos.y + 1, pos.z),
                        // back     (+z)
                        new(pos.x, pos.y, pos.z + 1),
                        new(pos.x, pos.y + 1, pos.z + 1),
                        new(pos.x + 1, pos.y + 1, pos.z + 1),
                        new(pos.x + 1, pos.y, pos.z + 1),
                        // forward  (-z)
                        new(pos.x, pos.y, pos.z),
                        new(pos.x, pos.y + 1, pos.z),
                        new(pos.x + 1, pos.y + 1, pos.z),
                        new(pos.x + 1, pos.y, pos.z)
                    });

                    int i = vertices.Count;

                    triangles.AddRange(new int[]
                    {
                        i - 24, i - 21, i - 23, i - 23, i - 21, i - 22, // top (+y)
                        i - 20, i - 19, i - 17, i - 19, i - 18, i - 17, // bottom (-y)
                        i - 16, i - 13, i - 15, i - 15, i - 13, i - 14, // right (+x)
                        i - 12, i - 11, i - 9, i - 11, i - 10, i - 9, // left (-x)
                        i - 8, i - 5, i - 7, i - 7, i - 5, i - 6, // back (+z)
                        i - 4, i - 3, i - 1, i - 3, i - 2, i - 1, // forward (-z)
                    });
                }
            }
        }

        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();

        GameObject cube = new("SingleMeshTerrain");
        cube.transform.SetParent(_parent);
        cube.AddComponent<MeshFilter>().mesh = mesh;
        cube.AddComponent<MeshRenderer>().material = _material;
    }
}
