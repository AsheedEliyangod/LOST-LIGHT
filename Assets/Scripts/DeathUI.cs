using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathUI : MonoBehaviour
{
    [Header("Death UI")]
    [SerializeField] private GameObject deathPanel;

    private bool restarting;

    private void Awake()
    {
        if (deathPanel != null)
        {
            deathPanel.SetActive(false);
        }
    }

    public void ShowDeathScreen()
    {
        if (restarting)
            return;

        if (deathPanel != null)
        {
            deathPanel.SetActive(true);
        }

        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        if (restarting)
            return;

        restarting = true;

        // Resume time first.
        Time.timeScale = 1f;

        // Hide persistent death UI.
        if (deathPanel != null)
        {
            deathPanel.SetActive(false);
        }

        // Reset persistent player.
        PlayerHealth playerHealth =
            FindAnyObjectByType<PlayerHealth>();

        if (playerHealth != null)
        {
            playerHealth.ResetForRestart();
        }

        // Reset lantern.
        LanternSystem lanternSystem =
            FindAnyObjectByType<LanternSystem>();

        if (lanternSystem != null)
        {
            lanternSystem.ResetForRestart();
        }

        // Reset combat.
        PlayerCombat playerCombat =
            FindAnyObjectByType<PlayerCombat>();

        if (playerCombat != null)
        {
            playerCombat.ResetForRestart();
        }

        int sceneIndex =
            SceneManager.GetActiveScene().buildIndex;

        Debug.Log(
            "Restarting: " +
            SceneManager.GetActiveScene().name
        );

        SceneManager.LoadScene(
            sceneIndex
        );

        restarting = false;
    }
}