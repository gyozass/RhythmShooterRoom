using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewEnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] _enemyPrefabs;
    [SerializeField] private Transform[] _spawnPoints; 
    private List<GameObject> _spawnedEnemies = new();
    private const int _maxEnemies = 10;

    private void Start()
    {
        SpawnEnemies(5);
    }

    private void Update()
    {
        CheckEnemyHealth();
    }

    private void SpawnEnemies(int amount)
    {
        for (int i = 0; i < _maxEnemies; i++)
        {
            GameObject enemyPrefab = _enemyPrefabs[Random.Range(0, _enemyPrefabs.Length)];
            Transform spawnPoint = _spawnPoints[i % _spawnPoints.Length];
            GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
            _spawnedEnemies.Add(enemy);
        }
    }

    private void CheckEnemyHealth()
    {
        _spawnedEnemies.RemoveAll(e => e == null);

        int aliveEnemies = _spawnedEnemies.Count;

        if (aliveEnemies < 3)
        {
            int enemiesToSpawn = _maxEnemies - aliveEnemies;
            SpawnEnemies(enemiesToSpawn);
        }
    }
}
