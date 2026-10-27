using UnityEngine;

public class PlayerAbilityUpgrade : MonoBehaviour
{
    [Header("Ability System")]
    [SerializeField] private bool abilitySystemUnlocked = false;
    [SerializeField] private int abilityPoints = 0;

    [Header("Basic Attack")]
    [SerializeField] private float attackDamage = 25f;
    [SerializeField] private float attackRange = 0.8f;

    [Header("Upgrade Amounts")]
    [SerializeField] private float damageUpgradeAmount = 5f;
    [SerializeField] private float rangeUpgradeAmount = 0.25f;

    [Header("Magic Skill")]
    [SerializeField] private bool luminousBoltUnlocked = false;

    public bool AbilitySystemUnlocked => abilitySystemUnlocked;
    public int AbilityPoints => abilityPoints;

    public float AttackDamage => attackDamage;
    public float AttackRange => attackRange;

    public bool LuminousBoltUnlocked => luminousBoltUnlocked;

    // =========================
    // ABILITY SYSTEM
    // =========================

    public void UnlockAbilitySystem()
    {
        if (abilitySystemUnlocked)
            return;

        abilitySystemUnlocked = true;

        Debug.Log("Ability System unlocked!");
    }

    public void AddAbilityPoints(int amount)
    {
        if (amount <= 0)
            return;

        abilityPoints += amount;

        Debug.Log(
            "Ability Points +" +
            amount +
            " | Total: " +
            abilityPoints
        );
    }

    // =========================
    // BASIC ATTACK UPGRADES
    // =========================

    public void UpgradeDamage()
    {
        if (!abilitySystemUnlocked)
        {
            Debug.Log("Ability System is not unlocked yet!");
            return;
        }

        if (abilityPoints <= 0)
        {
            Debug.Log("Not enough Ability Points!");
            return;
        }

        abilityPoints--;

        attackDamage += damageUpgradeAmount;

        Debug.Log(
            "Attack Damage upgraded to: " +
            attackDamage +
            " | Ability Points: " +
            abilityPoints
        );
    }

    public void UpgradeAttackRange()
    {
        if (!abilitySystemUnlocked)
        {
            Debug.Log("Ability System is not unlocked yet!");
            return;
        }

        if (abilityPoints <= 0)
        {
            Debug.Log("Not enough Ability Points!");
            return;
        }

        abilityPoints--;

        attackRange += rangeUpgradeAmount;

        Debug.Log(
            "Attack Range upgraded to: " +
            attackRange +
            " | Ability Points: " +
            abilityPoints
        );
    }

    // =========================
    // LUMINOUS BOLT
    // =========================

    public void UnlockLuminousBolt()
    {
        if (!abilitySystemUnlocked)
        {
            Debug.Log("Ability System is not unlocked yet!");
            return;
        }

        if (luminousBoltUnlocked)
        {
            Debug.Log("Luminous Bolt is already unlocked!");
            return;
        }

        if (abilityPoints <= 0)
        {
            Debug.Log("Not enough Ability Points!");
            return;
        }

        abilityPoints--;

        luminousBoltUnlocked = true;

        Debug.Log(
            "Luminous Bolt unlocked! | Ability Points: " +
            abilityPoints
        );
    }
}