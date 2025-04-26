using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] public float hitPoints = 100f;
    [NonSerialized] public float maxHealth = 100f;
    [SerializeField] Canvas bloodCanvas;
  ///  DeathHandler deathHandler;

    public void TakeDamage(float damage)
    {
        hitPoints -= damage;
        bloodCanvas.enabled = true;

        if (hitPoints <= 0)
        {
            Debug.Log("ded");
           
        }

        Invoke("DisableBloodCanvas", 1f);
    }

    void DisableBloodCanvas()
    {
        bloodCanvas.enabled = false; 
    }
}

