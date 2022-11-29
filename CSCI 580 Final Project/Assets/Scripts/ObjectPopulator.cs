using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPopulator : MonoBehaviour
{
    [SerializeField] float raycastStartHeight;
    [SerializeField] GameObject prefabToSpawn;
    [SerializeField] float minPoissonRadius;
    [SerializeField] float maxPoissonRadius;
    [SerializeField] float minSpawnHeight;
    [SerializeField] float maxSpawnHeight;

    public void SpawnObjects(Transform parentMesh, NoiseData noiseData, TerrainData terrainData, float[,] noiseMap, float mapChunkSize, Transform parentObj)
    {
        Debug.Log("Spawning");
        List<Vector2> objectSpawnPoints = PoissonDiscSampler.GeneratePoints(noiseData.seed, minPoissonRadius, maxPoissonRadius, new Vector2(mapChunkSize, mapChunkSize));
        foreach (Vector2 objectSpawnPoint in objectSpawnPoints)
        {
            float spawnX = -(mapChunkSize / 2 - objectSpawnPoint.x);
            float spawnZ = (mapChunkSize / 2 - objectSpawnPoint.y);
            float spawnY = terrainData.meshAnimationCurve.Evaluate(noiseMap[(int)objectSpawnPoint.x, (int)objectSpawnPoint.y]);
            if (spawnY >= minSpawnHeight && spawnY <= maxSpawnHeight)
            {
                RaycastHit hit;
                if (Physics.Raycast((new Vector3(spawnX, raycastStartHeight, spawnZ) * terrainData.uniformScale) + parentMesh.position, transform.TransformDirection(-Vector3.up), out hit, Mathf.Infinity))
                {
                    Debug.Log("Hit");
                    GameObject obj = Instantiate(prefabToSpawn, hit.point, Quaternion.Euler(0, Random.Range(0, 360), 0), parentObj);
                }
                else
                {
                    //GameObject obj = Instantiate(prefabToSpawn, new Vector3(spawnX, raycastStartHeight, spawnZ), Quaternion.Euler(0, Random.Range(0, 360), 0), parentObj);
                }
            }
        }
    }
}
