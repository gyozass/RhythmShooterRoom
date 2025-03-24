using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class JumpPad : MonoBehaviour
{
    public float JumpForce = 10f;

    private void OnTriggerEnter(Collider other)
    {
        FirstPersonController playerController = other.GetComponent<FirstPersonController>();

        if (playerController != null)
        {
            playerController._verticalVelocity = Mathf.Sqrt(JumpForce * -2f * playerController.Gravity);

         //   playerController.directionalVector = transform.forward;
        }
    }
}

