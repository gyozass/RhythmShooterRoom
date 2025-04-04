using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewEnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] _enemyPrefabs;
    [SerializeField] private Transform[] _spawnPoints; // Add as many as you want in the Inspector
    private List<GameObject> _spawnedEnemies = new();
    private const int _maxEnemies = 5;

    private void Start()
    {
        SpawnEnemies();
    }

    private void Update()
    {
        CheckEnemyHealth();
    }

    private void SpawnEnemies()
    {
        for (int i = 0; i < _maxEnemies; i++)
        {
            GameObject enemyPrefab = _enemyPrefabs[Random.Range(0, _enemyPrefabs.Length)];
            Transform spawnPoint = _spawnPoints[i % _spawnPoints.Length]; // Cycle through spawn points
            GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
            _spawnedEnemies.Add(enemy);
        }
    }

    private void CheckEnemyHealth()
    {
        int aliveEnemies = 0;

        foreach (GameObject enemy in _spawnedEnemies)
        {
            if (enemy != null)
            {
                aliveEnemies++;
            }
        }

        if (aliveEnemies < 3)
        {
            _spawnedEnemies.RemoveAll(e => e == null); // Clean up dead references
            SpawnEnemies();
        }
    }
}
