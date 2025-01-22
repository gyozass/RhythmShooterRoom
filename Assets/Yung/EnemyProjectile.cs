using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float speed = 10f; 
    private Vector3 targetPosition;

    public void Initialize(Vector3 playerPosition)
    {
        targetPosition = playerPosition;
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            Despawn();
        }
    }

    private void Despawn()
    {
        gameObject.SetActive(false);
    }
}
