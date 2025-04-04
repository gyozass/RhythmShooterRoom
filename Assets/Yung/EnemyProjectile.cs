using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    private void OnTriggerEnter(Collider hit)
    {
        if (hit.transform.CompareTag("Player"))
        {
            EnemyHealth enemyHealth = hit.transform.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(5f);
                Debug.Log("Enemy damage dealt : " + 5f);
            }
        }
    }
}
