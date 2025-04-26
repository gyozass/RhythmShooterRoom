using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class DoorOpen : MonoBehaviour
{
    public Transform doorLeft;
    public Transform doorRight;

    public float slideDistance = 3f; // How far each door should slide
    public float slideSpeed = 3f;    // How fast doors should slide
    public string triggeringTag = "Player";

    private Vector3 leftStartPos;
    private Vector3 rightStartPos;
    private Vector3 leftTargetPos;
    private Vector3 rightTargetPos;

    private bool isOpening = false;

    [SerializeField] private PlayableDirector director;
    private bool hasTriggered = false;

    void Start()
    {
        leftStartPos = doorLeft.position;
        rightStartPos = doorRight.position;

        leftTargetPos = leftStartPos + Vector3.forward * slideDistance;
        rightTargetPos = rightStartPos + -Vector3.forward * slideDistance;
        Debug.Log("dist! " + leftTargetPos);
    }

    void Update()
    {
        if (isOpening)
        {
            doorLeft.position = Vector3.MoveTowards(doorLeft.position, leftTargetPos, Time.deltaTime * slideSpeed);
            Debug.Log("dist! " + leftTargetPos);
            Debug.Log("dist! " + doorLeft.position);
            doorRight.position = Vector3.MoveTowards(doorRight.position, rightTargetPos, Time.deltaTime * slideSpeed);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;

        if (other.CompareTag(triggeringTag))
        {
            hasTriggered = true;
            Debug.Log("Triggered by player, resuming timeline");

            if (director != null)
            {
                FindObjectOfType<TimelineEvents>().OnDoorTriggered();
            }
            else
            {
                Debug.LogWarning("PlayableDirector not assigned!");
            }

            isOpening = true;
        }
    }
}
