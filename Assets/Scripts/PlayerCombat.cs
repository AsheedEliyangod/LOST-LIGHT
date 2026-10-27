
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerCombat : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer playerSprite;
    [SerializeField] private Transform attackPoint;

    [Header("Attack")]
    [SerializeField] private float attackCooldown = 0.6f;
    [SerializeField] private LayerMask enemyLayer;

    [Header("Hit VFX")]
    [SerializeField] private GameObject hitEffectPrefab;

    private PlayerHealth playerHealth;
    private PlayerAbilityUpgrade abilityUpgrade;

    private bool attacking;
    private float nextAttackTime;
    private float attackPointOriginalX;

    public bool IsAttacking => attacking;

    private void Awake()
    {
        playerHealth = GetComponent<PlayerHealth>();
        abilityUpgrade = GetComponent<PlayerAbilityUpgrade>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (playerSprite == null)
            playerSprite = GetComponentInChildren<SpriteRenderer>();

        if (attackPoint != null)
            attackPointOriginalX =
                Mathf.Abs(attackPoint.localPosition.x);

        if (abilityUpgrade == null)
            Debug.LogWarning(
                "PlayerAbilityUpgrade component is missing!");
    }

    private void Update()
    {
        if (playerHealth != null &&
            (playerHealth.IsDead || playerHealth.IsHurt))
            return;

        if (DialogueUI.Instance != null &&
            DialogueUI.Instance.IsDialogueActive)
            return;

        UpdateAttackPointDirection();

        // Prevent mouse clicks on UI controls from attacking.
        if (Input.GetMouseButtonDown(0))
        {
            bool clickedOnUI =
                EventSystem.current != null &&
                EventSystem.current.IsPointerOverGameObject();

            if (!clickedOnUI)
                TryAttack();
        }

        // Preserve keyboard attack for PC.
        if (Input.GetKeyDown(KeyCode.J))
            TryAttack();
    }

    // Called by the mobile Attack button.
    public void MobileAttackPressed()
    {
        if (playerHealth != null &&
            (playerHealth.IsDead || playerHealth.IsHurt))
            return;

        if (DialogueUI.Instance != null &&
            DialogueUI.Instance.IsDialogueActive)
            return;

        // Start attack immediately for jump cut feel.
        TryAttack();
        
        // Apply a short delay for mobile attack to match the jump cut feel.
        Invoke(nameof(FinishAttack), 0.1f);
    }

    private void TryAttack()
    {
        if (attacking)
            return;

        if (Time.time < nextAttackTime)
            return;

        nextAttackTime = Time.time + attackCooldown;
        attacking = true;

        if (animator != null)
        {
            animator.SetFloat("Speed", 0f);
            animator.SetTrigger("Attack");
        }
    }

    public void DealAttackDamage()
    {
        if (!attacking || attackPoint == null)
            return;

        float currentAttackDamage = 25f;
        float currentAttackRange = 0.8f;

        if (abilityUpgrade != null)
        {
            currentAttackDamage = abilityUpgrade.AttackDamage;
            currentAttackRange = abilityUpgrade.AttackRange;
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            attackPoint.position,
            currentAttackRange,
            enemyLayer
        );

        HashSet<EnemyHealth> damagedEnemies =
            new HashSet<EnemyHealth>();

        foreach (Collider2D hit in hits)
        {
            EnemyHealth enemy = hit.GetComponent<EnemyHealth>();

            if (enemy == null)
                enemy = hit.GetComponentInParent<EnemyHealth>();

            if (enemy == null ||
                enemy.IsDead ||
                damagedEnemies.Contains(enemy))
                continue;

            damagedEnemies.Add(enemy);

            Vector2 hitPosition =
                hit.ClosestPoint(attackPoint.position);

            enemy.TakeDamage(currentAttackDamage);

            if (hitEffectPrefab != null)
                Instantiate(
                    hitEffectPrefab,
                    hitPosition,
                    Quaternion.identity
                );
        }
    }

    public void FinishAttack()
    {
        attacking = false;
    }

    private void UpdateAttackPointDirection()
    {
        if (attackPoint == null || playerSprite == null)
            return;

        Vector3 position = attackPoint.localPosition;

        position.x = playerSprite.flipX
            ? -attackPointOriginalX
            : attackPointOriginalX;

        attackPoint.localPosition = position;
    }

    public void ResetForRestart()
    {
        attacking = false;
        nextAttackTime = 0f;

        if (animator != null)
        {
            animator.ResetTrigger("Attack");
            animator.SetFloat("Speed", 0f);
        }

        Debug.Log("Player combat reset.");
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        float range = 0.8f;

        PlayerAbilityUpgrade upgrade =
            GetComponent<PlayerAbilityUpgrade>();

        if (upgrade != null)
            range = upgrade.AttackRange;

        Gizmos.DrawWireSphere(attackPoint.position, range);
    }
}
