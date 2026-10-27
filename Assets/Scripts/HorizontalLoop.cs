using UnityEngine;

public class HorizontalLoop : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Segments")]
    [SerializeField] private Transform leftSegment;
    [SerializeField] private Transform centerSegment;
    [SerializeField] private Transform rightSegment;

    [Header("Settings")]
    [SerializeField] private float segmentWidth = 20f;

    private void Start()
    {
        SetupSegments();
    }

    private void LateUpdate()
    {
        if (target == null)
            return;

        UpdateLoop();
    }

    private void SetupSegments()
    {
        if (leftSegment == null ||
            centerSegment == null ||
            rightSegment == null)
        {
            return;
        }

        leftSegment.position = new Vector3(
            centerSegment.position.x - segmentWidth,
            leftSegment.position.y,
            leftSegment.position.z
        );

        rightSegment.position = new Vector3(
            centerSegment.position.x + segmentWidth,
            rightSegment.position.y,
            rightSegment.position.z
        );
    }

    private void UpdateLoop()
    {
        float halfWidth = segmentWidth * 0.5f;

        if (target.position.x >
            centerSegment.position.x + halfWidth)
        {
            MoveRight();
        }
        else if (target.position.x <
                 centerSegment.position.x - halfWidth)
        {
            MoveLeft();
        }
    }

    private void MoveRight()
    {
        Transform oldLeft = leftSegment;

        oldLeft.position = new Vector3(
            rightSegment.position.x + segmentWidth,
            oldLeft.position.y,
            oldLeft.position.z
        );

        leftSegment = centerSegment;
        centerSegment = rightSegment;
        rightSegment = oldLeft;
    }

    private void MoveLeft()
    {
        Transform oldRight = rightSegment;

        oldRight.position = new Vector3(
            leftSegment.position.x - segmentWidth,
            oldRight.position.y,
            oldRight.position.z
        );

        rightSegment = centerSegment;
        centerSegment = leftSegment;
        leftSegment = oldRight;
    }
}