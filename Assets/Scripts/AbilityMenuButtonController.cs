
using UnityEngine;

public class AbilityMenuButtonController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject abilityMenuButton;
    [SerializeField] private AbilityUpgradeUI abilityUpgradeUI;

    private PlayerAbilityUpgrade abilityUpgrade;

    private void Start()
    {
        FindReferences();
        RefreshButtonVisibility();
    }

    private void Update()
    {
        if (abilityUpgrade == null)
            FindReferences();

        if (abilityUpgrade == null || abilityMenuButton == null)
            return;

        bool unlocked = abilityUpgrade.AbilitySystemUnlocked;

        if (abilityMenuButton.activeSelf != unlocked)
            abilityMenuButton.SetActive(unlocked);
    }

    private void FindReferences()
    {
        if (abilityUpgrade == null)
            abilityUpgrade = FindAnyObjectByType<PlayerAbilityUpgrade>();

        if (abilityUpgradeUI == null)
            abilityUpgradeUI = FindAnyObjectByType<AbilityUpgradeUI>();
    }

    private void RefreshButtonVisibility()
    {
        if (abilityMenuButton != null)
        {
            abilityMenuButton.SetActive(
                abilityUpgrade != null &&
                abilityUpgrade.AbilitySystemUnlocked
            );
        }
    }

    public void OpenAbilityPanel()
    {
        FindReferences();

        if (abilityUpgrade == null ||
            !abilityUpgrade.AbilitySystemUnlocked)
            return;

        if (abilityUpgradeUI != null)
            abilityUpgradeUI.OpenPanel();
        else
            Debug.LogWarning("AbilityUpgradeUI was not found.");
    }
}
