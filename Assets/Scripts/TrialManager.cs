using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrialManager : MonoBehaviour
{
    [System.Serializable]
    public class TrialWave
    {
        [Header("Wave Settings")]
        [Min(1)]
        public int enemyCount = 1;

        [Min(0f)]
        public float spawnDelay = 0.8f;

        [Min(0f)]
        public float delayAfterWave = 2f;
    }

    [Header("Enemy")]
    [SerializeField] private GameObject enemyPrefab;

    [Header("Spawn Points")]
    [SerializeField] private Transform[] spawnPoints;

    [Header("Trial")]
    [SerializeField] private float trialStartDelay = 2f;

    [Header("Waves")]
    [SerializeField] private TrialWave[] waves;

    [Header("Trial Completion Reward")]
    [SerializeField] private int completionXPReward = 60;
    [SerializeField] private float completionHealthReward = 30f;
    [SerializeField] private float completionLanternReward = 40f;

    private readonly List<GameObject> activeEnemies =
        new List<GameObject>();

    private int currentWaveIndex = -1;

    private bool trialStarted;
    private bool trialCompleted;
    private bool completionRewardGiven;

    public int CurrentWave =>
        currentWaveIndex + 1;

    public int TotalWaves =>
        waves != null
            ? waves.Length
            : 0;

    public bool TrialCompleted =>
        trialCompleted;

    private void Start()
    {
        StartCoroutine(
            TrialRoutine()
        );
    }

    private IEnumerator TrialRoutine()
    {
        if (trialStarted)
            yield break;

        trialStarted = true;

        if (enemyPrefab == null)
        {
            Debug.LogError(
                "TrialManager: Enemy Prefab is missing!"
            );

            yield break;
        }

        if (spawnPoints == null ||
            spawnPoints.Length == 0)
        {
            Debug.LogError(
                "TrialManager: No spawn points assigned!"
            );

            yield break;
        }

        if (waves == null ||
            waves.Length == 0)
        {
            Debug.LogError(
                "TrialManager: No waves configured!"
            );

            yield break;
        }

        Debug.Log(
            "TRIAL OF WORTH STARTING..."
        );

        yield return new WaitForSeconds(
            trialStartDelay
        );

        // =========================
        // WAVES
        // =========================
        for (int i = 0;
             i < waves.Length;
             i++)
        {
            currentWaveIndex = i;

            TrialWave wave =
                waves[i];

            Debug.Log(
                "WAVE " +
                CurrentWave +
                " / " +
                TotalWaves
            );

            yield return StartCoroutine(
                SpawnWave(wave)
            );

            yield return StartCoroutine(
                WaitForWaveClear()
            );

            Debug.Log(
                "Wave " +
                CurrentWave +
                " cleared!"
            );

            if (i < waves.Length - 1)
            {
                yield return new WaitForSeconds(
                    wave.delayAfterWave
                );
            }
        }

        CompleteTrial();
    }

    private IEnumerator SpawnWave(
        TrialWave wave)
    {
        activeEnemies.Clear();

        for (int i = 0;
             i < wave.enemyCount;
             i++)
        {
            Transform spawnPoint =
                GetSpawnPoint(i);

            if (spawnPoint == null)
                continue;

            GameObject enemy =
                Instantiate(
                    enemyPrefab,
                    spawnPoint.position,
                    Quaternion.identity
                );

            activeEnemies.Add(
                enemy
            );

            if (wave.spawnDelay > 0f &&
                i < wave.enemyCount - 1)
            {
                yield return new WaitForSeconds(
                    wave.spawnDelay
                );
            }
        }
    }

    private Transform GetSpawnPoint(
        int enemyNumber)
    {
        if (spawnPoints == null ||
            spawnPoints.Length == 0)
        {
            return null;
        }

        int index =
            (enemyNumber +
             currentWaveIndex) %
            spawnPoints.Length;

        return spawnPoints[index];
    }

    private IEnumerator WaitForWaveClear()
    {
        while (true)
        {
            activeEnemies.RemoveAll(
                enemy => enemy == null
            );

            if (activeEnemies.Count == 0)
            {
                yield break;
            }

            yield return null;
        }
    }

    // =========================
    // TRIAL COMPLETE
    // =========================
    private void CompleteTrial()
    {
        if (trialCompleted)
            return;

        trialCompleted = true;

        GiveCompletionReward();

        Debug.Log(
            "TRIAL OF WORTH COMPLETED!"
        );

        // Later:
        // Show Trial Complete panel
        // Unlock Ability tab
        // Give Ability Points
        // Spawn exit portal
    }

    // =========================
    // COMPLETION REWARD
    // =========================
    private void GiveCompletionReward()
    {
        if (completionRewardGiven)
            return;

        completionRewardGiven = true;

        PlayerProgression progression =
            FindAnyObjectByType<PlayerProgression>();

        PlayerHealth playerHealth =
            FindAnyObjectByType<PlayerHealth>();

        LanternSystem lanternSystem =
            FindAnyObjectByType<LanternSystem>();

        RewardFeedbackUI rewardUI =
            FindAnyObjectByType<RewardFeedbackUI>();

        float actualHealthReward = 0f;
        float actualLanternReward = 0f;

        // XP
        if (progression != null)
        {
            progression.AddXP(
                completionXPReward
            );
        }

        // HEALTH
        if (playerHealth != null)
        {
            actualHealthReward =
                playerHealth.Heal(
                    completionHealthReward
                );
        }

        // LANTERN
        if (lanternSystem != null)
        {
            actualLanternReward =
                lanternSystem.RechargeEnergy(
                    completionLanternReward
                );
        }

        // HUD popup
        if (rewardUI != null)
        {
            rewardUI.ShowRewards(
                actualHealthReward,
                actualLanternReward,
                completionXPReward
            );
        }

        Debug.Log(
            "TRIAL REWARD: +" +
            completionXPReward +
            " XP, +" +
            actualHealthReward +
            " HP, +" +
            actualLanternReward +
            " Lantern."
        );
    }
}