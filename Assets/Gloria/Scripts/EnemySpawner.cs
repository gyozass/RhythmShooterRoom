using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject prefab;
    public GameObject bulletProjectile;

    [Header("Spawn")]
    public List<Transform> spawnPoints; 
    public int spawnAmount = 10;
    public float spawnRadius = 5f;

    [Header("Wave")]
    public float waveCooldown = 3f;
    private bool isWaveCooldown = false;

    void Start()
    {
        SpawnAtRandomPoint();
    }

    void Update()
    {
        if (IsReadyForNextWave() && !isWaveCooldown)
        {
            StartCoroutine(SpawnNextWaveWithCooldown());
        }
    }

    public void SpawnAtRandomPoint()
    {
        if (spawnPoints.Count == 0)
        {
            Debug.Log("no zombies");
            return;
        }

        Transform chosenSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];

        for (int i = 0; i < spawnAmount; i++)
        {
            Vector3 randomPosition = chosenSpawnPoint.position + Random.insideUnitSphere * spawnRadius;

            randomPosition.y = 0;

            Instantiate(prefab, randomPosition, Quaternion.identity);
            Instantiate(bulletProjectile, randomPosition, Quaternion.identity);
        }
    }

    public bool IsReadyForNextWave()
    {
        int activePrefabs = GameObject.FindGameObjectsWithTag(prefab.tag).Length;

        return activePrefabs == 1;
    }

    public IEnumerator SpawnNextWaveWithCooldown()
    {
        isWaveCooldown = true;
        yield return new WaitForSeconds(waveCooldown);
        SpawnAtRandomPoint();
        isWaveCooldown = false;
    }
}
