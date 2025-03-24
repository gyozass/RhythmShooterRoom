using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewEnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] _enemyPrefabs;
    [SerializeField] private Transform _spawnPoint1;
    [SerializeField] private Transform _spawnPoint2;
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
            Transform spawnPoint = (i < _maxEnemies / 2) ? _spawnPoint1 : _spawnPoint2;
            GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
            _spawnedEnemies.Add(enemy);
        }
    }

    private void CheckEnemyHealth()
    {
        int aliveEnemies = _spawnedEnemies.Count;
        foreach (GameObject enemy in _spawnedEnemies)
        {
            if (enemy == null)
            {
                aliveEnemies--;
            }
        }

        if (aliveEnemies <= 3)
        {
            _spawnedEnemies.Clear();
            SpawnEnemies();
        }
    }
}
