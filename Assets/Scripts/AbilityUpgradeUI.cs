using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AbilityUpgradeUI : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject panel;

    [Header("Ability Points")]
    [SerializeField] private TMP_Text abilityPointsText;

    [Header("Damage")]
    [SerializeField] private TMP_Text damageTitleText;
    [SerializeField] private TMP_Text damageDescriptionText;
    [SerializeField] private TMP_Text damageValueText;

    [Header("Range")]
    [SerializeField] private TMP_Text rangeTitleText;
    [SerializeField] private TMP_Text rangeDescriptionText;
    [SerializeField] private TMP_Text rangeValueText;

    [Header("Luminous Bolt")]
    [SerializeField] private TMP_Text luminousBoltTitleText;
    [SerializeField] private TMP_Text luminousBoltDescriptionText;
    [SerializeField] private TMP_Text luminousBoltValueText;

    [Header("Buttons")]
    [SerializeField] private Button damageButton;
    [SerializeField] private Button rangeButton;
    [SerializeField] private Button luminousBoltButton;

    private PlayerAbilityUpgrade abilityUpgrade;

    private void Start()
    {
        FindPlayerAbilityUpgrade();

        if (panel != null)
            panel.SetActive(false);

        UpdateUI();
    }

    private void Update()
    {
        if (abilityUpgrade == null)
        {
            FindPlayerAbilityUpgrade();
            return;
        }

        UpdateUI();
    }

    private void FindPlayerAbilityUpgrade()
    {
        abilityUpgrade =
            FindAnyObjectByType<PlayerAbilityUpgrade>();
    }

    public void OpenPanel()
    {
        if (abilityUpgrade == null)
            FindPlayerAbilityUpgrade();

        if (abilityUpgrade == null)
            return;

        if (!abilityUpgrade.AbilitySystemUnlocked)
        {
            Debug.Log(
                "Ability System is not unlocked yet."
            );

            return;
        }

        if (panel != null)
            panel.SetActive(true);

        UpdateUI();

        Time.timeScale = 0f;
    }

    public void ClosePanel()
    {
        if (panel != null)
            panel.SetActive(false);

        Time.timeScale = 1f;
    }

    public void UpgradeDamage()
    {
        if (abilityUpgrade == null)
            return;

        abilityUpgrade.UpgradeDamage();

        UpdateUI();
    }

    public void UpgradeRange()
    {
        if (abilityUpgrade == null)
            return;

        abilityUpgrade.UpgradeAttackRange();

        UpdateUI();
    }

    public void UnlockLuminousBolt()
    {
        if (abilityUpgrade == null)
            return;

        abilityUpgrade.UnlockLuminousBolt();

        UpdateUI();
    }

    private void UpdateUI()
    {
        if (abilityUpgrade == null)
            return;

        // =========================
        // ABILITY POINTS
        // =========================

        if (abilityPointsText != null)
        {
            abilityPointsText.text =
                "ABILITY POINTS: " +
                abilityUpgrade.AbilityPoints;
        }

        // =========================
        // DAMAGE
        // =========================

        if (damageTitleText != null)
        {
            damageTitleText.text =
                "UPGRADE DAMAGE";
        }

        if (damageDescriptionText != null)
        {
            damageDescriptionText.text =
                "Increase basic attack damage by +5.";
        }

        if (damageValueText != null)
        {
            damageValueText.text =
                abilityUpgrade.AttackDamage +
                " → " +
                (abilityUpgrade.AttackDamage + 5f) +
                " DAMAGE";
        }

        // =========================
        // RANGE
        // =========================

        if (rangeTitleText != null)
        {
            rangeTitleText.text =
                "UPGRADE RANGE";
        }

        if (rangeDescriptionText != null)
        {
            rangeDescriptionText.text =
                "Extend the reach of your basic attack.";
        }

        if (rangeValueText != null)
        {
            rangeValueText.text =
                abilityUpgrade.AttackRange.ToString("0.00") +
                " → " +
                (abilityUpgrade.AttackRange + 0.25f).ToString("0.00") +
                " RANGE";
        }

        // =========================
        // LUMINOUS BOLT
        // =========================

        if (luminousBoltTitleText != null)
        {
            luminousBoltTitleText.text =
                "LUMINOUS BOLT";
        }

        if (luminousBoltDescriptionText != null)
        {
            luminousBoltDescriptionText.text =
                "Unleash a ranged bolt of lantern energy.";
        }

        if (luminousBoltValueText != null)
        {
            if (abilityUpgrade.LuminousBoltUnlocked)
            {
                luminousBoltValueText.text =
                    "UNLOCKED";
            }
            else
            {
                luminousBoltValueText.text =
                    "COST: 1 AP";
            }
        }

        // =========================
        // BUTTON STATES
        // =========================

        bool hasPoints =
            abilityUpgrade.AbilityPoints > 0;

        if (damageButton != null)
        {
            damageButton.interactable =
                hasPoints;
        }

        if (rangeButton != null)
        {
            rangeButton.interactable =
                hasPoints;
        }

        if (luminousBoltButton != null)
        {
            luminousBoltButton.interactable =
                hasPoints &&
                !abilityUpgrade.LuminousBoltUnlocked;
        }
    }
}