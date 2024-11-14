using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using MoreMountains.Feedbacks;

public class EnemyHealth : MonoBehaviour
{
   // public MMFeedbacks HitFeedback;
   // public MMFeedbacks ShakeFeedback;
    [SerializeField] float hitPoints = 100f;
    RaycastHit hit;
    Weapon weapon;


    private void Start()
    {
     //  HitFeedback?.Initialization(this.gameObject);
     //  ShakeFeedback?.Initialization(this.gameObject);
    }

    public void TakeDamage(float damage)
    {
        BroadcastMessage("OnDamageTaken");
        hitPoints -= damage;

      // ShakeFeedback?.PlayFeedbacks(this.transform.position);
      // HitFeedback?.PlayFeedbacks(this.transform.position);


        if (hitPoints <= 0)
        {
            Destroy(gameObject);
        }
    }




}
