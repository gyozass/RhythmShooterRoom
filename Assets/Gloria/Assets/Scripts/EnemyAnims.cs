using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnims : MonoBehaviour
{
    PlayerHealth player;
    Animator animator;
    private void Start()
    {
        player = FindObjectOfType<PlayerHealth>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        AttackAnim();
    }
    void AttackAnim()
    {
        if (Vector3.Distance(transform.position, player.transform.position) <= 2)
        {
            if (animator != null) 
            {
                animator.SetTrigger("Chasing");
            }
        }
    }

    void RunAnim()
    { 

    }
}
