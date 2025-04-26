using SonicBloom.Koreo.Players;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunSFXManager : MonoBehaviour
{
    [SerializeField] List<AudioClip> gunAudioArr = new List<AudioClip>();
    PlayerShooting playerShooting;
    AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        playerShooting = GetComponent<PlayerShooting>();    
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (playerShooting.isWithinThreshold)
        {
            if (gunAudioArr != null && gunAudioArr.Count > 0)
            {
                int randomIndex = Random.Range(0, gunAudioArr.Count);
                audioSource.PlayOneShot(gunAudioArr[randomIndex]);
                playerShooting.isWithinThreshold = false;
            }
        }
    }
}
