
using UnityEngine;
using UnityEngine.EventSystems;

public class MobileHoldButton : MonoBehaviour,
    IPointerDownHandler,
    IPointerUpHandler,
    IPointerExitHandler
{
    public enum MoveDirection
    {
        Left,
        Right
    }

    [SerializeField] private MoveDirection direction;

    private static bool leftPressed;
    private static bool rightPressed;

    public static float HorizontalInput =>
        (rightPressed ? 1f : 0f) -
        (leftPressed ? 1f : 0f);

    public void OnPointerDown(PointerEventData eventData)
    {
        SetPressed(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        SetPressed(false);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetPressed(false);
    }

    private void SetPressed(bool pressed)
    {
        if (direction == MoveDirection.Left)
            leftPressed = pressed;
        else
            rightPressed = pressed;
    }

    private void OnDisable()
    {
        SetPressed(false);
    }
}
