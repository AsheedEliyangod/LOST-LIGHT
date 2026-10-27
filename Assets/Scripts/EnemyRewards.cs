using UnityEngine;

public class EnemyRewards : MonoBehaviour
{
    [Header("Reward Toggles")]
    [SerializeField] private bool giveHealthReward = true;
    [SerializeField] private bool giveLanternReward = true;
    [SerializeField] private bool giveXPReward = true;

    [Header("Reward Amounts")]
    [SerializeField] private float healthReward = 10f;
    [SerializeField] private float lanternEnergyReward = 15f;
    [SerializeField] private int xpReward = 20;

    private bool rewardsGiven;

    public void GiveRewards()
    {
        if (rewardsGiven)
            return;

        rewardsGiven = true;

        PlayerHealth playerHealth =
            FindAnyObjectByType<PlayerHealth>();

        LanternSystem lanternSystem =
            FindAnyObjectByType<LanternSystem>();

        PlayerProgression progression =
            FindAnyObjectByType<PlayerProgression>();

        RewardFeedbackUI rewardUI =
            FindAnyObjectByType<RewardFeedbackUI>();

        float actualHealthReceived = 0f;
        float actualLanternReceived = 0f;
        int actualXPReceived = 0;

        // =========================
        // HEALTH
        // =========================
        if (giveHealthReward &&
            playerHealth != null)
        {
            actualHealthReceived =
                playerHealth.Heal(
                    healthReward
                );
        }

        // =========================
        // LANTERN ENERGY
        // =========================
        if (giveLanternReward &&
            lanternSystem != null)
        {
            actualLanternReceived =
                lanternSystem.RechargeEnergy(
                    lanternEnergyReward
                );
        }

        // =========================
        // XP
        // =========================
        if (giveXPReward &&
            progression != null)
        {
            actualXPReceived =
                Mathf.Max(
                    0,
                    xpReward
                );

            progression.AddXP(
                actualXPReceived
            );
        }

        // =========================
        // HUD FEEDBACK
        // =========================
        if (rewardUI != null)
        {
            rewardUI.ShowRewards(
                actualHealthReceived,
                actualLanternReceived,
                actualXPReceived
            );
        }

        Debug.Log(
            "Enemy defeated! Rewards: +" +
            actualHealthReceived +
            " HP, +" +
            actualLanternReceived +
            " Lantern, +" +
            actualXPReceived +
            " XP."
        );
    }
}