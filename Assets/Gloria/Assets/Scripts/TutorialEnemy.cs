using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Playables;
using UnityEngine.UI;

public class TutorialEnemy : MonoBehaviour
{
    public TimelineEvents cutsceneManager;
    private enum State { Shooting, Idle }
    private State _currentState;
    private Animator _animator;
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private Transform _weaponRoot;
    [SerializeField] private float _shootInterval = 1f;
    [SerializeField] private float _bulletSpeed = 10f;
    private Transform _player;
    private float _shootTimer;
    PlayableDirector director;
    //private HighlightManager _hmManager;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        _animator = GetComponent<Animator>(); 

        if (_animator == null)
            _animator = GetComponentInChildren<Animator>();

        _currentState = State.Shooting;
    }

    private void Update()
    {
        switch (_currentState)
        {
            case State.Shooting:
                _animator.SetBool("Running", false);
                ShootPlayer();
                FacePlayer();
                break;
            case State.Idle:
                break;
        }
    }
    private void ShootPlayer()
    {
        if (_player == null) return;
        _currentState = State.Shooting;
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

    public void Die()
    {
        Destroy(gameObject);

        director.Resume();
    }
}

