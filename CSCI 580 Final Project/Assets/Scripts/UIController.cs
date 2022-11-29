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

public class UIController : MonoBehaviour
{
    [SerializeField] WorldManager worldManager;
    [Header("UI Elements")]
    [SerializeField] Text seedText;
    [SerializeField] string seedString;
    [SerializeField] Text modeText;
    [SerializeField] string modeString;
    [SerializeField] Button seedButton;
    [SerializeField] Button modeButton;

    [SerializeField] List<TerrainMode> terrainModes;
    int curTerrainModeIndex = 0;

    private void Awake()
    {
        worldManager.DrawMapInEditor(terrainModes[0]);
        curTerrainModeIndex = 0;
        UpdateSeedText(terrainModes[0].noiseData.seed);
        UpdateModeText(terrainModes[0].modeName);
        seedButton.onClick.AddListener(GenerateNewSeed);
        modeButton.onClick.AddListener(ToggleMode);
    }

    public void UpdateSeedText(int seed)
    {
        seedText.text = seedString + seed;
    }

    public void UpdateModeText(string modeName)
    {
        modeText.text = modeString + modeName;
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

}
