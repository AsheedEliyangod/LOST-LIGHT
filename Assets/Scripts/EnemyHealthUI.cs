using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnemyHealth enemyHealth;
    [SerializeField] private Image healthFill;

    private void Awake()
    {
        // Automatically find EnemyHealth
        // from ShadowEnemy_01 parent.
        if (enemyHealth == null)
        {
            enemyHealth = GetComponentInParent<EnemyHealth>();
        }
    }

    private void Start()
    {
        UpdateHealthBar();
    }

    private void Update()
    {
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        if (enemyHealth == null || healthFill == null)
            return;

        float healthPercent =
            enemyHealth.CurrentHealth /
            enemyHealth.MaxHealth;

        healthFill.fillAmount =
            Mathf.Clamp01(healthPercent);
    }
}