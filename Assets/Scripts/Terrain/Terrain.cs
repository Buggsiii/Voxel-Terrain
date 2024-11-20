using UnityEngine;

public abstract class Terrain
{
    protected Transform _parent;
    protected Material _material;
    protected TerrainSettings _terrainSettings;

    public Terrain(Transform parent, Material material, TerrainSettings terrainSettings)
    {
        _parent = parent;
        _material = material;
        _terrainSettings = terrainSettings;
    }

    public abstract void GenerateTerrain();

    protected bool IsVoxelAt(int x, int y, int z)
    {
        float xScaled = x * _terrainSettings.NoiseScale;
        float yScaled = y * _terrainSettings.NoiseScale;
        float zScaled = z * _terrainSettings.NoiseScale;

        float noiseValue = Perlin.Noise(xScaled, yScaled, zScaled);
        float val = (noiseValue + 1) * 0.5f;
        if (y > 0) val += y / (_terrainSettings.ChunkCount.y * 8f);
        bool isVoxel = val < _terrainSettings.Density;

        return isVoxel;
    }
}
