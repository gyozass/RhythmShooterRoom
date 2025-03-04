using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BeatSpawner : MonoBehaviour
{
    public enum Judgements
    {
        Perfect,
        Great,
        Good,
        Miss
    }

    public Judgements CurrentJudgement = Judgements.Miss;

    public Canvas canvas;
    public GameObject spritePrefab;
    public Sprite particleEffect;

    [Header("beats")]
    public float beatsPerMinute = 120f;
    public float countdownStart = 2000f;

    [Header("judgement stuff")]
    public int perfectWindow = 25;
    public int greatWindow = 50;
    public int goodWindow = 100;
    public float judgmentRadius = 50f;
    private float spawnInterval;
    private InputAction clickAction;
    private List<GameObject> activeBeatSprites = new List<GameObject>();

    void Awake()
    {
        // Initialize InputAction
        // clickAction = new InputAction(type: InputActionType.Button, binding: "<Mouse>/leftButton");
        // clickAction.performed += ctx => HandleClickOnBeat();
        // clickAction.Enable();

        // Calculate the interval between spawns based on BPM
        spawnInterval = 60f / beatsPerMinute;
    }

    void Start()
    {
        StartCoroutine(SpawnUIElements());
    }

    private IEnumerator SpawnUIElements()
    {
        while (true)
        {
            SpawnSprite();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnSprite()
    {
        GameObject newBeatSprite = Instantiate(spritePrefab, canvas.transform);

        RectTransform spriteRect = newBeatSprite.GetComponent<RectTransform>();
        spriteRect.anchorMin = new Vector2(0.5f, 0.5f);
        spriteRect.anchorMax = new Vector2(0.5f, 0.5f);
        spriteRect.pivot = new Vector2(0.5f, 0.5f);

        bool isFromLeft = Random.value > 0.5f;
        float canvasWidth = canvas.GetComponent<RectTransform>().rect.width;
        Vector2 startPosition = isFromLeft ? new Vector2(-canvasWidth * 0.4f, 0) : new Vector2(canvasWidth * 0.4f, 0);

        spriteRect.anchoredPosition = startPosition;

        BeatSprite beatComponent = newBeatSprite.GetComponent<BeatSprite>();
        if (beatComponent != null)
        {
            beatComponent.InitializeCountdown(countdownStart, isFromLeft);
        }

        activeBeatSprites.Add(newBeatSprite);
    }

    public void HandleClickOnBeat()
    {
        if (activeBeatSprites.Count == 0) return;

        GameObject closestSprite = null;
        float closestCountdown = float.MaxValue;

        for (int i = 0; i < activeBeatSprites.Count; i++)
        {
            GameObject sprite = activeBeatSprites[i];
            BeatSprite beatComponent = sprite.GetComponent<BeatSprite>();
            if (beatComponent == null) continue;

            float distanceFromCenter = Vector2.Distance(sprite.GetComponent<RectTransform>().anchoredPosition, Vector2.zero);
            if (distanceFromCenter > judgmentRadius) continue;

            float currentCountdown = beatComponent.GetCurrentCountdown();

            if (currentCountdown == -1f) continue; // Skip uninitialized notes

            if (Mathf.Abs(currentCountdown) < closestCountdown)
            {
                closestCountdown = Mathf.Abs(currentCountdown);
                closestSprite = sprite;
            }
        }

        if (closestSprite != null)
        {
            BeatSprite beatComponent = closestSprite.GetComponent<BeatSprite>();
            float currentCountdown = beatComponent.GetCurrentCountdown();

            CurrentJudgement = GetJudgment(currentCountdown);
            Debug.Log($"Click Judgment: {CurrentJudgement} with Countdown: {currentCountdown}ms");

            closestSprite.SetActive(false);
            activeBeatSprites.Remove(closestSprite);
            Instantiate(particleEffect, closestSprite.transform.position, Quaternion.identity);
        }
    }

    public Judgements GetJudgment(float countdown)
    {
        if (countdown <= perfectWindow && countdown > 0f) return Judgements.Perfect;
        if (countdown <= greatWindow && countdown >= perfectWindow) return Judgements.Great;
        if (countdown <= goodWindow && countdown >= greatWindow) return Judgements.Good;
        return Judgements.Miss;
    }

    private void OnDisable()
    {
        clickAction?.Disable();
    }
}
