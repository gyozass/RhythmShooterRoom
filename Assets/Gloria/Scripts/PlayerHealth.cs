using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] public float hitPoints = 100f;
  ///  DeathHandler deathHandler;

    public void TakeDamage(float damage)
    {
        hitPoints -= damage;

        if (hitPoints <= 0)
        {
            Debug.Log("ded");
           
        }
    }
}

