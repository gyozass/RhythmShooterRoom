using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Cinemachine;
using Unity.PlasticSCM.Editor.WebApi;
using TMPro;

public class BeatManager : MonoBehaviour{
    
    [SerializeField] private float _bpm;
    [SerializeField] private float _sampledTime;
    [SerializeField] private float _lastClickedTime;
    [SerializeField] private float[] _valHolder = new float[2];
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] internal BeatDataSO beatDataSO;
    //[SerializeField] private Beat[] _beats;
    [SerializeField] private GameObject __go_beatEffectObject;
    [SerializeField] private GameObject[] _beatEffectObject;
    [SerializeField] private TextMeshProUGUI textMeshProUGUI;
    [SerializeField] TimelineEvents _timelineEvents;
    public float xOffset = 0f;
    public float roundedDifference;

    private void OnEnable()
    {
        GameController.OnLastClick += SetLastClickedTime;
        GameController.OnLastClick += DetermineHit;
    }

    private void OnDisable()
    {
        GameController.OnLastClick -= SetLastClickedTime;
        GameController.OnLastClick -= DetermineHit;

    }

    private void Start()
    {
      //  textMeshProUGUI = FindObjectOfType<TextMeshProUGUI>();  
        //for repositioning the gameobject when spawned
        Vector3 offsetVector = new();

        _beatEffectObject = new GameObject[beatDataSO._beats.Length];

        //Spawn "Note Cubes" for visualization
        for (int i = 0; i < beatDataSO._beats.Length; i++)
        {
            //Debug.Log("ID: " + i);
            offsetVector.x = i * xOffset;
            _beatEffectObject[i] = Instantiate(__go_beatEffectObject, transform.position + offsetVector, Quaternion.identity) as GameObject;            
            IObjectEffect iObjectEffect = _beatEffectObject[i].GetComponent<IObjectEffect>();            
            iObjectEffect.SetInfo(i, beatDataSO._beats[i], true);
            //Debug.Log("ID: Done");
        }
    }
    private void Update()
    {
        //loop through each type of 'Beat' we want to track for in the game
        for (int i = 0; i < beatDataSO._beats.Length; i++)
        {
            //float _sampledTime = (_audioSource.timeSamples / (_audioSource.clip.frequency * beatDataSO._beats[i].GetBeatLength(_bpm)));
            //_sampledTime = (_audioSource.timeSamples / (_audioSource.clip.frequency * beatDataSO._beats[i].GetBeatLength(_bpm)));
            _valHolder[0] = (_audioSource.timeSamples / (_audioSource.clip.frequency * beatDataSO._beats[i].GetBeatLength(_bpm)));
            
            beatDataSO._beats[i].CheckForNextBeat(_valHolder[0]);
        }        
    }

    public void SetLastClickedTime()
    {
        //last clicked time
        _valHolder[1] = _valHolder[0];
    }

    public float _terribleThreshold = 0.4f,
                 _badThreshold = 0.3f,
                 _okThreshold = 0.2f, 
                 _goodThreshold = 0.1f, 
                 _perfectThreshold = 0.05f;

    public void DetermineHit()
    {
        textMeshProUGUI = FindObjectOfType<TextMeshProUGUI>();
        float roundedValue = (float)Math.Round(_valHolder[1]);
        roundedDifference = Mathf.Abs(roundedValue - _valHolder[1]);

            if (roundedDifference > _terribleThreshold)
            {
                Debug.Log("miss");
            }
            else if (roundedDifference > _badThreshold)
            {
                textMeshProUGUI.text = "Terrible".ToString();
                Debug.Log("terrible");
                _timelineEvents.OnPlayerShot("Bad");

        }
            else if (roundedDifference > _okThreshold)
            {
                textMeshProUGUI.text = "Bad".ToString();
                _timelineEvents.OnPlayerShot("Bad");
                Debug.Log("bad");
            }
            else if (roundedDifference > _goodThreshold)
            {
                textMeshProUGUI.text = "ok".ToString();
                _timelineEvents.OnPlayerShot("Bad");
                Debug.Log("ok");
            }
            else if (roundedDifference > _perfectThreshold)
            {
                textMeshProUGUI.text = "great".ToString();
                Debug.Log("great");
                _timelineEvents.OnPlayerShot("Bad");
            }

            else
            {
                textMeshProUGUI.text = "perfect".ToString();
            _timelineEvents.OnPlayerShot("Perfect");
            Debug.Log("perfect");
            }


            Invoke("DisableText", 0.6f);

    }

    public float GetValue(int value)
    {
        //[0] - sample size
        //[1] - last clicked time        

        return _valHolder[value];
    }

    void DisableText()
    {
        textMeshProUGUI.text = "";
    }
}

[System.Serializable]
public class Beat
{
    [SerializeField] private float _steps;
    private int _last_beatInterval;

    //Declare the delegate action here so we can specify separate objects that subscribe to a specific beat step
    public event Action OnBeat;    

    public float GetBeatLength(float bpm)
    {
        return 60f / (bpm * _steps);
    }

    public void CheckForNextBeat (float beatInterval)
    {
        //We round down the interval value to the last whole number,
        //this way we know the last whole number that was passed.
        //Debug.Log("To Floor " + _steps);
        if (Mathf.FloorToInt(beatInterval) != _last_beatInterval)
        {
            _last_beatInterval = Mathf.FloorToInt(beatInterval);
            OnBeat?.Invoke();
        }
    }

    public float GetSteps()
    {
        return _steps;
    }
}