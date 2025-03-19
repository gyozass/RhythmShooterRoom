using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewEnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] _enemyPrefabs;
    [SerializeField] private Transform _spawnPoint;
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
            GameObject enemy = Instantiate(enemyPrefab, _spawnPoint.position, Quaternion.identity);
            _spawnedEnemies.Add(enemy);
        }
    }

    private void CheckEnemyHealth()
    {
        int deadEnemies = 0;
        foreach (GameObject enemy in _spawnedEnemies)
        {
            if (enemy == null)
            {
                deadEnemies++;
            }
        }

        if (deadEnemies >= 4)
        {
            _spawnedEnemies.Clear();
            SpawnEnemies();
        }
    }
}
