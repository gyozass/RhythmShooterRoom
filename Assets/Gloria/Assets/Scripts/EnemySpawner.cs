using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject prefab;
   // public GameObject bulletProjectile;

    [Header("Spawn")]
    public List<Transform> spawnPoints;
    public int spawnAmount = 20;
    public float spawnRadius = 100f;

    [Header("Wave")]
    public float waveCooldown = 3f;
    public float bulletSpawnInterval = 1f; 
    private bool isWaveCooldown = false;
    private bool isPlayerTriggered = false;

    private Coroutine bulletSpawnCoroutine;

    void Start()
    {

    }

    void Update()
    {
        if (IsReadyForNextWave() && !isWaveCooldown && isPlayerTriggered)
        {
            StartCoroutine(SpawnNextWaveWithCooldown());
        }
    }

    public void SpawnAtRandomPoint()
    {
        if (spawnPoints.Count <= 2)
        {
            Debug.Log("No spawn points available");
            return;
        }

        Transform chosenSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];

        for (int i = 0; i < spawnAmount; i++)
        {
            Vector3 randomPosition = chosenSpawnPoint.position + Random.insideUnitSphere * spawnRadius;
            randomPosition.y = 0;

            GameObject spawnedPrefab = Instantiate(prefab, randomPosition, Quaternion.identity);

            //Instantiate(bulletProjectile, randomPosition, Quaternion.identity);

            // bullet keep spawning till enemy dies 
            if (bulletSpawnCoroutine == null)
            {
                bulletSpawnCoroutine = StartCoroutine(SpawnBullets(spawnedPrefab));
            }
        }
    }

    public IEnumerator SpawnBullets(GameObject targetPrefab)
    {
        while (targetPrefab != null)
        {
            Vector3 spawnPosition = targetPrefab.transform.position;
            //Instantiate(bulletProjectile, spawnPosition, Quaternion.identity);
            yield return new WaitForSeconds(bulletSpawnInterval);
        }

        bulletSpawnCoroutine = null;
    }

    public bool IsReadyForNextWave()
    {
        int activePrefabs = GameObject.FindGameObjectsWithTag(prefab.tag).Length;
        return activePrefabs == 0;
    }

    public IEnumerator SpawnNextWaveWithCooldown()
    {
        isWaveCooldown = true;
        yield return new WaitForSeconds(waveCooldown);
        SpawnAtRandomPoint();
        isWaveCooldown = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerTriggered = true;
            SpawnAtRandomPoint();
        }
    }
}
