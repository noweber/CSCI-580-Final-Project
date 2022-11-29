using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct TerrainMode
{
    public string modeName;
    public NoiseData noiseData;
    public TextureData textureData;
    public TerrainData terrainData;
}

[System.Serializable]
public struct TreeMode
{
    public string modeName;
    public GameObject prefabToSpawn;
}

public enum Shading { Shader, NoiseMap, ColorMap}
[System.Serializable]
public struct ShaderMode
{
    public string modeName;
    public Shading shadingMode;
}


public class UIController : MonoBehaviour
{
    [SerializeField] WorldManager worldManager;
    [SerializeField] ObjectPopulator objectPopulator;
    [Header("UI Elements")]
    [SerializeField] Text seedText;
    [SerializeField] string seedString;
    [SerializeField] Text modeText;
    [SerializeField] string modeString;
    [SerializeField] Text treeText;
    [SerializeField] string treeString;
    [SerializeField] Text shaderText;
    [SerializeField] string shaderString;
    [SerializeField] Button seedButton;
    [SerializeField] Button modeButton;
    [SerializeField] Button treeButton;
    [SerializeField] Button shaderButton;

    [SerializeField] List<TerrainMode> terrainModes;
    [SerializeField] List<TreeMode> treeModes;
    [SerializeField] List<ShaderMode> shaderModes;
    int curTerrainModeIndex = 0;
    int curTreeModeIndex = 0;
    int curShaderModeIndex = 0;

    private void Awake()
    {
        objectPopulator.SetTreeType(treeModes[0].prefabToSpawn);
        curTreeModeIndex = 0;
        worldManager.SetShadingMode(shaderModes[0].shadingMode);
        worldManager.DrawMapInEditor(terrainModes[0]);
        curTerrainModeIndex = 0;
        UpdateSeedText(terrainModes[0].noiseData.seed);
        UpdateModeText(terrainModes[0].modeName);
        UpdateTreeModeText(treeModes[0].modeName);
        UpdateMaterialText(shaderModes[0].modeName);
        seedButton.onClick.AddListener(GenerateNewSeed);
        modeButton.onClick.AddListener(ToggleMode);
        treeButton.onClick.AddListener(ToggleTrees);
        shaderButton.onClick.AddListener(ToggleShader);
    }

    public void UpdateSeedText(int seed)
    {
        seedText.text = seedString + seed;
    }

    public void UpdateModeText(string modeName)
    {
        modeText.text = modeString + modeName;
    }

    public void UpdateTreeModeText(string modeName)
    {
        treeText.text = treeString + modeName;
    }

    public void UpdateMaterialText(string modeName)
    {
        shaderText.text = shaderString + modeName;
    }

    public void GenerateNewSeed()
    {
        int newSeed = Random.Range(0, 999999);
        worldManager.DrawMapInEditor(terrainModes[curTerrainModeIndex],newSeed);
        UpdateSeedText(newSeed);
    }

    public void ToggleMode()
    {
        curTerrainModeIndex++;
        curTerrainModeIndex = curTerrainModeIndex % terrainModes.Count;
        worldManager.DrawMapInEditor(terrainModes[curTerrainModeIndex]);
        UpdateModeText(terrainModes[curTerrainModeIndex].modeName);
    }

    public void ToggleTrees()
    {
        curTreeModeIndex++;
        curTreeModeIndex = curTreeModeIndex % treeModes.Count;
        objectPopulator.SetTreeType(treeModes[curTreeModeIndex].prefabToSpawn);
        worldManager.DrawMapInEditor(terrainModes[curTerrainModeIndex]);
        UpdateTreeModeText(treeModes[curTreeModeIndex].modeName);
    }

    public void ToggleShader()
    {
        curShaderModeIndex++;
        curShaderModeIndex = curShaderModeIndex % shaderModes.Count;
        worldManager.SetShadingMode(shaderModes[curShaderModeIndex].shadingMode);
        worldManager.DrawMapInEditor(terrainModes[curTerrainModeIndex]);
        UpdateMaterialText(shaderModes[curShaderModeIndex].modeName);
    }

}
