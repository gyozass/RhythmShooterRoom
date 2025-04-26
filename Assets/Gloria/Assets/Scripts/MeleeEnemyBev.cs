using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.Experimental.GraphView.GraphView;

public class MeleeEnemyBev : MonoBehaviour
{
    private enum State { Chasing, PreparingToDash, Dashing }

    [SerializeField] private float EnemyDamage;
    [SerializeField] private GameObject groundAttackEffect;

    bool hasDealtDamage = false;

    [Header("Settings")]
    public float attackRange = 5f; 
    public float waitBeforeDash = 2f;
    public float dashSpeed = 20f;  
    public float dashDuration = 0.5f;

    private State currentState = State.Chasing;
    private Transform player;
    private NavMeshAgent agent;
    private Vector3 dashDirection;

    private PlayerHealth playerHealth;
    private Animator animator;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerHealth = player.GetComponent<PlayerHealth>(); 
        animator = GetComponent<Animator>();

        if (player == null)
        {
            Debug.LogError("Player tag not found!");
            enabled = false;
        }
    }

    private void Update()
    {
        FacePlayer();
        switch (currentState)
        {
            case State.Chasing:
                ChasePlayer();
                break;
            case State.PreparingToDash:
                animator.SetBool("isChasing", true);
                break;
            case State.Dashing:
                DashAttack();
                break;
        }
    }

    private void ChasePlayer()
    {
        if (player == null) return;

        agent.SetDestination(player.position);

        if (Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            StartCoroutine(PrepareToDash());
        }
    }

    private IEnumerator PrepareToDash()
    {
        currentState = State.PreparingToDash;
        agent.isStopped = true;
        yield return new WaitForSeconds(waitBeforeDash);

        if (player != null)
        {
            dashDirection = (player.position - transform.position).normalized;
        }

        currentState = State.Dashing;
        //agent.isStopped = false; 
        agent.speed = dashSpeed;
        StartCoroutine(Dash());
    }

    private IEnumerator Dash()
    {
        float timer = 0f;
        while (timer < dashDuration)
        {
            timer += Time.deltaTime;
            agent.Move(dashDirection * dashSpeed * Time.deltaTime);
            yield return null;
        }
    }

    private void DashAttack()
    {
        if (!hasDealtDamage && Vector3.Distance(transform.position, player.position) <= 3f)
        {
            playerHealth.TakeDamage(EnemyDamage);
            DeactivateEnemy();
        }

        animator.SetBool("isChasing", false);
        hasDealtDamage = true;

        Instantiate(groundAttackEffect,gameObject.transform.position, Quaternion.identity);
    }

    private void DeactivateEnemy()
    {
        groundAttackEffect.SetActive(false);    
        gameObject.SetActive(false);
    }

    private void FacePlayer()
    {
        if (player == null) return;

        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0; 
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }
}