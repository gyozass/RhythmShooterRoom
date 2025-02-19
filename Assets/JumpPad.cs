using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class JumpPad : MonoBehaviour
{
    [Tooltip("The force applied to the player when they touch the jump pad")]
    public float JumpForce = 10f;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the trigger has a FirstPersonController component
        FirstPersonController playerController = other.GetComponent<FirstPersonController>();

        if (playerController != null)
        {
            // Apply jump force to the player's vertical velocity
            playerController._verticalVelocity = Mathf.Sqrt(JumpForce * -2f * playerController.Gravity);
        }
    }
}

