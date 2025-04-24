using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine.InputSystem.XR;

public class DialogueSystem : MonoBehaviour
{
    public static DialogueSystem Instance;

    public GameObject dialoguePanel;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    public float typingSpeed = 0.03f;

    public UnityEvent OnDialogueStart;
    public UnityEvent OnDialogueEnd;

    private Queue<DialogueLine> dialogueQueue = new Queue<DialogueLine>();
    private DialogueLine currentLineData;
    private bool isTyping = false;
    private Coroutine typingCoroutine;

    private PlayerInput playerInput;
    private InputAction advanceAction;
    private InputAction fireAction;

    [SerializeField] FirstPersonController firstPersonController;
    [SerializeField] private DialogueData dialogueData;

    public void PlayDialogueLine(int index)
    {
        if (index < 0 || index >= dialogueData.dialogueLines.Length) return;

        DialogueLine line = dialogueData.dialogueLines[index];

        DialogueSystem.Instance.StartDialogue(new DialogueData
        {
            dialogueLines = new DialogueLine[] { line }
        });
    }

    void Awake()
    {
        firstPersonController = FindObjectOfType<FirstPersonController>();

        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        dialoguePanel.SetActive(false);
        playerInput = FindObjectOfType<PlayerInput>();

        fireAction = playerInput.actions["Fire"];
        fireAction.Disable();
    }

    public void StartDialogue(DialogueData data)
    {
        // disable input, switch maps, etc…
        dialogueQueue.Clear();
        foreach (var line in data.dialogueLines)
            dialogueQueue.Enqueue(line);

        dialoguePanel.SetActive(true);
        ShowNextLine();

        if (firstPersonController != null)
            firstPersonController.enabled = false;

        OnDialogueStart?.Invoke();
        playerInput.actions["Fire"].Disable();

        if (playerInput != null)
        {
            playerInput.SwitchCurrentActionMap("Dialogue");

            advanceAction = playerInput.actions["Advance"];
            advanceAction.started += OnAdvancePressed;
        }
    }

    private void ShowNextLine()
    {
        if (dialogueQueue.Count == 0)
        {
            EndDialogue();
            return;
        }

        currentLineData = dialogueQueue.Dequeue();
        nameText.text = currentLineData.speakerName;
        typingCoroutine = StartCoroutine(TypeLine(currentLineData.text));
    }

    IEnumerator TypeLine(string line)
    {
        isTyping = true;
        dialogueText.text = "";
        foreach (char c in line)
        {
            dialogueText.text += c;
            yield return new WaitForSecondsRealtime(typingSpeed);
        }
        isTyping = false;
    }

    private void OnAdvancePressed(InputAction.CallbackContext ctx)
    {
        if (!dialoguePanel.activeSelf) return;

        if (isTyping)
        {
            StopCoroutine(typingCoroutine);
            dialogueText.text = currentLineData.text; ;
            isTyping = false;
        }
        else
        {
            ShowNextLine();
        }
    }

    private void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        OnDialogueEnd?.Invoke();
        playerInput.actions["Fire"].Enable();

        // Remove input listener
        if (advanceAction != null)
            advanceAction.started -= OnAdvancePressed;

        // Switch back to gameplay input
        if (playerInput != null)
            playerInput.SwitchCurrentActionMap("Player");

        fireAction.Enable();
        if (firstPersonController != null)
            firstPersonController.enabled = true;

        OnDialogueEnd.RemoveAllListeners();
    }
}