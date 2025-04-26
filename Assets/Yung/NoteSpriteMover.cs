using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NoteSpriteMover : MonoBehaviour
{
 //  public static event Action OnReset;
 //  
 //  //[SerializeField] private Transform target;
 //  [SerializeField] private float _timeToReachTarget;
 //  [SerializeField] private AnimationCurve _animCurve;
 //  private float _t = 0f;
 //  private Vector2 _startPosition;
 //  private Vector2 _targetPosition;
 //  private Image image;
 //
 //  void Start()
 //  {
 //      _startPosition = transform.position;
 //      _targetPosition = new Vector2(Screen.width / 2, Screen.height / 2);
 //      image = GetComponent<Image>();
 //      Color color = new Color(image.color.r, image.color.g , image.color.b, 0f);
 //  }
 //
 //  void Update()
 //  {
 //      _t += Time.deltaTime / _timeToReachTarget;
 //       //_animCurve.Evaluate(_t);
 //      float targetAlpha =_t;
 //      image.color = new Color(1f, 1f, 1f, targetAlpha);
 //
 //      if (_t >= 0.95f)
 //      {
 //          ResetObject();  
 //      }
 //
 //      transform.position = Vector2.Lerp(_startPosition, _targetPosition, _t);
 //      Vector2 v2 = new Vector2(transform.position.x, transform.position.y);
 //
 //      if (v2 == _targetPosition)
 //      {
 //          ResetObject();
 //      }
 //  }
 //
 //  public void ResetObject()
 //  {
 //      Debug.Log(name + " reached");
 //      Color color = new Color(image.color.r, image.color.g, image.color.b, 0f);
 //      gameObject.SetActive(false);
 //      _t = 0;
 //      transform.position = _startPosition;
 //      OnReset();
 //
 //  }
 //
 //  public void SetDestination(Vector2 destination, float time)
 //  {
 //      _t = 0;
 //      _startPosition = transform.position;
 //      _timeToReachTarget = time;
 //      _targetPosition = destination;
 //  }
}
