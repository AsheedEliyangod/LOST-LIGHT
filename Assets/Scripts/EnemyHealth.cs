using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;

    private float currentHealth;
    private bool dead;

    private ShadowEnemy shadowEnemy;
    private EnemyRewards enemyRewards;

    private Collider2D enemyCollider;
    private Rigidbody2D rb;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public bool IsDead => dead;

    private void Awake()
    {
        currentHealth = maxHealth;

        shadowEnemy =
            GetComponent<ShadowEnemy>();

        enemyRewards =
            GetComponent<EnemyRewards>();

        enemyCollider =
            GetComponent<Collider2D>();

        rb =
            GetComponent<Rigidbody2D>();
    }

    public void TakeDamage(float damage)
    {
        if (dead)
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
            "Enemy HP: " +
            currentHealth +
            " / " +
            maxHealth
        );

        if (currentHealth <= 0f)
        {
            Die();
            return;
        }

        if (shadowEnemy != null)
        {
            shadowEnemy.TakeHit();
        }
    }

    private void Die()
    {
        if (dead)
            return;

        dead = true;
        currentHealth = 0f;

        // GIVE PLAYER REWARDS
        if (enemyRewards != null)
        {
            enemyRewards.GiveRewards();
        }
        else
        {
            Debug.LogWarning(
                "EnemyRewards component is missing!"
            );
        }

        if (shadowEnemy != null)
        {
            shadowEnemy.Die();
        }

        if (rb != null)
        {
            rb.linearVelocity =
                Vector2.zero;

            rb.angularVelocity = 0f;

            rb.bodyType =
                RigidbodyType2D.Kinematic;
        }
    }

    // Called by death animation event.
    public void FinishDeathAnimation()
    {
        if (!dead)
            return;

        if (enemyCollider != null)
        {
            enemyCollider.enabled = false;
        }

        Destroy(gameObject);
    }
}