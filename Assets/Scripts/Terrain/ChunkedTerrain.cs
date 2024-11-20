using System.Collections.Generic;
using UnityEngine;

public class ChunkedTerrain : Terrain
{
    public ChunkedTerrain(Transform parent, Material material, TerrainSettings terrainSettings) : base(parent, material, terrainSettings)
    { }

    public override void GenerateTerrain()
    {
        for (int x = 0; x < _terrainSettings.ChunkCount.x; x++)
        {
            for (int y = 0; y < _terrainSettings.ChunkCount.y; y++)
            {
                for (int z = 0; z < _terrainSettings.ChunkCount.z; z++)
                {
                    Vector3Int index = new(x, y, z);
                    GenerateChunk(index);
                }
            }
        }
    }

    protected virtual void GenerateChunk(Vector3Int index)
    {
        Vector3Int size = _terrainSettings.ChunkSize * _terrainSettings.ChunkCount;
        Vector3Int offset = new(-size.x / 2, -size.y / 2, -size.z / 2);
        Mesh mesh = new();
        List<Vector3> vertices = new();
        List<int> triangles = new();
        int i = 0;

        for (int x = 0; x < _terrainSettings.ChunkSize.x; x++)
        {
            for (int y = 0; y < _terrainSettings.ChunkSize.y; y++)
            {
                for (int z = 0; z < _terrainSettings.ChunkSize.z; z++)
                {
                    Vector3Int pos = new(x, y, z);
                    Vector3Int worldPos = new(
                        pos.x + index.x * _terrainSettings.ChunkSize.x,
                        pos.y + index.y * _terrainSettings.ChunkSize.y,
                        pos.z + index.z * _terrainSettings.ChunkSize.z
                    );
                    Vector3Int perlinPos = new(
                        worldPos.x + offset.x,
                        worldPos.y + offset.y,
                        worldPos.z + offset.z
                    );
                    if (!IsVoxelAt(perlinPos.x, perlinPos.y, perlinPos.z))
                        continue;

                    bool isTop = IsVoxelAt(perlinPos.x, perlinPos.y + 1, perlinPos.z);
                    bool isBottom = IsVoxelAt(perlinPos.x, perlinPos.y - 1, perlinPos.z);
                    bool isRight = IsVoxelAt(perlinPos.x + 1, perlinPos.y, perlinPos.z);
                    bool isLeft = IsVoxelAt(perlinPos.x - 1, perlinPos.y, perlinPos.z);
                    bool isBack = IsVoxelAt(perlinPos.x, perlinPos.y, perlinPos.z + 1);
                    bool isForward = IsVoxelAt(perlinPos.x, perlinPos.y, perlinPos.z - 1);

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

        GameObject chunk = new("CulledTerrain");
        chunk.transform.SetParent(_parent);

        Vector4 cPos = new(
            0,
            0,
            0,
            1
        );

        Matrix4x4 translation = new(
            new Vector4(1, 0, 0, index.x * _terrainSettings.ChunkSize.x + offset.x),
            new Vector4(0, 1, 0, index.y * _terrainSettings.ChunkSize.y + offset.y),
            new Vector4(0, 0, 1, index.z * _terrainSettings.ChunkSize.z + offset.z),
            new Vector4(0, 0, 0, 1)
        );

        Vector4 translatedPosition = new(
            translation.m00 * cPos.x + translation.m10 * cPos.y + translation.m20 * cPos.z + translation.m30 * cPos.w,
            translation.m01 * cPos.x + translation.m11 * cPos.y + translation.m21 * cPos.z + translation.m31 * cPos.w,
            translation.m02 * cPos.x + translation.m12 * cPos.y + translation.m22 * cPos.z + translation.m32 * cPos.w,
            translation.m03 * cPos.x + translation.m13 * cPos.y + translation.m23 * cPos.z + translation.m33 * cPos.w
        );

        chunk.transform.position = translatedPosition;
        chunk.AddComponent<MeshFilter>().mesh = mesh;
        chunk.AddComponent<MeshRenderer>().material = _material;
    }
}
