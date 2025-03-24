using StarterAssets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] Transform player ;
    [SerializeField] float chaseRange = 5f;
    [SerializeField] float turnSpeed = 5f;
    bool isProvoked = false;
     

    NavMeshAgent navMeshAgent;
    float distanceToTarget = Mathf.Infinity;

    void Start()
    {
        player = FindObjectOfType<FirstPersonController>().transform;

        navMeshAgent = GetComponent<NavMeshAgent>();  
    }

    void Update()
    {

        distanceToTarget = Vector3.Distance(player.position, transform.position);
        if (isProvoked)
        {
            EngageTarget();
        }

        else if (distanceToTarget <= chaseRange)
        {
            isProvoked = true;
        }
    }

    public void OnDamageTaken()
    {
        isProvoked=true;
    }

    private void EngageTarget()
    {
        FaceTarget();
        if (distanceToTarget >= navMeshAgent.stoppingDistance)
        {
            ChaseTarget();
        }
        if (distanceToTarget <= navMeshAgent.stoppingDistance) 
        {
            AttackTarget();
        }
    }

    private void ChaseTarget()
    {
       navMeshAgent.SetDestination(player.position);
    }

    private void AttackTarget()
    {
        //Debug.Log(name + "chasing" + player.name);
    }

    private void FaceTarget()
    {
        //whr the player (oppo) is minus where enemy is (me)
        Vector3 direction = (player.position - transform.position).normalized; //<- keep sem direction, magnitude (length) set to 1.0
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        //Slerp(me current rotation, new look rotation we wan look at, time/speed)
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * turnSpeed);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
    }


}
