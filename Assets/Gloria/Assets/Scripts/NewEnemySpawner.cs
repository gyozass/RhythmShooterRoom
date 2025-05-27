using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NewEnemySpawner : MonoBehaviour
{
    [Header("Enemy Settings")]
    [SerializeField] private GameObject[] _enemyPrefabs;
    [SerializeField] private GameObject _eliteEnemyPrefab;
    [SerializeField] private Transform[] _spawnPoints;
    [SerializeField] private int _maxEnemies = 10;

    [Header("Wave Settings")]
    [SerializeField] private int _startEnemies = 3;
    [SerializeField] private int _maxWaveCount = 10;
    [SerializeField] private int _eliteStartWave = 5;

    [Header("Rhythm Settings")]
    [SerializeField] private float bpm = 120f; 

    private readonly float[] _beatPattern = new float[] { 1f, 0.5f, 0.5f, 1f };

    private List<GameObject> _spawnedEnemies = new();
    private int _currentWave = 0;
    public int CurrentWave => _currentWave; 

    private float SecondsPerBeat => 60f / bpm;

    private void Start()
    {
        StartCoroutine(SpawnWaves());
    }

    private void Update()
    {
        _spawnedEnemies.RemoveAll(e => e == null || !e.activeInHierarchy);
    }

    private IEnumerator SpawnWaves()
    {
        while (_currentWave < _maxWaveCount)
        {
            _currentWave++;

            int enemiesThisWave = _startEnemies + (_currentWave * 2);
            enemiesThisWave = Mathf.Min(enemiesThisWave, _maxEnemies);

            yield return StartCoroutine(SpawnEnemiesWithRhythm(enemiesThisWave));

            yield return new WaitUntil(() =>
            {
                _spawnedEnemies.RemoveAll(e => e == null);
                return _spawnedEnemies.Count == 3;
            });

            yield return new WaitForSeconds(SecondsPerBeat * 4);
        }

        Debug.Log("all waves completed");
    }

    private IEnumerator SpawnEnemiesWithRhythm(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            SpawnEnemy();

            float rhythmDelay = SecondsPerBeat * _beatPattern[i % _beatPattern.Length];
            yield return new WaitForSeconds(rhythmDelay);
        }
    }

    private void SpawnEnemy()
    {
        if (_spawnedEnemies.Count >= _maxEnemies) return;

        GameObject prefabToSpawn;

        if (_currentWave >= _eliteStartWave && Random.value < 0.2f)
        {
            prefabToSpawn = _eliteEnemyPrefab;
        }
        else
        {
            prefabToSpawn = _enemyPrefabs[Random.Range(0, _enemyPrefabs.Length)];
        }

        Transform spawnPoint = _spawnPoints[Random.Range(0, _spawnPoints.Length)];
        GameObject enemy = Instantiate(prefabToSpawn, spawnPoint.position, Quaternion.identity);
        _spawnedEnemies.Add(enemy);
    }
}