using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BeatSpawner : MonoBehaviour
{
    public Canvas canvas;
    public GameObject spritePrefab;

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
        clickAction = new InputAction(type: InputActionType.Button, binding: "<Mouse>/leftButton");
        clickAction.performed += ctx => HandleClickOnBeat();
        clickAction.Enable();

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
        Vector2 startPosition = isFromLeft
            ? new Vector2(-canvasWidth * 0.4f, 0)
            : new Vector2(canvasWidth * 0.4f, 0);

        spriteRect.anchoredPosition = startPosition;

        // Attach BeatSprite Component and Set Initial Values
        BeatSprite beatComponent = newBeatSprite.GetComponent<BeatSprite>();
        if (beatComponent != null)
        {
            beatComponent.InitializeCountdown(countdownStart, isFromLeft);
        }

        activeBeatSprites.Add(newBeatSprite);
    }

    private void HandleClickOnBeat()
    {
        if (activeBeatSprites.Count == 0) return;

        GameObject closestSprite = null;
        float closestDistance = float.MaxValue;
        float closestCountdown = float.MaxValue;

        // find nearest note
        foreach (GameObject sprite in activeBeatSprites)
        {
            BeatSprite beatComponent = sprite.GetComponent<BeatSprite>();
            if (beatComponent == null) continue;

            float distanceFromCenter = Vector2.Distance(sprite.GetComponent<RectTransform>().anchoredPosition, Vector2.zero);
            if (distanceFromCenter > judgmentRadius) continue;

            // ms
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

            // Determine judgment based on countdown in milliseconds
            string judgment = GetJudgment(currentCountdown);
            Debug.Log($"Click Judgment: {judgment} with Countdown: {currentCountdown}ms");

            closestSprite.SetActive(false);
            activeBeatSprites.Remove(closestSprite);
        }
    }

    private string GetJudgment(float countdown)
    {
        if (Mathf.Abs(countdown) <= perfectWindow && Mathf.Abs(countdown) > 0f ) return "Perfect";
        if (Mathf.Abs(countdown) <= greatWindow) return "Great";
        if (Mathf.Abs(countdown) <= goodWindow) return "Good";
        return "Miss";
    }

    private void OnDisable()
    {
        clickAction.Disable();
    }
}