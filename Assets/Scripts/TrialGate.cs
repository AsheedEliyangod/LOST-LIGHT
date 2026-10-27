using UnityEngine;
using UnityEngine.SceneManagement;

public class TrialGate : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject trialGatePanel;

    [Header("Scene")]
    [SerializeField] private string trialSceneName =
        "CombatTrialScene_01";

    private bool playerInRange;
    private bool promptOpen;

    private void Update()
    {
        if (!playerInRange)
            return;

        if (!promptOpen &&
            Input.GetKeyDown(KeyCode.E))
        {
            OpenPrompt();
        }

        if (promptOpen &&
            Input.GetKeyDown(KeyCode.Escape))
        {
            DeclineTrial();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInRange = false;

        if (promptOpen)
        {
            DeclineTrial();
        }
    }

    private void OpenPrompt()
    {
        promptOpen = true;

        if (trialGatePanel != null)
        {
            trialGatePanel.SetActive(true);
        }

        // Freeze gameplay while player chooses.
        Time.timeScale = 0f;
    }

    public void AcceptTrial()
    {
        if (!promptOpen)
            return;

        Time.timeScale = 1f;

        SceneManager.LoadScene(
            trialSceneName
        );
    }

    public void DeclineTrial()
    {
        promptOpen = false;

        if (trialGatePanel != null)
        {
            trialGatePanel.SetActive(false);
        }

        Time.timeScale = 1f;
    }

    private void OnDisable()
    {
        // Safety in case object is disabled while prompt is open.
        if (Time.timeScale == 0f)
        {
            Time.timeScale = 1f;
        }
    }
}