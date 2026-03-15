using System.Collections;
using UnityEngine;

public class GrenadeSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject autoGrenadePrefab;
    public GameObject throwableGrenadePrefab;

    [Header("Spawn Timing")]
    public float minSpawnInterval = 4f;
    public float maxSpawnInterval = 9f;

    [Header("Spawn Area")]
    public float spawnRangeX = 3f;
    public float spawnRangeY = 2f;

    private void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            float wait = Random.Range(minSpawnInterval, maxSpawnInterval);
            yield return new WaitForSeconds(wait);

            SpawnGrenade();
        }
    }

    private void SpawnGrenade()
    {
        bool spawnAuto = Random.value > 0.5f;
        GameObject prefab = spawnAuto ? autoGrenadePrefab : throwableGrenadePrefab;

        if (prefab == null) return;

        Vector2 spawnPos = new Vector2(
            Random.Range(-spawnRangeX, spawnRangeX),
            Random.Range(-spawnRangeY, spawnRangeY)
        );

        Instantiate(prefab, spawnPos, Quaternion.identity);
    }
}