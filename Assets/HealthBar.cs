using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public PlayerHealth hp;
    public Image image;

    //health bar display fill 
    void Update()
    {
        image.fillAmount = hp.hitPoints / hp.maxHealth;
    }
}
