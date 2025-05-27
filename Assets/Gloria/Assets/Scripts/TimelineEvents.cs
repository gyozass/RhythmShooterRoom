using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.UIElements;

public class TimelineEvents : MonoBehaviour
{
    [Header("Intro Canvas")]
    public CanvasGroup fadeCanvas;

    [Header("Enemies")]
    [SerializeField] private GameObject firstRobot;
    [SerializeField] private GameObject secondRobot;
    [SerializeField] private GameObject crosshairUI;
    [SerializeField] private GameObject enemySpawner;
    private EnemyHealth firstHealth;
    private EnemyHealth secondHealth;

    [Header("Dialogues")]
    [SerializeField] private DialogueData introDialogue;
    [SerializeField] private DialogueData shootTutDialogue;
    [SerializeField] private DialogueData perfectShotDialogue;
    [SerializeField] private DialogueData badShotDialogue;
    [SerializeField] private DialogueData postKillDialogue;
    [SerializeField] private DialogueData secondDeathDialogue;

    [Header("Audio")]
    [SerializeField] public AudioSource pulsar;
    [SerializeField] public AudioSource mainGameplaySong;

    [Header("bool")]
    private bool hasCompletedUIOnly = false;
    private bool hasRespondedToShot = false;

    [Header("Components")]
    private PlayableDirector director;
    [SerializeField] AdvBeatManager advBeatManager;
    [SerializeField] DoorOpen doorOpen;
    [SerializeField] AudioTimer audioTimer;

    void Start()
    {
        director = GetComponent<PlayableDirector>();
        firstHealth = firstRobot.GetComponent<EnemyHealth>();
        secondHealth = secondRobot.GetComponent<EnemyHealth>();

        if (pulsar) pulsar.Play();
    }

    public void PlayIntroDialogue()
    {
        director.Pause();
        DialogueSystem.Instance.OnDialogueEnd.AddListener(OnIntroDialogueEnd);
        DialogueSystem.Instance.StartDialogue(introDialogue);
    }

    private void OnIntroDialogueEnd()
    {
        DialogueSystem.Instance.OnDialogueEnd.RemoveListener(OnIntroDialogueEnd);
        director.Resume(); // resumes to FadeFromBlack signal
    }

    public void StartTutorial1()
    {
        director.Pause();

        hasCompletedUIOnly = true;
        hasRespondedToShot = false;

        crosshairUI.SetActive(true);

        StartCoroutine(advBeatManager.CountDown());
        DialogueSystem.Instance.StartDialogue(shootTutDialogue);
    }

    public void OnPlayerShot(HitType hitType)
    {
        if (!hasCompletedUIOnly || hasRespondedToShot) return;
        hasRespondedToShot = true;

        StartCoroutine(HandleShotWithDelay(hitType));
    }

    private IEnumerator HandleShotWithDelay(HitType hitType)
    {
        yield return new WaitForSeconds(1f);

        DialogueSystem.Instance.OnDialogueEnd.AddListener(SpawnFirstRobot);

        switch (hitType)
        {
            case HitType.Perfect:
                DialogueSystem.Instance.StartDialogue(perfectShotDialogue);
                break;

            case HitType.Okay:
            case HitType.Good:
            case HitType.Miss:
                DialogueSystem.Instance.StartDialogue(badShotDialogue);
                break;
        }
    }

    private void SpawnFirstRobot()
    {
        DialogueSystem.Instance.OnDialogueEnd.RemoveListener(SpawnFirstRobot);

      // if (pulsar) pulsar.Stop();
      // if (mainGameplaySong) mainGameplaySong.Play();

        firstRobot.SetActive(true);
        crosshairUI.SetActive(true);

        director.Pause();
        firstHealth.OnDeath.AddListener(OnFirstRobotKilled);
    }

    private void OnFirstRobotKilled()
    {
        firstHealth.OnDeath.RemoveListener(OnFirstRobotKilled);
        director.Resume(); // resumes to PostKillDialogue signal
    }

    public void PlayPostKillDialogue()
    {
        director.Pause();
        DialogueSystem.Instance.OnDialogueEnd.AddListener(SpawnSecondRobot);
        StartCoroutine(StartPostKillDialogueNextFrame());
    }

    private IEnumerator StartPostKillDialogueNextFrame()
    {
        yield return null; // wait 1 frame
        DialogueSystem.Instance.StartDialogue(postKillDialogue);
    }

    public void SpawnSecondRobot()
    {
        if (secondRobot != null)
        {
            secondRobot.SetActive(true);
        }
        DialogueSystem.Instance.OnDialogueEnd.RemoveListener(SpawnSecondRobot);

        director.Pause();
        secondHealth.OnDeath.AddListener(OnSecondRobotKilled);
    }

    private void OnSecondRobotKilled()
    {
        secondHealth.OnDeath.RemoveListener(OnSecondRobotKilled);
        director.Resume(); // resume timeline

        StartCoroutine(HandleSecondDeathDialogue());
    }

    private IEnumerator HandleSecondDeathDialogue()
    {
        yield return new WaitForSeconds(2f); // wait 1 frame to let Unity clean up the enemy properly

        DialogueSystem.Instance.OnDialogueEnd.AddListener(OnSecondDeathDialogueFinished);
        DialogueSystem.Instance.StartDialogue(secondDeathDialogue);
    }
    private void OnSecondDeathDialogueFinished()
    {
        DialogueSystem.Instance.OnDialogueEnd.RemoveListener(OnSecondDeathDialogueFinished);
        // After dialogue finishes, wait for door trigger (don't resume yet)
        Debug.Log("Second death dialogue finished, now waiting for door trigger...");
    }
    public void OnDoorTriggered()
    {
        if (doorOpen.hasTriggered) return;
        doorOpen.hasTriggered = true;
        director.Resume(); // resumes to camera pan + player push
    }

    public void StartGameplay()
    {
        mainGameplaySong.Stop();
    }
    public void PauseTimeline()
    {
        if (director != null)
        {
            director.Pause();
            Debug.Log("Timeline paused after camera pan and player push.");
        }
    }

    public void FadeFromBlack()
    {
        StartCoroutine(FadeRoutine());
    }

    private IEnumerator FadeRoutine()
    {
        fadeCanvas.gameObject.SetActive(true);
        fadeCanvas.alpha = 1;
        fadeCanvas.blocksRaycasts = true;

        float duration = 5f, t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            fadeCanvas.alpha = Mathf.Lerp(1, 0, t / duration);
            yield return null;
        }

        fadeCanvas.alpha = 0;
        fadeCanvas.blocksRaycasts = false;
        fadeCanvas.interactable = false;
        fadeCanvas.gameObject.SetActive(false);
    }

}
