using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectEffects : MonoBehaviour//, IObjectEffect
{
//    [Header("General Settings")]        
//    [SerializeField] private Beat _beat;
//
//    [SerializeField] private float _timeElapsed = 0;
//    [SerializeField] private float _duration = 5f;
//    [SerializeField] private AnimationCurve _lerpAnimCurve;
//    [SerializeField] private Coroutine coroutine;
//
//    [Header("Pulse Settings")]    
//    [SerializeField] float _pulseSize = 1.2f;    
//    private Vector3 _startSize;
//
//    [Header("Color Settings")]
//    [SerializeField] private Renderer __renderer; //double underscore indicates object must be preset        
//    [SerializeField] private Color _endColor;    
//    private Color _startColor;
//
//
//    private void OnEnable()
//    {        
//        _beat.OnBeat += this.Pulse;
//        _beat.OnBeat += this.ColorChange;        
//    }
//
//    private void OnDisable()
//    {
//        _beat.OnBeat -= this.Pulse;
//        _beat.OnBeat -= this.ColorChange;
//    }
//
//    void Start()
//    {
//        //Set Defaults
//        _startSize = transform.localScale;
//        _startColor = __renderer.material.color;        
//    }
//
//    // Update is called once per frame
//    void Update()
//    {
//        if (_timeElapsed < _duration)
//        {
//            float t = _timeElapsed / _duration;
//            t = _lerpAnimCurve.Evaluate(t);
//            transform.localScale = Vector3.Lerp(transform.localScale, _startSize, t);
//            __renderer.material.color = Color.Lerp(__renderer.material.color, _startColor, t);
//            _timeElapsed += Time.deltaTime;
//        }
//        else
//        {
//            transform.localScale = _startSize;
//            __renderer.material.color = _startColor;            
//        }
//    }
//
//    public void Pulse()
//    {
//        _timeElapsed = 0f;
//        transform.localScale = _startSize * _pulseSize;
//
//    }
//
//    public void ColorChange()
//    {
//        __renderer.material.color = _endColor;        
//    }
//
//    //Use this interface to pass all info and update information whenever the object is instantiated.
//    public void SetInfo(int id, Beat beat, bool active)
//    {
//        name = "Note: " + beat.GetSteps().ToString();
//        _beat = beat;
//        this.gameObject.SetActive(active);
//    }  
//}
//
//
//public interface IObjectEffect
//{    
//    void SetInfo(int id, Beat beat, bool active);
}