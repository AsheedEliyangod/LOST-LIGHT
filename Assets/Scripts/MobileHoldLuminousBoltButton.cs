using UnityEngine;
using UnityEngine.EventSystems;

public class MobileHoldLuminousBoltButton : MonoBehaviour,
    IPointerDownHandler,
    IPointerUpHandler
{
    private PlayerCombat playerCombat;

    // Instance-specific state to track button press
    private bool m_isPressed = false;
    private bool m_holdingLuminousBolt = false;

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
        // Activate luminous bolt on button press
        if (playerCombat == null)
        {
            playerCombat = GetComponent<PlayerCombat>();
        }

        if (playerCombat != null)
        {
            playerCombat.SendMessage("LaunchLuminousBolt", SendMessageOptions.DontRequireReceiver);

            m_isPressed = true;
            m_holdingLuminousBolt = true;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Deactivate luminous bolt when button is released
        m_isPressed = false;
        m_holdingLuminousBolt = false;
    }

    private void OnDisable()
    {
        // Ensure state is reset when script stops
        m_isPressed = false;
        m_holdingLuminousBolt = false;
    }
}