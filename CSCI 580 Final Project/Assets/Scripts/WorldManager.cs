using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    public enum MapType { NoiseMap, Mesh }

    [SerializeField] MapType mapType;
    [SerializeField] bool autoUpdate;
    public bool AutoUpdate => autoUpdate;
    [Header("General Data")]
    [Range(1,100)]
    [SerializeField] int mapChunkSize;
    [Range(1, 10)]
    [SerializeField] int chunkGridWidth;
    [Range(1, 10)]
    [SerializeField] int chunkGridHeight;
    [SerializeField] float wallHeight;
    [SerializeField] float wallHeightOffset;
    [SerializeField] NoiseData noiseData;
    [SerializeField] TextureData textureData;
    [SerializeField] GameObject noiseMapRendererPrefab;
    [SerializeField] Material shaderGraphMaterial;
    [SerializeField] Material noiseMapMaterial;

    //[Header("Noise Map Data")]

    [Header("Mesh Data")]
    [Range(0, 6)]
    public int editorLevelofDetail;
    [SerializeField] TerrainData terrainData;
    [SerializeField] ObjectPopulator objectPopulator;
    [SerializeField] GameObject waterPrefab;
    [SerializeField] CameraManager cameraManager;

    float[,] falloffMap;
    float[,] circleFalloffMap;

    List<GameObject> terrainObjs;

    Shading curShadingMode;

    private void Awake()
    {
        //DrawMapInEditor();
    }

    void OnValuesUpdated()
    {
        if (!Application.isPlaying)
        {
            //DrawMapInEditor();
        }
    }

    public void SetShadingMode(Shading shading)
    {
        curShadingMode = shading;
    }

    private void OnValidate()
    {
        if (noiseData != null)
        {
            noiseData.OnValuesUpdated -= OnValuesUpdated;
            noiseData.OnValuesUpdated += OnValuesUpdated;
        }
        if (terrainData != null)
        {
            terrainData.OnValuesUpdated -= OnValuesUpdated;
            terrainData.OnValuesUpdated += OnValuesUpdated;
        }
        if (textureData != null)
        {
            textureData.OnValuesUpdated -= OnValuesUpdated;
            textureData.OnValuesUpdated += OnValuesUpdated;
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

    public void DrawMapInEditor(TerrainMode terrainMode,int seed = 0)
    {
        noiseData = terrainMode.noiseData;
        textureData = terrainMode.textureData;
        terrainData = terrainMode.terrainData;
        if(seed != 0)
        {
            noiseData.seed = seed;
        }
        if(terrainObjs == null)
        {
            terrainObjs = new List<GameObject>();
        }

        if(terrainObjs.Count > 0)
        {
            foreach(var obj in terrainObjs)
            {
                DestroyImmediate(obj);
            }
            terrainObjs.Clear();
        }

        this.transform.position = Vector3.zero;

        for (int x = 0; x < chunkGridWidth; x++)
        {
            for (int y = 0; y < chunkGridHeight; y++)
            {
                Vector2 chunkPos = new Vector2(0 + (x * (mapChunkSize-1)), 0 + (y * (mapChunkSize-1)));
                MapData mapData = GenerateMapData(chunkPos);
                if (mapType == MapType.NoiseMap)
                {
                    var noiseMapRenderer = Instantiate(noiseMapRendererPrefab,new Vector3(chunkPos.x,0,chunkPos.y),Quaternion.identity,this.transform);
                    noiseMapRenderer.GetComponent<MapRenderer>().DrawTexture(TextureGenerator.TextureFromNoiseMap(mapData.heightMap));
                    terrainObjs.Add(noiseMapRenderer);
                }
                if(mapType == MapType.Mesh)
                {

                    var noiseMapRenderer = Instantiate(noiseMapRendererPrefab, new Vector3(chunkPos.x, 0, chunkPos.y), Quaternion.identity, this.transform);
                    noiseMapRenderer.GetComponent<MapRenderer>().DrawMesh(MeshGenerator.GenerateTerrainMesh(mapData.heightMap, terrainData.meshHeightMultiplier, terrainData.meshAnimationCurve, editorLevelofDetail, terrainData.useFlatShading), TextureGenerator.TextureFromColorMap(mapData.colorMap, mapChunkSize, mapChunkSize));
                    noiseMapRenderer.AddComponent<MeshCollider>();
                    switch(curShadingMode)
                    {
                        case Shading.Shader:
                            noiseMapRenderer.GetComponent<MeshRenderer>().material = shaderGraphMaterial;
                            break;
                        case Shading.NoiseMap:
                            noiseMapRenderer.GetComponent<MeshRenderer>().material = noiseMapMaterial;
                            noiseMapRenderer.GetComponent<MapRenderer>().DrawTexture(TextureGenerator.TextureFromNoiseMap(mapData.heightMap));
                            break;
                        case Shading.ColorMap:
                            noiseMapRenderer.GetComponent<MeshRenderer>().material = noiseMapMaterial;
                            noiseMapRenderer.GetComponent<MapRenderer>().DrawTexture(TextureGenerator.TextureFromColorMap(mapData.colorMap, mapChunkSize, mapChunkSize));
                            break;
                    }

                    //Object Container
                    noiseMapRenderer.tag = "Terrain";
                    GameObject terrainObjectParent = new GameObject();
                    terrainObjectParent.name = "Object Parent";
                    terrainObjectParent.transform.parent = noiseMapRenderer.transform;
                    terrainObjectParent.transform.localPosition = Vector3.zero;
                    objectPopulator.SpawnObjects(noiseMapRenderer.transform, noiseData, terrainData, mapData.heightMap, mapChunkSize, terrainObjectParent.transform);
                    terrainObjs.Add(noiseMapRenderer);
                }
            }
        }
        for(int i = 0; i < 4; i++)
        {
            var wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            switch(i)
            {
                //Vertical Top
                case 0:
                    wall.transform.position = new Vector3(0,wallHeightOffset,((chunkGridHeight * (mapChunkSize-1))/2.0f));
                    wall.transform.localScale = new Vector3((chunkGridWidth * (mapChunkSize-1)), wallHeight, 1);
                    break;
                //Vertical Bottom
                case 1:
                    wall.transform.position = new Vector3(0, wallHeightOffset,(-(chunkGridHeight * (mapChunkSize-1)) / 2.0f));
                    wall.transform.localScale = new Vector3((chunkGridWidth * (mapChunkSize-1)), wallHeight, 1);
                    break;
                //Horizontal Left
                case 2:
                    wall.transform.position = new Vector3(((chunkGridWidth * (mapChunkSize-1)) / 2.0f), wallHeightOffset, 0);
                    wall.transform.localScale = new Vector3(1, wallHeight, 1+(chunkGridHeight * (mapChunkSize-1)));
                    break;
                //Horizontal Right
                case 3:
                    wall.transform.position = new Vector3(-((chunkGridWidth * (mapChunkSize-1)) / 2.0f), wallHeightOffset, 0);
                    wall.transform.localScale = new Vector3(1, wallHeight, 1+(chunkGridHeight * (mapChunkSize-1)));
                    break;
            }
            terrainObjs.Add(wall);
        }
        float gridWidthMultiplier = chunkGridWidth-1;
        float gridHeightMultiplier = chunkGridHeight-1;
        this.transform.position = new Vector3(gridWidthMultiplier*-((mapChunkSize-1)/2.0f),0,gridHeightMultiplier*-((mapChunkSize-1)/2.0f));

        //Generating Water
        var newWater = Instantiate(waterPrefab);
        newWater.transform.parent = this.transform;
        newWater.transform.localPosition = new Vector3(gridWidthMultiplier * (mapChunkSize-1)*.5f, terrainData.waterHeight, gridHeightMultiplier * (mapChunkSize - 1) * .5f);
        newWater.transform.localScale = new Vector3(9.9f * chunkGridWidth, 1, 9.9f * chunkGridHeight);
        terrainObjs.Add(newWater);

        //Camera Controls
        cameraManager.InitCameraPos(new Vector3(gridWidthMultiplier * (mapChunkSize - 1) * .5f, terrainData.waterHeight, gridHeightMultiplier * (mapChunkSize - 1) * .5f));
    }

    MapData GenerateMapData(Vector2 center)
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapChunkSize + 2, mapChunkSize + 2, noiseData.seed, noiseData.noiseScale, noiseData.octaves, noiseData.persistence, noiseData.lacunarity, center + noiseData.offset, noiseData.normalizeMode);
        Color[] colorMap = new Color[mapChunkSize * mapChunkSize];
        if (terrainData.useFalloff)
        {
            if (falloffMap == null)
            {
                falloffMap = FalloffGenerator.GenerateFalloffMap(mapChunkSize + 2);
            }
            for (int x = 0; x < mapChunkSize + 2; x++)
            {
                for (int y = 0; y < mapChunkSize + 2; y++)
                {
                    noiseMap[x, y] = Mathf.Clamp01(noiseMap[x, y] - falloffMap[x, y]);
                }
            }
        }
        if (terrainData.useCircleFalloff)
        {
            circleFalloffMap = FalloffGenerator.GenerateCircularFalloffMap(mapChunkSize + 2, terrainData.circleFalloffRadius, terrainData.circleFalloffGradient);
            for (int x = 0; x < mapChunkSize + 2; x++)
            {
                for (int y = 0; y < mapChunkSize + 2; y++)
                {
                    noiseMap[x, y] = Mathf.Clamp01(noiseMap[x, y] - circleFalloffMap[x, y]);
                }
            }
        }
        for (int x = 0; x < mapChunkSize; x++)
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
        }
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
