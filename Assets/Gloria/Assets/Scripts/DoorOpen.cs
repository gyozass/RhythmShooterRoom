using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class DoorOpen : MonoBehaviour
{
    public Transform doorLeft;
    public Transform doorRight;
    public float slideDistance = 3f;
    public float slideSpeed = 3f;
    public string triggeringTag = "Player";

    private Vector3 leftStartPos;
    private Vector3 rightStartPos;
    private Vector3 leftTargetPos;
    private Vector3 rightTargetPos;

    private bool isOpening = false;
    public bool hasTriggered = false;

    [SerializeField] private PlayableDirector director;

    void Start()
    {
        leftStartPos = doorLeft.position;
        rightStartPos = doorRight.position;

        leftTargetPos = leftStartPos + Vector3.forward * slideDistance;
        rightTargetPos = rightStartPos + -Vector3.forward * slideDistance;
    }

    void Update()
    {
        if (isOpening)
        {
            doorLeft.position = Vector3.MoveTowards(doorLeft.position, leftTargetPos, Time.deltaTime * slideSpeed);
            doorRight.position = Vector3.MoveTowards(doorRight.position, rightTargetPos, Time.deltaTime * slideSpeed);

            if (Vector3.Distance(doorLeft.position, leftTargetPos) < 0.01f &&
                Vector3.Distance(doorRight.position, rightTargetPos) < 0.01f)
            {
                isOpening = false;
                Debug.Log("Doors fully opened!");
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;

        if (other.CompareTag(triggeringTag))
        {
            hasTriggered = true;
            Debug.Log("Triggered by player, opening doors and resuming timeline!");

            if (director != null)
            {
                if (director.state == PlayState.Paused)
                    director.Resume();
                else
                    director.Play();
            }
            else
            {
                Debug.LogWarning("PlayableDirector not assigned!");
            }

            isOpening = true;
        }
    }
}