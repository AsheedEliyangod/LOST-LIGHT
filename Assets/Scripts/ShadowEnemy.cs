using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ShadowEnemy : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private LanternSystem lanternSystem;
    [SerializeField] private SpriteRenderer enemySprite;
    [SerializeField] private Animator animator;

    [Header("Movement")]
    [SerializeField] private float patrolSpeed = 1.2f;
    [SerializeField] private float chaseSpeed = 2.2f;

    [Header("Patrol")]
    [SerializeField] private float patrolDistance = 3f;

    [Header("Detection")]
    [SerializeField] private float detectionRange = 5f;

    [Header("Attack")]
    [SerializeField] private float attackRange = 1.2f;
    [SerializeField] private float attackDamage = 25f;
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private float attackHitDelay = 0.3f;
    [SerializeField] private float attackAnimationDuration = 0.7f;

    [Header("Hit Reaction")]
    [SerializeField] private float hurtDuration = 0.25f;
    [SerializeField] private float knockbackForce = 2.5f;
    [SerializeField] private float knockbackUpForce = 0.8f;
    [SerializeField] private float flashDuration = 0.1f;

    [SerializeField] private Color hitFlashColor =
        new Color(1f, 0.25f, 0.25f, 1f);

    [Header("Lantern Stun")]
    [SerializeField] private float lightStunRange = 3.5f;
    [SerializeField] private float stunDuration = 1.5f;

    private Rigidbody2D rb;
    private PlayerHealth playerHealth;

    private Vector2 startPosition;
    private int patrolDirection = 1;

    private bool stunned;
    private float stunTimer;

    private bool attacking;
    private float nextAttackTime;

    private bool hurt;
    private bool dead;

    private bool playerDeathHandled;

    private Color originalSpriteColor =
        Color.white;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (enemySprite == null)
        {
            enemySprite =
                GetComponentInChildren<SpriteRenderer>();
        }

        if (animator == null)
        {
            animator =
                GetComponentInChildren<Animator>();
        }

        if (enemySprite != null)
        {
            originalSpriteColor =
                enemySprite.color;
        }
    }

    private void Start()
    {
        startPosition = transform.position;

        // =========================
        // FIND PLAYER AUTOMATICALLY
        // =========================
        if (player == null)
        {
            PlayerHealth foundPlayer =
                FindAnyObjectByType<PlayerHealth>();

            if (foundPlayer != null)
            {
                playerHealth =
                    foundPlayer;

                player =
                    foundPlayer.transform;
            }
            else
            {
                Debug.LogWarning(
                    name +
                    ": No PlayerHealth found in scene."
                );
            }
        }
        else
        {
            playerHealth =
                player.GetComponent<PlayerHealth>();

            if (playerHealth == null)
            {
                playerHealth =
                    player.GetComponentInParent<PlayerHealth>();
            }
        }

        // =========================
        // FIND LANTERN AUTOMATICALLY
        // =========================
        if (lanternSystem == null)
        {
            lanternSystem =
                FindAnyObjectByType<LanternSystem>();

            if (lanternSystem == null)
            {
                Debug.LogWarning(
                    name +
                    ": No LanternSystem found in scene."
                );
            }
        }
    }

    private void Update()
    {
        if (dead)
            return;

        if (IsPlayerDead())
        {
            HandlePlayerDeath();
            return;
        }

        playerDeathHandled = false;

        if (!hurt)
        {
            HandleLanternStun();
        }

        if (stunned)
        {
            stunTimer -= Time.deltaTime;

            if (stunTimer <= 0f)
            {
                stunned = false;
                stunTimer = 0f;
            }
        }
    }

    private void FixedUpdate()
    {
        if (dead)
        {
            StopMovement();
            return;
        }

        // Don't cancel knockback.
        if (hurt)
        {
            return;
        }

        if (IsPlayerDead())
        {
            Patrol();
            return;
        }

        if (stunned)
        {
            StopMovement();
            return;
        }

        if (attacking)
        {
            StopMovement();
            return;
        }

        if (player == null)
        {
            Patrol();
            return;
        }

        float distanceToPlayer =
            Vector2.Distance(
                transform.position,
                player.position
            );

        // ATTACK
        if (distanceToPlayer <= attackRange)
        {
            FacePlayer();
            TryAttack();
        }

        // CHASE
        else if (distanceToPlayer <= detectionRange)
        {
            ChasePlayer();
        }

        // PATROL
        else
        {
            Patrol();
        }
    }

    private void HandlePlayerDeath()
    {
        if (playerDeathHandled)
            return;

        playerDeathHandled = true;

        attacking = false;
        stunned = false;
        hurt = false;

        StopAllCoroutines();

        ResetSpriteColor();

        if (animator != null)
        {
            animator.ResetTrigger("Attack");

            animator.SetFloat(
                "Speed",
                0f
            );

            animator.Play(
                "Enemy_Idle",
                0,
                0f
            );
        }
    }

    private bool IsPlayerDead()
    {
        return playerHealth != null &&
               playerHealth.IsDead;
    }

    private void Patrol()
    {
        float leftLimit =
            startPosition.x -
            patrolDistance;

        float rightLimit =
            startPosition.x +
            patrolDistance;

        if (transform.position.x >= rightLimit)
        {
            patrolDirection = -1;
        }
        else if (transform.position.x <= leftLimit)
        {
            patrolDirection = 1;
        }

        rb.linearVelocity =
            new Vector2(
                patrolDirection *
                patrolSpeed,

                rb.linearVelocity.y
            );

        UpdateFacing(
            patrolDirection
        );

        UpdateSpeedAnimation();
    }

    private void ChasePlayer()
    {
        if (player == null)
            return;

        float direction =
            Mathf.Sign(
                player.position.x -
                transform.position.x
            );

        rb.linearVelocity =
            new Vector2(
                direction *
                chaseSpeed,

                rb.linearVelocity.y
            );

        UpdateFacing(
            direction
        );

        UpdateSpeedAnimation();
    }

    private void TryAttack()
    {
        if (IsPlayerDead() ||
            hurt ||
            stunned ||
            dead)
        {
            return;
        }

        StopMovement();

        if (Time.time < nextAttackTime)
            return;

        nextAttackTime =
            Time.time +
            attackCooldown;

        StartCoroutine(
            AttackRoutine()
        );
    }

    private IEnumerator AttackRoutine()
    {
        attacking = true;

        StopMovement();

        if (animator != null)
        {
            animator.SetTrigger(
                "Attack"
            );
        }

        yield return new WaitForSeconds(
            attackHitDelay
        );

        if (!IsPlayerDead() &&
            !hurt &&
            !dead)
        {
            TryDamagePlayer();
        }

        float remainingTime =
            attackAnimationDuration -
            attackHitDelay;

        if (remainingTime > 0f)
        {
            yield return new WaitForSeconds(
                remainingTime
            );
        }

        attacking = false;
    }

    private void TryDamagePlayer()
    {
        if (player == null ||
            playerHealth == null ||
            playerHealth.IsDead)
        {
            return;
        }

        float distance =
            Vector2.Distance(
                transform.position,
                player.position
            );

        if (distance > attackRange)
            return;

        playerHealth.TakeDamage(
            attackDamage,
            transform.position
        );
    }

    public void TakeHit()
    {
        if (dead)
            return;

        StopAllCoroutines();

        attacking = false;
        stunned = false;

        if (animator != null)
        {
            animator.ResetTrigger(
                "Attack"
            );

            animator.SetFloat(
                "Speed",
                0f
            );

            animator.Play(
                "Enemy_Idle",
                0,
                0f
            );
        }

        StartCoroutine(
            HurtRoutine()
        );
    }

    private IEnumerator HurtRoutine()
    {
        hurt = true;

        float direction = 1f;

        if (player != null)
        {
            direction =
                Mathf.Sign(
                    transform.position.x -
                    player.position.x
                );

            if (Mathf.Approximately(
                    direction,
                    0f))
            {
                direction = 1f;
            }
        }

        rb.linearVelocity =
            new Vector2(
                direction *
                knockbackForce,

                knockbackUpForce
            );

        if (enemySprite != null)
        {
            enemySprite.color =
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

        rb.linearVelocity =
            new Vector2(
                0f,
                rb.linearVelocity.y
            );

        hurt = false;
    }

    private void HandleLanternStun()
    {
        if (stunned ||
            attacking ||
            hurt ||
            dead ||
            IsPlayerDead())
        {
            return;
        }

        if (player == null ||
            lanternSystem == null)
        {
            return;
        }

        if (!lanternSystem.IsLanternOn)
            return;

        if (!lanternSystem.IsStrongMode)
            return;

        float distance =
            Vector2.Distance(
                transform.position,
                player.position
            );

        if (distance <= lightStunRange)
        {
            Stun();
        }
    }

    private void Stun()
    {
        stunned = true;
        stunTimer = stunDuration;

        StopMovement();
    }

    private void StopMovement()
    {
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
        }
    }

    private void UpdateSpeedAnimation()
    {
        if (animator == null ||
            rb == null)
        {
            return;
        }

        animator.SetFloat(
            "Speed",
            Mathf.Abs(
                rb.linearVelocity.x
            )
        );
    }

    private void FacePlayer()
    {
        if (player == null)
            return;

        float direction =
            Mathf.Sign(
                player.position.x -
                transform.position.x
            );

        UpdateFacing(
            direction
        );
    }

    private void UpdateFacing(float direction)
    {
        if (enemySprite == null)
            return;

        if (direction > 0f)
        {
            enemySprite.flipX = false;
        }
        else if (direction < 0f)
        {
            enemySprite.flipX = true;
        }
    }

    private void ResetSpriteColor()
    {
        if (enemySprite != null)
        {
            enemySprite.color =
                originalSpriteColor;
        }
    }

    public void Die()
    {
        if (dead)
            return;

        dead = true;

        attacking = false;
        stunned = false;
        hurt = false;

        StopAllCoroutines();

        ResetSpriteColor();

        StopMovement();

        if (animator != null)
        {
            animator.ResetTrigger(
                "Attack"
            );

            animator.SetFloat(
                "Speed",
                0f
            );

            animator.SetBool(
                "Dead",
                true
            );
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(
            transform.position,
            detectionRange
        );

        Gizmos.DrawWireSphere(
            transform.position,
            attackRange
        );

        Gizmos.DrawWireSphere(
            transform.position,
            lightStunRange
        );
    }
}