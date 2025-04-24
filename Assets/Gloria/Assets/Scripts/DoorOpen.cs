using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    void Start()
    {
        leftStartPos = doorLeft.position;
        rightStartPos = doorRight.position;

        leftTargetPos = leftStartPos + Vector3.left * slideDistance;
        rightTargetPos = rightStartPos + Vector3.right * slideDistance;
    }

    void Update()
    {
        if (isOpening)
        {
            doorLeft.position = Vector3.MoveTowards(doorLeft.position, leftTargetPos, Time.deltaTime * slideSpeed);
            doorRight.position = Vector3.MoveTowards(doorRight.position, rightTargetPos, Time.deltaTime * slideSpeed);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(triggeringTag))
        {
            isOpening = true;
        }
    }
}
