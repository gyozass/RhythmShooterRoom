using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class JumpPad : MonoBehaviour
{
    public float JumpForce = 10f; 
    [SerializeField] private Transform JumpPadVisual;
    [SerializeField] private float PulseScaleMultiplier = 1.2f;
    [SerializeField] private float PulseDuration = 0.2f;

    private void OnTriggerEnter(Collider other)
    {
        FirstPersonController playerController = other.GetComponent<FirstPersonController>();

        if (playerController != null)
        {
            playerController._verticalVelocity = Mathf.Sqrt(JumpForce * -2f * playerController.Gravity);
            StartCoroutine(PulseJumpPad());
            //   playerController.directionalVector = transform.forward;
        }
    }
    private IEnumerator PulseJumpPad()
    {
        Vector3 originalScale = JumpPadVisual.localScale;
        Vector3 targetScale = originalScale * PulseScaleMultiplier;
        float elapsedTime = 0f;

        while (elapsedTime < PulseDuration)
        {
            JumpPadVisual.localScale = Vector3.Lerp(originalScale, targetScale, elapsedTime / PulseDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        elapsedTime = 0f;
        while (elapsedTime < PulseDuration)
        {
            JumpPadVisual.localScale = Vector3.Lerp(targetScale, originalScale, elapsedTime / PulseDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}

