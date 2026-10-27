
using UnityEngine;
using UnityEngine.EventSystems;

public class MobileAttackButton : MonoBehaviour,
    IPointerDownHandler
{
    private PlayerCombat playerCombat;

    private void Awake()
    {
        playerCombat = FindAnyObjectByType<PlayerCombat>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (playerCombat == null)
        {
            playerCombat = FindAnyObjectByType<PlayerCombat>();
        }

        if (playerCombat != null)
        {
            playerCombat.MobileAttackPressed();
        }
        else
        {
            Debug.LogWarning(
                "MobileAttackButton: PlayerCombat was not found."
            );
        }
    }
}
