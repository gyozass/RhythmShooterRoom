using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEnemySpawner : MonoBehaviour
{
    [SerializeField] GameObject enemySpawner;
    [SerializeField] TimelineEvents timelineEvents;
    [SerializeField] AudioTimer audioTimer;
   

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            enemySpawner.SetActive(true);
            audioTimer.timerStarted = true;
            timelineEvents.mainGameplaySong.Play();

        }
    }
}
