using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class NoteSpriteMover : MonoBehaviour
{
    public static event Action OnReset;
    
    //[SerializeField] private Transform target;
    [SerializeField] private float _timeToReachTarget;
    [SerializeField] private AnimationCurve _animCurve;
    private float _t = 0f;
    private Vector2 _startPosition;
    private Vector2 _targetPosition;

    void Start()
    {
        _startPosition = transform.position;
        _targetPosition = new Vector2(Screen.width / 2, Screen.height / 2);
    }

    void Update()
    {
        _t += Time.deltaTime / _timeToReachTarget;
        _animCurve.Evaluate(_t);

        if (_t >= 0.9f)
        {
            ResetObject();  
        }

        transform.position = Vector2.Lerp(_startPosition, _targetPosition, _t);
        Vector2 v2 = new Vector2(transform.position.x, transform.position.y);

        if (v2 == _targetPosition)
        {
            ResetObject();
        }
    }

    public void ResetObject()
    {
        gameObject.SetActive(false);
        _t = 0;
        transform.position = _startPosition;
        OnReset();


    }

    public void SetDestination(Vector2 destination, float time)
    {
        _t = 0;
        _startPosition = transform.position;
        _timeToReachTarget = time;
        _targetPosition = destination;
    }
}
