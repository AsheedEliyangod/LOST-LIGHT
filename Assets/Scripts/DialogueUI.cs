using System.Collections;
using TMPro;
using UnityEngine;

public class DialogueUI : MonoBehaviour
{
    public static DialogueUI Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text npcNameText;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private TMP_Text continueText;

    private string[] currentDialogue;
    private int currentLine;

    private bool canAdvance;

    public bool IsDialogueActive { get; private set; }

    private void Awake()
    {
        Instance = this;

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
    }

    private void Update()
    {
        if (!IsDialogueActive)
            return;

        // Prevent the same E press that opened the dialogue
        // from instantly skipping the first line.
        if (!canAdvance)
            return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            NextLine();
        }
    }

    public void StartDialogue(string npcName, string[] dialogue)
    {
        if (dialogue == null || dialogue.Length == 0)
            return;

        currentDialogue = dialogue;
        currentLine = 0;

        npcNameText.text = npcName;
        dialogueText.text = currentDialogue[currentLine];

        if (continueText != null)
            continueText.text = "[E] Continue";

        dialoguePanel.SetActive(true);

        IsDialogueActive = true;

        // Temporarily block E so the opening press is ignored.
        canAdvance = false;
        StartCoroutine(EnableDialogueInput());
    }

    private IEnumerator EnableDialogueInput()
    {
        // Wait until the next frame.
        yield return null;

        canAdvance = true;
    }

    private void NextLine()
    {
        currentLine++;

        if (currentLine >= currentDialogue.Length)
        {
            EndDialogue();
            return;
        }

        dialogueText.text = currentDialogue[currentLine];
    }

    public void EndDialogue()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        IsDialogueActive = false;
        canAdvance = false;

        currentDialogue = null;
        currentLine = 0;
    }
}