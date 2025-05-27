using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] public float hitPoints = 100f;
    [NonSerialized] public float maxHealth = 100f;
    [SerializeField] GameObject bloodCanvas;
    [SerializeField] PlayerShooting playerShooting;
    [SerializeField] PlayerScore PlayerScore;
    ///  DeathHandler deathHandler;

    public void TakeDamage(float damage)
    {
        playerShooting.ApplyKnockback();
        hitPoints -= damage;
        bloodCanvas.SetActive(true);

        if (hitPoints <= 0)
        {
            Debug.Log("ded"); 
            PlayerPrefs.SetInt("Score", PlayerScore.GetScore());

            SceneManager.LoadScene("EndScene");
        }

        Invoke("DisableBloodCanvas", 0.5f);
    }

    void DisableBloodCanvas()
    {
        bloodCanvas.SetActive(false); 
    }
}

