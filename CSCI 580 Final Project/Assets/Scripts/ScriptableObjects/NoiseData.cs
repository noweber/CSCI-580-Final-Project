using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class NoiseData : UpdatableData
{
    public Noise.NormalizeMode normalizeMode;
    public float noiseScale;

    [Range(0, 8)]
    public int octaves;
    [Range(0, 1)]
    public float persistence;
    [Range(0, 100)]
    public float lacunarity;

    public int seed;
    public Vector2 offset;

    protected override void OnValidate()
    {
        base.OnValidate();
        if (lacunarity < 1)
        {
            lacunarity = 1;
        }
        if (octaves < 0)
        {
            octaves = 0;
        }
    }
}