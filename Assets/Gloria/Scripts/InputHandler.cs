using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    [SerializeField] private InputActionReference fire;

    public UnityEvent OnFire;

    private void OnEnable()
    {
        fire.action.performed += PerformAttack;
    }

    private void OnDisable()
    {
        fire.action.performed -= PerformAttack;
    }

    private void PerformAttack(InputAction.CallbackContext obj)
    {
        OnFire?.Invoke();
    }
}
