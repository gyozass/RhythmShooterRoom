using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Security.Cryptography;

public enum MovingDirection { From_Left, From_Right };

public class AdvBeatManager : MonoBehaviour
{
    [Header("Debug")]
    //Debug Canvas
    public bool showDebug = false;
    public bool showDebugPanel = false;
    public TextMeshProUGUI _debugText;

    //Static Song Info
    [Header("Configurable")]
    [SerializeField] private AudioSource _songAudioSource;
    public AudioSource _beatAudioSource;
    public float BeatsShownOnScreen = 4f;
    [SerializeField] private int bpm;
    [SerializeField] private float songOffset;
    [SerializeField] private bool autoGenerateTrack = true;
    [SerializeField] private float autoTrackInterval = 1;
    [SerializeField] private int autoTrackSize = 16;
    public float[] track;  //keep all the position-in-beats of notes in the song
    [SerializeField] private float posY;
    [SerializeField] private float startLineX;
    [SerializeField] private float finishLineX;   // The finish line (the positionX where players hit) of the notes.
    [SerializeField] private float removeLineX;   // The positionX where the note should be destroyed.    
    [SerializeField] private float tolerationOffset;  // The position offest of toleration. (If the players hit slightly inaccurate for the music note, we tolerate them and count it as a successful hit.)    
    [SerializeField] private GameObject _notePrefab;
    [SerializeField] private Transform _noteSpawnLocation;    
    
    //announcement
    public TextMeshPro announcementText;
    public Animator announcementAnimator;

    //Dynamic Song info
    [Header("Runtime")]
    [SerializeField] private bool _musicStarted = false;
    public float secPerBeat;    //the duration of a beat      secondsPerBeat                      
    public float songPosition;    //the current position of the song (in seconds)
    public float songPosInBeats;    //the current position of the song (in beats)    
    public float dspSongTime;    //how much time (in seconds) has passed since the song started

    //public float[] notes; 
    public Queue<MusicNote> notesOnScreen;
    public List<MusicNote> musicNotesInQueue;
    private float[] _defaultTrackValues;
    private int nextIndex = 0;    
    private int _loopedTimes = 0;

    //Pausing
    public static bool paused = false;
    public static float pauseTimeStamp = -1f;
    public static float pausedTime = 0;
        
    //Instance
    public static AdvBeatManager instance;

    [SerializeField] private TextMeshProUGUI hitText;
    [SerializeField] private Animator noteAnimator;    
    [SerializeField] private TimelineEvents timelineEvents;

    [SerializeField] MusicNote musicNote;


    private void Awake()
    {
        instance = this;
    }

    private void OnEnable()
    {
        GameController.OnLastClick += PlayerInputted;
    }

    private void OnDisable()
    {
        GameController.OnLastClick -= PlayerInputted;
    }

    void Start()
    {

        if (autoGenerateTrack)
        {
            AutoGenerateTrack();
        }

        //arrays get referenced so need to clone them to be independent.
        _defaultTrackValues = new float[track.Length];
        _defaultTrackValues = (float[]) track.Clone();

        // Initialize some variables.
        notesOnScreen = new Queue<MusicNote>();
        nextIndex = 0;

        StartCoroutine(CountDown());

    }

    void AutoGenerateTrack()
    {
        track = new float[autoTrackSize];        
        for (int i = 0; i < track.Length; i++)
        {
            track[i] = autoTrackInterval * (i + 1);
        }
    }


    IEnumerator CountDown()
    {
        yield return new WaitForSeconds(1f);
        announcementAnimator.SetTrigger("getReady");
        for (int i = 0; i < 1; i++)
        {
            if (showDebugPanel) Debug.Log("Countdown " + i.ToString());
            yield return new WaitForSeconds(3f);
        }
        
        StartSong();
    }

    void StartSong()
    {
        //calculate how many seconds is one beat
        //we will see the declaration of bpm later
        secPerBeat = 60f / bpm;

        //record the time when the song starts
        dspSongTime = (float)AudioSettings.dspTime;

        if(showDebugPanel) _debugText.text = "Music Started";        
        //_songAudioSource.Play();
        _musicStarted = true;

    }

    // Update is called once per frame
    void Update()
    {
        // Check key press.
        /*if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            PlayerInputted();
        }*/

        if (!_musicStarted) return;         
                
        if (paused)
        {
            if (pauseTimeStamp < 0f) //not managed
            {
                pauseTimeStamp = (float)AudioSettings.dspTime;
                AudioListener.pause = true;
                //Activate some UI here
                //"Your Pause Canvas".SetActive(true);
                if (showDebugPanel) _debugText.text = "\nPaused";
            }
            return;
        }
        else if (pauseTimeStamp > 0f)
        {
            AudioListener.pause = false;
            pauseTimeStamp = -1f;
        }

        //calculate the position in seconds
        songPosition = (float)(AudioSettings.dspTime - dspSongTime - pausedTime - songOffset);

        songPosInBeats = songPosition / secPerBeat;

        float beatToShow = songPosition / secPerBeat + BeatsShownOnScreen;

        //Debug.Log("next Index " + nextIndex + " / " + track.Length);
        //Debug.Log("song position " + songPosition + " next index " + nextIndex + " / " + track.Length + " song in queue " + notesOnScreen.Count + " beats show " + beatToShow + " / " + track[nextIndex]);
        if (showDebug) Debug.Log(" next index " + nextIndex + " / " + track.Length + " song in queue " + notesOnScreen.Count + " Song in Beat Pos: " + songPosInBeats);
        
        if (track[nextIndex] < beatToShow)
        {
            if (showDebug) Debug.Log("INDEX IS LESS THAN BEAT " + "track index " + track[nextIndex] + " beats show " + beatToShow);
        }
        else
        {
            if (showDebug) Debug.Log("INDEX IS MORE THAN BEAT " + "track index " + track[nextIndex] + " beats show " + beatToShow);
            //Debug.Break();
        }
                
        if (nextIndex < track.Length && track[nextIndex] < beatToShow)
        //if (nextIndex < track.Length)
        {

            if (showDebug) Debug.Log("Hi");

            // Instantiate a new music note. (Search "Object Pooling" for more information if you wish to minimize the delay when instantiating game objects.)
            // We don't care about the position and rotation because we will set them later in MusicNote.Initialize(...).
            
            //ori// MusicNote musicNote = ((GameObject)Instantiate(_notePrefab, _noteSpawnLocation.position, _noteSpawnLocation.rotation, _noteSpawnLocation)).GetComponent<MusicNote>();
            MusicNote musicNoteRight = ((GameObject)Instantiate(_notePrefab, _noteSpawnLocation.position, _noteSpawnLocation.rotation, _noteSpawnLocation)).GetComponent<MusicNote>();
            MusicNote musicNoteLeft = ((GameObject)Instantiate(_notePrefab, _noteSpawnLocation.position, _noteSpawnLocation.rotation, _noteSpawnLocation)).GetComponent<MusicNote>();

            if (showDebug) Debug.Log("I'm in - you're alright? " + musicNoteRight.beat);
                        
            musicNoteLeft.Initialize(-startLineX, -finishLineX, -removeLineX, tolerationOffset, posY, track[nextIndex],MovingDirection.From_Left);
            musicNoteRight.Initialize(startLineX, finishLineX, removeLineX, tolerationOffset, posY, track[nextIndex], MovingDirection.From_Right,musicNoteLeft);

            nextIndex++;

            if (showDebug) Debug.Log("note Count" + notesOnScreen.Count);
            //Debug.Log(" tracknextindex " + track[nextIndex]);
            if (showDebug) Debug.Log("next Index " + nextIndex + " / " + track.Length);

            //Debug.Log("song position " + songPosition + " next index " + nextIndex + " / " + track.Length + " song in queue " + notesOnScreen.Count + " beats show " + beatToShow + " / " + track[nextIndex]);

            //end of Notes List (Tracks reached the last note)
            if (nextIndex == track.Length)
            {
                if (showDebug) Debug.Log("Okay!!!!");
                nextIndex = 0;  //reset index                
                _loopedTimes++; //increase loop count
                
                for (int i = 0; i < track.Length; i++)
                {                    
                    track[i] = (track.Length * _loopedTimes) + _defaultTrackValues[i];
                    if (showDebug) Debug.Log("Reset Loop: Index " + i + " BeatValue " + track[i] + " loop times: " + _loopedTimes);
                }
                if (showDebug) Debug.Log("Resetting Index!!!!");
            }

            //Debug.Log("song position " + songPosition + " next index " + nextIndex + " / " + track.Length + " song in queue " + notesOnScreen.Count + " beats show " + beatToShow + " / " + track[nextIndex]);
        }
        else
        {
            if (showDebug) Debug.Log("NOPE - " + "next Index " + nextIndex + " / " + track.Length);
        }

        // Loop the queue to check if any of them reaches the finish line.
        /*if (notesOnScreen.Count > 0)
        {   
            MusicNote currNote = notesOnScreen.Peek();            
            Debug.Log("Total Notes on Screen = " + notesOnScreen.Count + "Top Note = " + currNote.beat + " posX at: " + currNote.transform.localPosition.x);
            
            if (currNote.transform.localPosition.x <= finishLineX - tolerationOffset)
            {
                //Debug.Log("Passed finish line. Beat " + currNote.beat);
                // Change color to red to indicate a miss.
                currNote.ChangeColor(false);

                notesOnScreen.Dequeue();                

                //_debugText.text += "\nDidn't Click for Beat " + currNote.beat;
            }
        }*/

        if (showDebug) Debug.Log("==End of update== Top Note is" + notesOnScreen.Peek().beat);
    }

    void PlayerInputted()
    {
        if (!_musicStarted) return;
        if (notesOnScreen.Count == 0) return;

        // 1) Pop the next note
        MusicNote note = notesOnScreen.Peek();
        HitResult result = note.CheckIfHit();

        // 2) Update your debug UI
        if (showDebugPanel)
        {
            _debugText.text +=
              $"\nClicked at {note.beat} with offset {result.offset:F2}";
            _debugText.text +=
              result.type == HitType.Miss
                ? $"\n<color=red>MISS</color> Beat {note.beat}"
                : $"\n<color=green>{result.type.ToString().ToUpper()}</color> Beat {note.beat}";
        }

        // 3) Change colors & trigger animations on the note(s)
        bool didHit = result.type != HitType.Miss;
        note.ChangeColor(didHit);
        if (note.twinNote != null)
            note.twinNote.ChangeColor(didHit);

        note.animator.SetTrigger(didHit ? "arrowHit" : "arrowMiss");
        if (note.twinNote != null)
            note.twinNote.animator.SetTrigger(didHit ? "arrowHit" : "arrowMiss");

        // 4) Update on‑screen hit text
        hitText.text = result.type.ToString().ToLower();

        // 5) Fire your Timeline event
        timelineEvents.OnPlayerShot(result.type.ToString());

        // 6) Store offset for PlayerShooting
        musicNote.currentOffset = result.offset;
    }

}