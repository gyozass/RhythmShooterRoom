using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    private void OnTriggerEnter(Collider hit)
    {
        if (hit.transform.CompareTag("Player"))
        {
            PlayerHealth playerHealth = hit.transform.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(5f);
                Debug.Log("Enemy damage dealt : " + 5f);
            }
        }
    }
}
