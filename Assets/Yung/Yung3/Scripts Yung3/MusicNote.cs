using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MusicNote : MonoBehaviour
{	
	// We keep the start and end positionX to perform interpolation.
	public float startX;
	public float endX;
	public float removeLineX;	
	public float tolerationOffset;
	public float currentOffset;
	public float beat;
	public bool alreadyDequeued = false;
	public MovingDirection movingDirection;
	[SerializeField] public MusicNote twinNote;
	[SerializeField] public Animator animator;
    [SerializeField] private TextMeshProUGUI textMeshProUGUI;
	[SerializeField] TimelineEvents _timelineEvents;

    public float _perfectPercentage = 0.1f;
	public float _goodPercentage = 0.2f;
	public float _okPercentage = 0.4f;
	public Color hitColor;
	public Color missColor;
	private Color defaultColor;
	[SerializeField] private SpriteRenderer spriteRenderer;
    //[SerializeField] private Image _image;

    private void Awake()
    {
		//gameObject.SetActive(false);
    }

    private void Start()
    {
		//defaultColor = spriteRenderer.color;
	}

    public void Initialize(float startX, float finishLineX, float removeLineX, float tolerationOffset, float posY, float beat, MovingDirection direction, MusicNote twinNote)
    {
		this.twinNote = twinNote;
	Initialize(startX, finishLineX, removeLineX, tolerationOffset, posY, beat, direction);
	}

	public void Initialize(float startX, float finishLineX, float removeLineX, float tolerationOffset, float posY, float beat, MovingDirection direction)
	{
		//this._advBeatManager = advBeatManager;
		this.startX = startX;
		this.endX = finishLineX;
		this.removeLineX = removeLineX;
		this.tolerationOffset = tolerationOffset;
		this.beat = beat;
		this.name += " " + beat;
		this.movingDirection = direction;
		
		if (movingDirection == MovingDirection.From_Left)
		{
			this.name += " Left";
			twinNote = null;
			this.GetComponentInChildren<Transform>().localRotation = Quaternion.Euler(0, 180, 0);			
		}
		else
		{
			this.name += " Right";
			this.GetComponentInChildren<Transform>().localRotation = Quaternion.identity;
			// The note is push into the queue for reference.
			AdvBeatManager.instance.notesOnScreen.Enqueue(this);
			AdvBeatManager.instance.musicNotesInQueue.Add(this);
		}


		//determines the percentage of hit type
		_perfectPercentage = tolerationOffset * _perfectPercentage;
		_goodPercentage = (tolerationOffset * _goodPercentage) + _perfectPercentage;
		_okPercentage = (tolerationOffset * _okPercentage) + _goodPercentage;

		if (AdvBeatManager.instance.showDebug && movingDirection == MovingDirection.From_Right) Debug.Log("I live " + this.name);

		// Set to initial position.
		transform.localPosition = new Vector2(startX, posY);

		if (AdvBeatManager.instance.showDebug && movingDirection == MovingDirection.From_Right) Debug.Log(this.name + " pos: " + transform.localPosition.x + " fin line: " + (endX - tolerationOffset));
		
		gameObject.SetActive(true);
	}

	void Update()
	{
		if (!AdvBeatManager.paused)
        {
			// We update the position of the note according to the position of the song.
			// (Think of this as "resetting" instead of "updating" the position of the note each frame according to the position of the song.)
			// See this image: http://shinerightstudio.com/posts/music-syncing-in-rhythm-games/pic3.png (Note that the direction is reversed.)
			
			transform.localPosition = new Vector2(startX + (endX - startX) * (1f - (beat - AdvBeatManager.instance.songPosition / AdvBeatManager.instance.secPerBeat) / AdvBeatManager.instance.BeatsShownOnScreen), transform.localPosition.y);
			
			if (AdvBeatManager.instance.showDebug && movingDirection == MovingDirection.From_Right) Debug.Log("Beat: " + beat + " Xpos: " + transform.localPosition.x + " Topbeat: " + AdvBeatManager.instance.notesOnScreen.Peek().beat);
			//Check if this beat has reached Finish Line
						
			if (movingDirection == MovingDirection.From_Right)
            {
				//if (AdvBeatManager.instance.showDebug && movingDirection == MovingDirection.From_Right) Debug.Log("HALLOO" + this.name + " pos: " + transform.localPosition.x + " fin line: " + (endX - tolerationOffset));
				if (transform.localPosition.x <= endX - tolerationOffset)
				{					
					ReachedFinishLine();
				}
            }

			// Remove itself when out of the screen (remove line).
			if (movingDirection == MovingDirection.From_Right)
			{
				if (transform.localPosition.x < removeLineX)
				{
					if (AdvBeatManager.instance.showDebug && movingDirection == MovingDirection.From_Right) Debug.Log("Passed remove line. Beat " + beat);					
					RemoveNote_FromMainQueue();
					DeleteNote();
				}
			}
		}		
	}

	public void ReachedFinishLine()
    {
		if (AdvBeatManager.instance.showDebug && movingDirection == MovingDirection.From_Right) Debug.Log("Passed finish line (with tolerance). Beat " + beat);
		// Change color to red to indicate a miss.

		if (!alreadyDequeued)
        {
			ChangeColor(false);
			twinNote.ChangeColor(false);
		}		

		RemoveNote_FromMainQueue();

	}

	public void RemoveNote_FromMainQueue()
    {
		//AdvBeatManager.instance._debugText.text += "\nRemove Beat " + beat;
		/*Debug.Log("I'm deleted. Beat " + beat);
		MusicNote temp = AdvBeatManager.instance.notesOnScreen.Peek();
		Debug.Log("Latest beat is Beat" + temp.beat);*/

		if (!alreadyDequeued) //triggered by click
		{
			alreadyDequeued = true;

			if (AdvBeatManager.instance.notesOnScreen.Peek() == this)
			{
				if (AdvBeatManager.instance.showDebug && movingDirection == MovingDirection.From_Right) Debug.Log("This beat missed the Finish Line - Beat " + beat);
				AdvBeatManager.instance.notesOnScreen.Dequeue();
				//MusicNote removedNote = AdvBeatManager.instance.notesOnScreen.Dequeue();
				//if (AdvBeatManager.instance.showDebug && movingDirection == MovingDirection.From_Right) Debug.Log(removedNote.beat);
				AdvBeatManager.instance.musicNotesInQueue.Remove(this);
			}
		}
		else
		{
			if (AdvBeatManager.instance.showDebug && movingDirection == MovingDirection.From_Right) Debug.Log("This already missed finish line - Beat " + beat);			
		}		
	}

	public void DeleteNote()
    {
		if (twinNote != null)
        {
			if (twinNote.twinNote != null)
            {
				Debug.LogError("What the f");
				Debug.Break();
            }
			twinNote.DeleteNote();
		}
		Destroy(gameObject);
		/*gameObject.SetActive(false);
		spriteRenderer.color = defaultColor;
		twinNote = null;
		alreadyDequeued = false;
*/
	}

	public HitResult CheckIfHit()
	{
		float offset = Mathf.Abs(transform.localPosition.x - endX);
		HitType type;

		if (offset <= tolerationOffset)
		{
			if (offset < _perfectPercentage) type = HitType.Perfect;
			else if (offset < _goodPercentage) type = HitType.Good;
			else if (offset < _okPercentage) type = HitType.Okay;
			else type = HitType.Miss;
		}
		else
		{
			type = HitType.Miss;
		}

		RemoveNote_FromMainQueue();
		return new HitResult { offset = offset, type = type };

		// Play the beat sound.
		// AdvBeatManager.instance._beatAudioSource.Play();

	}

	// Change the color to indicate whether its a "HIT" or a "MISS".
	public void ChangeColor(bool hit)
	{
		if (hit)
		{			
			spriteRenderer.color = hitColor;			
		}
		else
		{
			spriteRenderer.color = missColor;			
		}
	}
}