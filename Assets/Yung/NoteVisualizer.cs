using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteVisualizer : MonoBehaviour
{
    [SerializeField] private bool _stopSpawn;
    [SerializeField] private GameObject[] _noteSprites;        
    [SerializeField] private GameObject[] _noteSprites2;        
    [SerializeField] private float _spawnInterval;
    [SerializeField] private int _currentToSpawn;


    private void OnEnable()
    {
        //GameController.OnLastClick += SpawnNote;
       // NoteSpriteMover.OnReset += ReduceSpawn;
    }

    private void OnDisable()
    {
        //GameController.OnLastClick -= SpawnNote;
        //NoteSpriteMover.OnReset -= ReduceSpawn;
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < _noteSprites.Length; i++)
        {
            _noteSprites[i].SetActive(false);
        }

        for (int i = 0; i < _noteSprites2.Length; i++)
        {
            _noteSprites2[i].SetActive(false);
        }

        StartCoroutine(SpawnRoutine());
    }

    public void ReduceSpawn()
    {
        _currentToSpawn--;
    }

    public void SpawnNote()
    {           
        if (_currentToSpawn < _noteSprites.Length)
        {
            _noteSprites[_currentToSpawn].SetActive(true);
            _currentToSpawn++;
        }

        if (_currentToSpawn < _noteSprites2.Length)
        {
            _noteSprites2[_currentToSpawn].SetActive(true);
            _currentToSpawn++;
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            Debug.Log("spawning");
            SpawnNote();
        }
    }
}
