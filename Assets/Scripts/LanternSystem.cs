using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LanternSystem : MonoBehaviour
{
    [Header("Lantern Light")]
    [SerializeField] private Light2D lanternLight;

    [Header("Lantern")]
    [SerializeField] private bool startLanternOn = true;

    [Header("Normal Mode")]
    [SerializeField] private float normalIntensity = 1.7f;
    [SerializeField] private float normalRadius = 3.5f;
    [SerializeField] private float normalDrainPerSecond = 2f;

    [Header("Strong Mode")]
    [SerializeField] private float strongIntensity = 3f;
    [SerializeField] private float strongRadius = 5f;
    [SerializeField] private float strongDrainPerSecond = 6f;

    [Header("Energy")]
    [SerializeField] private float maxEnergy = 100f;
    [SerializeField] private float startingEnergy = 100f;

    private PlayerHealth playerHealth;

    private float currentEnergy;

    private bool lanternOn;
    private bool strongMode;

    private bool deathHandled;

    public float CurrentEnergy => currentEnergy;
    public float MaxEnergy => maxEnergy;

    public bool IsLanternOn => lanternOn;
    public bool IsStrongMode => strongMode;

    private void Awake()
    {
        playerHealth =
            GetComponent<PlayerHealth>();

        currentEnergy =
            Mathf.Clamp(
                startingEnergy,
                0f,
                maxEnergy
            );

        lanternOn =
            startLanternOn;

        strongMode = false;

        UpdateLanternLight();
    }

    private void Update()
    {
        if (IsPlayerDead())
        {
            HandlePlayerDeath();
            return;
        }

        deathHandled = false;

        HandleKeyboardInput();
        DrainEnergy();
        UpdateLanternLight();
    }

    private bool IsPlayerDead()
    {
        return playerHealth != null &&
               playerHealth.IsDead;
    }

    private void HandlePlayerDeath()
    {
        if (deathHandled)
            return;

        deathHandled = true;

        lanternOn = false;
        strongMode = false;

        UpdateLanternLight();
    }

    private void HandleKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            ToggleLantern();
        }

        if (lanternOn &&
            currentEnergy > 0f)
        {
            strongMode =
                Input.GetKey(
                    KeyCode.LeftShift
                );
        }
        else
        {
            strongMode = false;
        }
    }

    private void DrainEnergy()
    {
        if (IsPlayerDead())
            return;

        if (!lanternOn)
            return;

        float drainAmount =
            strongMode
                ? strongDrainPerSecond
                : normalDrainPerSecond;

        currentEnergy -=
            drainAmount *
            Time.deltaTime;

        currentEnergy =
            Mathf.Clamp(
                currentEnergy,
                0f,
                maxEnergy
            );

        if (currentEnergy <= 0f)
        {
            currentEnergy = 0f;

            lanternOn = false;
            strongMode = false;
        }
    }

    private void UpdateLanternLight()
    {
        if (lanternLight == null)
            return;

        if (IsPlayerDead())
        {
            lanternLight.enabled = false;
            return;
        }

        lanternLight.enabled =
            lanternOn &&
            currentEnergy > 0f;

        if (!lanternLight.enabled)
            return;

        if (strongMode)
        {
            lanternLight.intensity =
                strongIntensity;

            lanternLight.pointLightOuterRadius =
                strongRadius;
        }
        else
        {
            lanternLight.intensity =
                normalIntensity;

            lanternLight.pointLightOuterRadius =
                normalRadius;
        }
    }

    public void ToggleLantern()
    {
        if (IsPlayerDead())
            return;

        if (lanternOn)
        {
            TurnLanternOff();
        }
        else
        {
            TurnLanternOn();
        }
    }

    public void TurnLanternOn()
    {
        if (IsPlayerDead())
            return;

        if (currentEnergy <= 0f)
            return;

        lanternOn = true;

        UpdateLanternLight();
    }

    public void TurnLanternOff()
    {
        lanternOn = false;
        strongMode = false;

        UpdateLanternLight();
    }

    public void SetStrongMode(bool enabled)
    {
        if (IsPlayerDead())
        {
            strongMode = false;

            UpdateLanternLight();

            return;
        }

        if (!lanternOn ||
            currentEnergy <= 0f)
        {
            strongMode = false;

            UpdateLanternLight();

            return;
        }

        strongMode = enabled;

        UpdateLanternLight();
    }

    // =========================
    // ENERGY REWARD
    // =========================
    public float RechargeEnergy(float amount)
    {
        if (IsPlayerDead())
            return 0f;

        if (amount <= 0f)
            return 0f;

        float previousEnergy =
            currentEnergy;

        currentEnergy += amount;

        currentEnergy =
            Mathf.Clamp(
                currentEnergy,
                0f,
                maxEnergy
            );

        float actualRecharge =
            currentEnergy -
            previousEnergy;

        if (actualRecharge > 0f)
        {
            Debug.Log(
                "Lantern recharged +" +
                actualRecharge +
                " | Energy: " +
                currentEnergy +
                " / " +
                maxEnergy
            );
        }

        return actualRecharge;
    }

    // =========================
    // RESTART RESET
    // =========================
    public void ResetForRestart()
    {
        currentEnergy = maxEnergy;

        lanternOn =
            startLanternOn;

        strongMode = false;
        deathHandled = false;

        UpdateLanternLight();

        Debug.Log(
            "Lantern reset. Energy: " +
            currentEnergy +
            " / " +
            maxEnergy
        );
    }
}