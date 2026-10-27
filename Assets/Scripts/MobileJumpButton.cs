
using UnityEngine;
using UnityEngine.EventSystems;

public class MobileJumpButton : MonoBehaviour,
    IPointerDownHandler,
    IPointerUpHandler,
    IPointerExitHandler
{
    [SerializeField] private PlayerMovement2D playerMovement;

    private void Awake()
    {
        if (playerMovement == null)
            playerMovement = FindAnyObjectByType<PlayerMovement2D>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (playerMovement == null)
            playerMovement = FindAnyObjectByType<PlayerMovement2D>();

        if (playerMovement != null)
            playerMovement.MobileJumpPressed();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        ReleaseJump();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ReleaseJump();
    }

    private void ReleaseJump()
    {
        if (playerMovement != null)
            playerMovement.MobileJumpReleased();
    }

    private void OnDisable()
    {
        ReleaseJump();
    }
}
