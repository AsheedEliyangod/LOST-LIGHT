using UnityEngine;

public class NPCDialogue : MonoBehaviour
{
    [Header("NPC")]
    [SerializeField] private string npcName = "Lost Wanderer";

    [TextArea(2, 5)]
    [SerializeField] private string[] dialogueLines;

    [Header("Interaction")]
    [SerializeField] private GameObject interactionPrompt;

    private bool playerNearby;

    // Prevent the E press that closes dialogue
    // from immediately starting it again.
    private bool waitForERelease;

    private void Start()
    {
        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
    }

    private void Update()
    {
        if (!playerNearby)
            return;

        // Dialogue currently running.
        if (DialogueUI.Instance != null &&
            DialogueUI.Instance.IsDialogueActive)
        {
            waitForERelease = true;

            if (interactionPrompt != null)
                interactionPrompt.SetActive(false);

            return;
        }

        // Dialogue just finished.
        // Wait until player releases E before interaction works again.
        if (waitForERelease)
        {
            if (!Input.GetKey(KeyCode.E))
            {
                waitForERelease = false;

                if (interactionPrompt != null)
                    interactionPrompt.SetActive(true);
            }

            return;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            Talk();
        }
    }

    private void Talk()
    {
        if (DialogueUI.Instance == null)
            return;

        DialogueUI.Instance.StartDialogue(
            npcName,
            dialogueLines
        );

        waitForERelease = true;

        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerNearby = true;

        if (DialogueUI.Instance == null ||
            !DialogueUI.Instance.IsDialogueActive)
        {
            if (interactionPrompt != null)
                interactionPrompt.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerNearby = false;
        waitForERelease = false;

        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);

        if (DialogueUI.Instance != null &&
            DialogueUI.Instance.IsDialogueActive)
        {
            DialogueUI.Instance.EndDialogue();
        }
    }
}