using UnityEngine;

public class PlayerProgression : MonoBehaviour
{
    [Header("Level")]
    [SerializeField] private int currentLevel = 1;

    [Header("Experience")]
    [SerializeField] private int currentXP = 0;

    [SerializeField] private int baseXPRequired = 100;
    [SerializeField] private int xpIncreasePerLevel = 50;

    [Header("Ability System")]
    [SerializeField] private int abilityPointsOnLevel2 = 2;

    private PlayerAbilityUpgrade abilityUpgrade;

    public int CurrentLevel => currentLevel;
    public int CurrentXP => currentXP;

    public int XPRequiredForNextLevel =>
        baseXPRequired +
        ((currentLevel - 1) * xpIncreasePerLevel);

    private void Awake()
    {
        abilityUpgrade =
            GetComponent<PlayerAbilityUpgrade>();
    }

    public void AddXP(int amount)
    {
        if (amount <= 0)
            return;

        currentXP += amount;

        Debug.Log(
            "XP +" + amount +
            " | XP: " +
            currentXP +
            " / " +
            XPRequiredForNextLevel
        );

        CheckLevelUp();
    }

    private void CheckLevelUp()
    {
        while (currentXP >= XPRequiredForNextLevel)
        {
            int requiredXP =
                XPRequiredForNextLevel;

            currentXP -= requiredXP;

            currentLevel++;

            LevelUp();
        }
    }

    private void LevelUp()
    {
        Debug.Log(
            "LEVEL UP! Player is now Level " +
            currentLevel
        );

        // =========================
        // LEVEL 2 ABILITY UNLOCK
        // =========================

        if (currentLevel == 2)
        {
            UnlockAbilitySystem();

            AbilityUpgradeUI abilityUI =
                FindAnyObjectByType<AbilityUpgradeUI>();

            if (abilityUI != null)
            {
                abilityUI.OpenPanel();

                Debug.Log(
                    "Ability Upgrade Panel opened!"
                );
            }
            else
            {
                Debug.LogError(
                    "No AbilityUpgradeUI found in the scene!"
                );
            }
        }
    }

    private void UnlockAbilitySystem()
    {
        if (abilityUpgrade == null)
        {
            Debug.LogWarning(
                "PlayerAbilityUpgrade component is missing!"
            );

            return;
        }

        abilityUpgrade.UnlockAbilitySystem();

        abilityUpgrade.AddAbilityPoints(
            abilityPointsOnLevel2
        );

        Debug.Log(
            "Ability System unlocked! +" +
            abilityPointsOnLevel2 +
            " Ability Points."
        );
    }
}