using UnityEngine;
using UnityEngine.EventSystems;

public class MobileLanternButton : MonoBehaviour,
    IPointerDownHandler,
    IPointerUpHandler
{
    private PlayerCombat playerCombat;
    
    // Instance-specific state to track button press
    private bool m_isPressed = false;

    private void Awake()
    {
        if (playerCombat == null)
            playerCombat = GetComponent<PlayerCombat>();

        if (playerCombat != null)
        {
            UnityEngine.Object.DontDestroyOnLoad(playerCombat);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Toggle the lantern on button press
        if (playerCombat == null)
        {
            playerCombat = GetComponent<PlayerCombat>();
        }

        if (playerCombat != null)
        {
            playerCombat.SendMessage("ToggleLantern", SendMessageOptions.DontRequireReceiver);
            
            m_isPressed = true;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Toggle the lantern off when button is released
        m_isPressed = false;
    }

    private void OnDisable()
    {
        // Ensure state is reset when script stops
        m_isPressed = false;
    }
}