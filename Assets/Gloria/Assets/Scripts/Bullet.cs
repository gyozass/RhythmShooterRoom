using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class Bullet : MonoBehaviour
{
    private Vector3 _targetPoint;
    private float _damage;
    private float _speed = 50f;
    private float _destroyDistance = 0.5f; // Distance threshold for destruction

    public void Initialize(Vector3 targetPoint, float damage)
    {
        _targetPoint = targetPoint;
        _damage = damage;
    }

    private void Update()
    {
        if (_targetPoint == Vector3.zero) return;

        transform.position = Vector3.MoveTowards(transform.position, _targetPoint, _speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, _targetPoint) <= _destroyDistance)
        {
      //      gameObject.SetActive(false);
        }
    }
}