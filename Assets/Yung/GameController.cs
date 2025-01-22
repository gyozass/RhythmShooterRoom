using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEngine.EventSystems;


public class GameController : MonoBehaviour
{
    [SerializeField] private InputSystem_Actions _inputSystem_Actions;
    //[SerializeField] private Rigidbody _rb;    
    
    public InputActionReference fire;
    public static event Action OnLastClick;

    private void OnEnable()
    {
        fire.action.started += Fire;
    }

    private void OnDisable()
    {
        fire.action.started -= Fire;
    }

    private void Start()
    {        
        _inputSystem_Actions = new InputSystem_Actions();        
    }

    //record the last saved time on click
    public void Fire(InputAction.CallbackContext context)
    {   
        OnLastClick?.Invoke();
    }    
}