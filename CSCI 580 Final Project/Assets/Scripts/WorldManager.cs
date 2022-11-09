using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    public enum MapType { NoiseMap }

    [SerializeField] MapType mapType;
    [SerializeField] bool autoUpdate;
    public bool AutoUpdate => autoUpdate;
    [Header("General Data")]
    [SerializeField] int mapChunkSize;
    [SerializeField] NoiseData noiseData;

    [Header("Noise Map Data")]
    [SerializeField] NoiseMapRenderer noiseMapRenderer;

    private void Awake()
    {
        DrawMapInEditor();
    }

    void OnValuesUpdated()
    {
        if (!Application.isPlaying)
        {
            DrawMapInEditor();
        }
    }

    private void OnValidate()
    {
        if (noiseData != null)
        {
            noiseData.OnValuesUpdated -= OnValuesUpdated;
            noiseData.OnValuesUpdated += OnValuesUpdated;
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void DrawMapInEditor()
    {
        MapData mapData = GenerateMapData(Vector2.zero);
        if (mapType == MapType.NoiseMap)
        {
            noiseMapRenderer.DrawTexture(TextureGenerator.TextureFromNoiseMap(mapData.heightMap));
        }
    }

    MapData GenerateMapData(Vector2 center)
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapChunkSize + 2, mapChunkSize + 2, noiseData.seed, noiseData.noiseScale, noiseData.octaves, noiseData.persistence, noiseData.lacunarity, center + noiseData.offset, noiseData.normalizeMode);
        Color[] colorMap = new Color[mapChunkSize * mapChunkSize];
        /*if (terrainData.useFalloff)
        {
            if (falloffMap == null)
            {
                falloffMap = FalloffGenerator.GenerateFalloffMap(mapChunkSize + 2);
            }
            for (int x = 0; x < mapChunkSize + 2; x++)
            {
                for (int y = 0; y < mapChunkSize + 2; y++)
                {
                    if (terrainData.useFalloff)
                    {
                        noiseMap[x, y] = Mathf.Clamp01(noiseMap[x, y] - falloffMap[x, y]);
                    }

                }
            }
        }*/
        /*for (int x = 0; x < mapChunkSize; x++)
        {
            for (int y = 0; y < mapChunkSize; y++)
            {
                float currentHeight = noiseMap[x, y];
                for (int i = 0; i < textureData.terrains.Length; i++)
                {
                    if (currentHeight >= textureData.terrains[i].height)
                    {
                        colorMap[y * mapChunkSize + x] = textureData.terrains[i].color;
                    }
                }
            }
        }*/
        return new MapData(noiseMap, colorMap);
    }

    public struct MapData
    {
        public readonly float[,] heightMap;
        public readonly Color[] colorMap;

        public MapData(float[,] heightMap, Color[] colorMap)
        {
            this.heightMap = heightMap;
            this.colorMap = colorMap;
        }
    }
}
