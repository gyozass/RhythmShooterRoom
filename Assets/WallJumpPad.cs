using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallJumpPad : MonoBehaviour
{
    [SerializeField] private float WallJumpForce = 10f;
    [SerializeField] private float UpwardForce = 5f;
    [SerializeField] private AnimationCurve ForceCurve;
    [SerializeField] private float JumpDuration = 0.5f;
    [SerializeField] private Transform JumpPadVisual;
    [SerializeField] private float PulseScaleMultiplier = 1.2f;
    [SerializeField] private float PulseDuration = 0.2f;

    private IEnumerator ApplyWallJumpForce(CharacterController controller, Vector3 direction)
    {
        float elapsedTime = 0f;
        while (elapsedTime < JumpDuration)
        {
            float forceMultiplier = ForceCurve.Evaluate(elapsedTime / JumpDuration);
            Vector3 jumpForce = (direction + Vector3.up * UpwardForce).normalized * WallJumpForce * forceMultiplier;
            controller.Move(jumpForce * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
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

    private void OnTriggerEnter(Collider other)
    {
        FirstPersonController playerController = other.GetComponent<FirstPersonController>();
        if (playerController != null)
        {
            Vector3 wallJumpDirection = (transform.forward + Vector3.up * 0.5f).normalized; // Slight upward boost
            playerController._verticalVelocity = 0f; // Reset vertical velocity to prevent interference
            StartCoroutine(ApplyWallJumpForce(other.GetComponent<CharacterController>(), wallJumpDirection));
            StartCoroutine(PulseJumpPad());
        }
    }
}