using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatSprite : MonoBehaviour
{
    private RectTransform rectTransform;
    private float countdown;
    private float travelDuration;
    private bool isFromLeft;
    private Vector2 initialPosition;
    private Vector2 centerPosition;
    private bool isInitialized = false;

    public void InitializeCountdown(float startCountdown, bool fromLeft)
    {
        rectTransform = GetComponent<RectTransform>();
        countdown = startCountdown;
        travelDuration = startCountdown / 1000f; // Convert ms to seconds
        isFromLeft = fromLeft;
        centerPosition = Vector2.zero;
        initialPosition = rectTransform.anchoredPosition;

        // Flip the sprite if coming from the left
        rectTransform.localScale = isFromLeft ? new Vector3(-1, 1, 1) : new Vector3(1, 1, 1);

        StartCoroutine(MoveSpriteToCenter());
        isInitialized = true; 
    }

    private IEnumerator MoveSpriteToCenter()
    {
        while (countdown > 0)
        {
            countdown -= Time.deltaTime * 1000f; // countdown in ms
            float progress = 1 - (countdown / 2000f); // progress from 2000ms to 0ms
            rectTransform.anchoredPosition = Vector2.Lerp(initialPosition, centerPosition, progress);

            yield return null;
        }

        //deactivate if it reaches the center 
        gameObject.SetActive(false);
    }

    public float GetCurrentCountdown()
    {
        // countdown is only returned after initialization
        if (!isInitialized) return -1f;
        return countdown;
    }
}