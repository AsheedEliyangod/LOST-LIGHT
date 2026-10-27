using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerXPUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerProgression playerProgression;
    [SerializeField] private Image xpFill;
    [SerializeField] private TMP_Text levelText;

    private void Start()
    {
        if (playerProgression == null)
        {
            playerProgression =
                FindAnyObjectByType<PlayerProgression>();
        }

        UpdateXPUI();
    }

    private void Update()
    {
        UpdateXPUI();
    }

    private void UpdateXPUI()
    {
        if (playerProgression == null)
            return;

        // UPDATE XP BAR
        if (xpFill != null)
        {
            float xpPercent =
                (float)playerProgression.CurrentXP /
                playerProgression.XPRequiredForNextLevel;

            xpFill.fillAmount =
                Mathf.Clamp01(xpPercent);
        }

        // UPDATE LEVEL TEXT
        if (levelText != null)
        {
            levelText.text =
                "LV " +
                playerProgression.CurrentLevel;
        }
    }
}