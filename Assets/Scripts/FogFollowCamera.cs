using UnityEngine;

public class FogFollowCamera : MonoBehaviour
{
    [SerializeField] private Transform targetCamera;

    [Header("Follow")]
    [SerializeField] private bool followX = true;
    [SerializeField] private bool followY = true;

    private float originalZ;

    private void Awake()
    {
        originalZ = transform.position.z;

        if (targetCamera == null && Camera.main != null)
        {
            targetCamera = Camera.main.transform;
        }
    }

    private void LateUpdate()
    {
        if (targetCamera == null)
            return;

        Vector3 position = transform.position;

        if (followX)
        {
            position.x = targetCamera.position.x;
        }

        if (followY)
        {
            position.y = targetCamera.position.y;
        }

        position.z = originalZ;

        transform.position = position;
    }
}