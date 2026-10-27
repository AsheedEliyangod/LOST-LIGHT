using UnityEngine;

public class PlayerCombatAnimationEvents : MonoBehaviour
{
    private PlayerCombat playerCombat;

    private void Awake()
    {
        playerCombat = GetComponentInParent<PlayerCombat>();

        if (playerCombat == null)
        {
            Debug.LogError(
                "PlayerCombat could not be found in parent!"
            );
        }
    }

    public void DealAttackDamage()
    {
        if (playerCombat != null)
        {
            playerCombat.DealAttackDamage();
        }
    }

    public void FinishAttack()
    {
        if (playerCombat != null)
        {
            playerCombat.FinishAttack();
        }
    }
}