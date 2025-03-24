using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RangedEnemyBev : MonoBehaviour
{
    private enum State { Chasing, Shooting, Idle }
    private State _currentState;
    private Animator _animator;
    [SerializeField] private float _stopRadius = 2f;
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private Transform _weaponRoot;
    [SerializeField] private float _shootInterval = 1f;
    [SerializeField] private float _bulletSpeed = 10f;
    private Transform _player;
    private NavMeshAgent _agent;
    private float _shootTimer;

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        _animator = GetComponent<Animator>();
        _currentState = State.Chasing;
    }

    private void Update()
    {
        switch (_currentState)
        {
            case State.Chasing:
                ChasePlayer();
                _animator.SetBool("Running", true);
                break;
            case State.Shooting:
                StopChasing();
                _animator.SetBool("Running", false);
                ShootPlayer();
                FacePlayer();
                break;
            case State.Idle:
                StopChasing();
                break;
        }
    }

    private void ChasePlayer()
    {
        if (_player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, _player.position);
        if (distanceToPlayer > _stopRadius)
        {
            _agent.SetDestination(_player.position);
        }
        else
        {
            _currentState = State.Shooting;
        }
    }

    private void StopChasing()
    {
        _agent.ResetPath();
    }

    private void ShootPlayer()
    {
        if (_player == null) return;

        _shootTimer += Time.deltaTime;
        if (_shootTimer >= _shootInterval)
        {
            _shootTimer = 0f;

            Vector3 direction = (_player.position - _weaponRoot.position).normalized;
            RaycastHit hit;
            if (Physics.Raycast(_weaponRoot.position, direction, out hit))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    FireBullet(direction, _player.position);
                }
            }
        }
    }
    private void FacePlayer()
    {
        if (_player == null) return;

        Vector3 direction = (_player.position - transform.position).normalized;
        direction.y = 0; // Keep rotation on the horizontal plane
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }
    private void FireBullet(Vector3 direction, Vector3 targetPosition)
    {
        GameObject bullet = Instantiate(_bulletPrefab, _weaponRoot.position, Quaternion.identity);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = direction * _bulletSpeed;
        }

        // Deactivate bullet after reaching target
        StartCoroutine(DeactivateBulletAfterDistance(bullet, targetPosition));
    }

    private IEnumerator DeactivateBulletAfterDistance(GameObject bullet, Vector3 targetPosition)
    {
        while (bullet != null && Vector3.Distance(bullet.transform.position, targetPosition) > 0.1f)
        {
            yield return null;
        }
        if (bullet != null)
        {
            bullet.SetActive(false);
            Destroy(bullet);
        }
    }
}
