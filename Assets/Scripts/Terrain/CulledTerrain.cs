using System.Collections.Generic;
using UnityEngine;

public class CulledTerrain : Terrain
{
    public CulledTerrain(Transform parent, Material material, TerrainSettings terrainSettings) : base(parent, material, terrainSettings)
    { }

    public override void GenerateTerrain()
    {
        Vector3Int size = _terrainSettings.ChunkSize * _terrainSettings.ChunkCount;
        Vector3Int offset = new(size.x / 2, size.y / 2, size.z / 2);
        Mesh mesh = new();
        List<Vector3> vertices = new();
        List<int> triangles = new();
        int i = 0;

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                for (int z = 0; z < size.z; z++)
                {
                    Vector3Int pos = new Vector3Int(x, y, z) - offset;
                    if (!IsVoxelAt(pos.x, pos.y, pos.z))
                        continue;

                    bool isTop = IsVoxelAt(pos.x, pos.y + 1, pos.z);
                    bool isBottom = IsVoxelAt(pos.x, pos.y - 1, pos.z);
                    bool isRight = IsVoxelAt(pos.x + 1, pos.y, pos.z);
                    bool isLeft = IsVoxelAt(pos.x - 1, pos.y, pos.z);
                    bool isBack = IsVoxelAt(pos.x, pos.y, pos.z + 1);
                    bool isForward = IsVoxelAt(pos.x, pos.y, pos.z - 1);

                    if (!isTop)
                    {
                        vertices.AddRange(new Vector3[]
                        {
                            new(pos.x, pos.y + 1, pos.z),
                            new(pos.x + 1, pos.y + 1, pos.z),
                            new(pos.x + 1, pos.y + 1, pos.z + 1),
                            new(pos.x, pos.y + 1, pos.z + 1),
                        });

                        i += 4;

                        triangles.AddRange(new int[]
                        {
                            i - 1, i - 2, i - 3,
                            i - 1, i - 3, i - 4
                        });
                    }

                    if (!isBottom)
                    {
                        vertices.AddRange(new Vector3[]
                        {
                            new(pos.x, pos.y, pos.z),
                            new(pos.x + 1, pos.y, pos.z),
                            new(pos.x + 1, pos.y, pos.z + 1),
                            new(pos.x, pos.y, pos.z + 1),
                        });

                        i += 4;

                        triangles.AddRange(new int[]
                        {
                            i - 4, i - 3, i - 1, i - 3, i - 2, i - 1
                        });
                    }

                    if (!isRight)
                    {
                        vertices.AddRange(new Vector3[]
                        {
                            new(pos.x + 1, pos.y, pos.z),
                            new(pos.x + 1, pos.y, pos.z + 1),
                            new(pos.x + 1, pos.y + 1, pos.z + 1),
                            new(pos.x + 1, pos.y + 1, pos.z),
                        });

                        i += 4;

                        triangles.AddRange(new int[]
                        {
                            i - 1, i - 2, i - 3, i - 1, i - 3, i - 4
                        });
                    }

                    if (!isLeft)
                    {
                        vertices.AddRange(new Vector3[]
                        {
                            new(pos.x, pos.y, pos.z),
                            new(pos.x, pos.y, pos.z + 1),
                            new(pos.x, pos.y + 1, pos.z + 1),
                            new(pos.x, pos.y + 1, pos.z),
                        });

                        i += 4;

                        triangles.AddRange(new int[]
                        {
                            i - 4, i - 3, i - 1, i - 3, i - 2, i - 1
                        });
                    }

                    if (!isBack)
                    {
                        vertices.AddRange(new Vector3[]
                        {
                            new(pos.x, pos.y, pos.z + 1),
                            new(pos.x, pos.y + 1, pos.z + 1),
                            new(pos.x + 1, pos.y + 1, pos.z + 1),
                            new(pos.x + 1, pos.y, pos.z + 1),
                        });

                        i += 4;

                        triangles.AddRange(new int[]
                        {
                            i - 1, i - 2, i - 3, i - 1, i - 3, i - 4
                        });
                    }

                    if (!isForward)
                    {
                        vertices.AddRange(new Vector3[]
                        {
                            new(pos.x, pos.y, pos.z),
                            new(pos.x, pos.y + 1, pos.z),
                            new(pos.x + 1, pos.y + 1, pos.z),
                            new(pos.x + 1, pos.y, pos.z),
                        });

                        i += 4;

                        triangles.AddRange(new int[]
                        {
                            i - 4, i - 3, i - 1, i - 3, i - 2, i - 1
                        });
                    }
                }
            }
        }

        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();

        GameObject cube = new("CulledTerrain");
        cube.transform.SetParent(_parent);
        cube.AddComponent<MeshFilter>().mesh = mesh;
        cube.AddComponent<MeshRenderer>().material = _material;
    }
}
