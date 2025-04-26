using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

public class TimelineEvents : MonoBehaviour
{
    [Header("Intro Canvas")]
    public CanvasGroup fadeCanvas;

    [Header("Enemies")]
    [SerializeField] private GameObject firstRobot;
    [SerializeField] private GameObject secondRobot;
    [SerializeField] private GameObject crosshairUI;

    private EnemyHealth firstHealth;
    private EnemyHealth secondHealth;

    [Header("Dialogues")]
    [SerializeField] private DialogueData introDialogue;
    [SerializeField] private DialogueData shootTutDialogue;
    [SerializeField] private DialogueData perfectShotDialogue;
    [SerializeField] private DialogueData badShotDialogue;
    [SerializeField] private DialogueData postKillDialogue;

    [Header("Audio")]
    [SerializeField] private AudioSource pulsar;
    [SerializeField] private AudioSource mainGameplaySong;

    [Header("State")]
    private bool hasCompletedUIOnly = false;
    private bool hasRespondedToShot = false;
    private bool hasTriggeredDoor = false;

    private PlayableDirector director;
    [SerializeField] AdvBeatManager advBeatManager;

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

        if (pulsar) pulsar.Stop();
        if (mainGameplaySong) mainGameplaySong.Play();

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
        DialogueSystem.Instance.StartDialogue(postKillDialogue);
    }

    public void SpawnSecondRobot()
    {
        DialogueSystem.Instance.OnDialogueEnd.RemoveListener(SpawnSecondRobot);

        secondRobot.SetActive(true);
        director.Resume(); // resumes to second robot death
        secondHealth.OnDeath.AddListener(OnSecondRobotKilled);
    }

    private void OnSecondRobotKilled()
    {
        secondHealth.OnDeath.RemoveListener(OnSecondRobotKilled);
        director.Pause(); // wait for door trigger
    }

    public void OnDoorTriggered()
    {
        if (hasTriggeredDoor) return;
        hasTriggeredDoor = true;
        director.Resume(); // resumes to camera pan + player push
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
