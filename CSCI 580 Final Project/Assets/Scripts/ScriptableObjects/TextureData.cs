using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class TextureData : UpdatableData
{
    public TerrainType[] terrains;
}

[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color color;
}