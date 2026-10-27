using System.Collections;
using UnityEngine;

public class PortalTeleport : MonoBehaviour
{
    [Header("Teleport")]
    [SerializeField] private Transform destination;

    [Header("Transition")]
    [SerializeField] private ScreenFader screenFader;

    [Header("Settings")]
    [SerializeField] private bool resetVelocity = true;

    private bool teleporting;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (teleporting)
            return;

        if (destination == null)
        {
            Debug.LogWarning("Portal destination has not been assigned!");
            return;
        }

        StartCoroutine(TeleportRoutine(other));
    }

    private IEnumerator TeleportRoutine(Collider2D playerCollider)
    {
        teleporting = true;

        if (screenFader != null)
            yield return StartCoroutine(screenFader.FadeOut());

        Rigidbody2D playerRb =
            playerCollider.GetComponent<Rigidbody2D>();

        if (playerRb != null)
        {
            playerRb.position = destination.position;

            if (resetVelocity)
                playerRb.linearVelocity = Vector2.zero;
        }
        else
        {
            playerCollider.transform.position =
                destination.position;
        }

        yield return null;

        if (screenFader != null)
            yield return StartCoroutine(screenFader.FadeIn());

        teleporting = false;
    }
}