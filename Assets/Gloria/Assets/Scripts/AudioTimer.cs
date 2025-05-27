using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioTimer : MonoBehaviour
{
    public AudioSource audioSource; 
    public string sceneToLoad; 
    public TextMeshProUGUI timerText;
    public PlayerScore PlayerScore;

    private float timer;
    public bool timerStarted = false;

    void Start()
    {
        if (audioSource != null)
        {
            timer = audioSource.clip.length;
            audioSource.Play();
        }
        else
        {
            Debug.LogError("AudioSource not assigned!");
        }
    }

    void Update()
    {
        if (timerStarted)
        {
            timer -= Time.deltaTime;

            // Clamp timer so it never goes below zero
            timer = Mathf.Max(timer, 0f);

            UpdateTimerUI(timer);

            if (timer <= 0f)
            {
                PlayerPrefs.SetInt("Score", PlayerScore.GetScore());
                SceneManager.LoadScene(sceneToLoad);
            }
        }
    }

    void UpdateTimerUI(float timeRemaining)
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60f);
        int seconds = Mathf.FloorToInt(timeRemaining % 60f);
        int milliseconds = Mathf.FloorToInt((timeRemaining * 1000f) % 1000f);

        timerText.text = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
    }
}