using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapRenderer : MonoBehaviour
{
    [SerializeField] Renderer textureRender;
    [SerializeField] MeshFilter meshFilter;
    public void DrawTexture(Texture2D texture)
    {
        var tempMaterial = new Material(textureRender.sharedMaterial);
        textureRender.sharedMaterial.mainTexture = texture;
        textureRender.sharedMaterial = tempMaterial;
        // textureRender.transform.localScale = new Vector3(texture.width, 1, texture.height);
    }

    public void DrawMesh(MeshData meshData, Texture2D texture)
    {
        meshFilter.sharedMesh = meshData.CreateMesh();
        // var tempMaterial = new Material(textureRender.sharedMaterial);
        // tempMaterial.mainTexture = texture;
        // textureRender.sharedMaterial = tempMaterial;
    }
}
