using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Follow")]
    [SerializeField] private float horizontalOffset = 0f;
    [SerializeField] private float fixedY = 0f;
    [SerializeField] private float smoothTime = 0.18f;

    [Header("Camera Bounds")]
    [SerializeField] private bool useBounds = true;
    [SerializeField] private float minX = 0f;
    [SerializeField] private float maxX = 0f;

    private Vector3 velocity;

    private void LateUpdate()
    {
        if (target == null)
            return;

        float targetX = target.position.x + horizontalOffset;

        if (useBounds)
        {
            targetX = Mathf.Clamp(
                targetX,
                minX,
                maxX
            );
        }

        Vector3 desiredPosition = new Vector3(
            targetX,
            fixedY,
            -10f
        );

        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPosition,
            ref velocity,
            smoothTime
        );
    }
}