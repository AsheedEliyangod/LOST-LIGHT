using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;

    [Header("Damage Protection")]
    [SerializeField] private float invulnerabilityTime = 0.7f;

    [Header("Hit Reaction")]
    [SerializeField] private float hurtDuration = 0.25f;
    [SerializeField] private float knockbackForce = 3f;
    [SerializeField] private float knockbackUpForce = 1.2f;
    [SerializeField] private float flashDuration = 0.1f;

    [Header("Hit Flash")]
    [SerializeField] private Color hitFlashColor = Color.white;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer playerSprite;

    [Header("Death")]
    [SerializeField] private float deathAnimationDuration = 1f;
    [SerializeField] private float deathScreenDelay = 0.35f;
    [SerializeField] private DeathUI deathUI;

    private Rigidbody2D rb;

    private float currentHealth;

    private bool invulnerable;
    private float invulnerabilityTimer;

    private bool hurt;
    private bool dead;

    private Color originalSpriteColor = Color.white;

    private Coroutine hurtCoroutine;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;

    public bool IsDead => dead;
    public bool IsHurt => hurt;

    private void Awake()
    {
        currentHealth = maxHealth;

        rb = GetComponent<Rigidbody2D>();

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        if (playerSprite == null)
        {
            playerSprite = GetComponentInChildren<SpriteRenderer>();
        }

        if (playerSprite != null)
        {
            originalSpriteColor = playerSprite.color;
        }

        if (deathUI == null)
        {
            deathUI = FindAnyObjectByType<DeathUI>();
        }
    }

    private void Update()
    {
        if (dead)
            return;

        if (invulnerable)
        {
            invulnerabilityTimer -= Time.deltaTime;

            if (invulnerabilityTimer <= 0f)
            {
                invulnerable = false;
                invulnerabilityTimer = 0f;
            }
        }
    }

    public void TakeDamage(float damage)
    {
        TakeDamageInternal(
            damage,
            null
        );
    }

    public void TakeDamage(
        float damage,
        Vector2 damageSourcePosition)
    {
        TakeDamageInternal(
            damage,
            damageSourcePosition
        );
    }

    private void TakeDamageInternal(
        float damage,
        Vector2? damageSourcePosition)
    {
        if (dead)
            return;

        if (invulnerable)
            return;

        if (damage <= 0f)
            return;

        currentHealth -= damage;

        currentHealth = Mathf.Clamp(
            currentHealth,
            0f,
            maxHealth
        );

        Debug.Log(
            "Player HP: " +
            currentHealth +
            " / " +
            maxHealth
        );

        if (currentHealth <= 0f)
        {
            Die();
            return;
        }

        invulnerable = true;
        invulnerabilityTimer = invulnerabilityTime;

        if (hurtCoroutine != null)
        {
            StopCoroutine(hurtCoroutine);
        }

        hurtCoroutine = StartCoroutine(
            HurtRoutine(
                damageSourcePosition
            )
        );
    }

    // =========================
    // HEALING
    // =========================
    public float Heal(float amount)
    {
        if (dead)
            return 0f;

        if (amount <= 0f)
            return 0f;

        float previousHealth =
            currentHealth;

        currentHealth += amount;

        currentHealth = Mathf.Clamp(
            currentHealth,
            0f,
            maxHealth
        );

        float actualHeal =
            currentHealth -
            previousHealth;

        if (actualHeal > 0f)
        {
            Debug.Log(
                "Player healed +" +
                actualHeal +
                " | HP: " +
                currentHealth +
                " / " +
                maxHealth
            );
        }

        return actualHeal;
    }

    private IEnumerator HurtRoutine(
        Vector2? damageSourcePosition)
    {
        hurt = true;

        float knockbackDirection;

        if (damageSourcePosition.HasValue)
        {
            knockbackDirection =
                Mathf.Sign(
                    transform.position.x -
                    damageSourcePosition.Value.x
                );

            if (Mathf.Approximately(
                    knockbackDirection,
                    0f))
            {
                knockbackDirection = 1f;
            }
        }
        else
        {
            if (playerSprite != null &&
                playerSprite.flipX)
            {
                knockbackDirection = 1f;
            }
            else
            {
                knockbackDirection = -1f;
            }
        }

        if (rb != null)
        {
            rb.linearVelocity =
                new Vector2(
                    knockbackDirection *
                    knockbackForce,

                    knockbackUpForce
                );
        }

        if (playerSprite != null)
        {
            playerSprite.color =
                hitFlashColor;
        }

        yield return new WaitForSeconds(
            flashDuration
        );

        ResetSpriteColor();

        float remainingHurtTime =
            hurtDuration -
            flashDuration;

        if (remainingHurtTime > 0f)
        {
            yield return new WaitForSeconds(
                remainingHurtTime
            );
        }

        if (rb != null)
        {
            rb.linearVelocity =
                new Vector2(
                    0f,
                    rb.linearVelocity.y
                );
        }

        hurt = false;
        hurtCoroutine = null;
    }

    private void ResetSpriteColor()
    {
        if (playerSprite != null)
        {
            playerSprite.color =
                originalSpriteColor;
        }
    }

    private void Die()
    {
        if (dead)
            return;

        dead = true;
        hurt = false;

        currentHealth = 0f;

        if (hurtCoroutine != null)
        {
            StopCoroutine(hurtCoroutine);
            hurtCoroutine = null;
        }

        ResetSpriteColor();

        if (rb != null)
        {
            rb.linearVelocity =
                new Vector2(
                    0f,
                    rb.linearVelocity.y
                );
        }

        if (animator != null)
        {
            animator.SetFloat(
                "Speed",
                0f
            );

            animator.SetBool(
                "Dead",
                true
            );
        }

        StartCoroutine(
            DeathSequence()
        );

        Debug.Log("Player died!");
    }

    private IEnumerator DeathSequence()
    {
        yield return new WaitForSeconds(
            deathAnimationDuration
        );

        if (playerSprite != null)
        {
            playerSprite.enabled = false;
        }

        yield return new WaitForSeconds(
            deathScreenDelay
        );

        if (deathUI != null)
        {
            deathUI.ShowDeathScreen();
        }
    }

    // =========================
    // RESTART RESET
    // =========================
    public void ResetForRestart()
    {
        StopAllCoroutines();

        dead = false;
        hurt = false;
        invulnerable = false;

        invulnerabilityTimer = 0f;
        hurtCoroutine = null;

        currentHealth = maxHealth;

        ResetSpriteColor();

        if (playerSprite != null)
        {
            playerSprite.enabled = true;
        }

        if (rb != null)
        {
            rb.linearVelocity =
                Vector2.zero;

            rb.angularVelocity = 0f;
        }

        if (animator != null)
        {
            // Completely reset the death animation state.
            animator.Rebind();
            animator.Update(0f);

            animator.SetBool(
                "Dead",
                false
            );

            animator.SetFloat(
                "Speed",
                0f
            );
        }

        Debug.Log(
            "Player reset. HP: " +
            currentHealth +
            " / " +
            maxHealth
        );
    }
}