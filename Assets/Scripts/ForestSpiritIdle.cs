using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ForestSpiritIdle : MonoBehaviour
{
    [Header("Floating")]
    [SerializeField] private float floatHeight = 0.06f;
    [SerializeField] private float floatSpeed = 1.4f;

    [Header("Sway")]
    [SerializeField] private float swayAmount = 1.5f;
    [SerializeField] private float swaySpeed = 0.8f;

    [Header("Breathing")]
    [SerializeField] private float breatheAmount = 0.015f;
    [SerializeField] private float breatheSpeed = 1.1f;

    [Header("Lantern Light")]
    [SerializeField] private Light2D spiritLight;
    [SerializeField] private float lightIntensity = 0.8f;
    [SerializeField] private float lightPulseAmount = 0.15f;
    [SerializeField] private float lightPulseSpeed = 1.8f;

    private Vector3 startPosition;
    private Vector3 startScale;
    private Quaternion startRotation;

    private void Start()
    {
        startPosition = transform.localPosition;
        startScale = transform.localScale;
        startRotation = transform.localRotation;
    }

    private void Update()
    {
        AnimateFloat();
        AnimateSway();
        AnimateBreathing();
        AnimateLantern();
    }

    private void AnimateFloat()
    {
        float offset =
            Mathf.Sin(Time.time * floatSpeed) *
            floatHeight;

        transform.localPosition =
            startPosition +
            new Vector3(0f, offset, 0f);
    }

    private void AnimateSway()
    {
        float angle =
            Mathf.Sin(Time.time * swaySpeed) *
            swayAmount;

        transform.localRotation =
            startRotation *
            Quaternion.Euler(0f, 0f, angle);
    }

    private void AnimateBreathing()
    {
        float breathe =
            Mathf.Sin(Time.time * breatheSpeed) *
            breatheAmount;

        transform.localScale =
            startScale *
            (1f + breathe);
    }

    private void AnimateLantern()
    {
        if (spiritLight == null)
            return;

        spiritLight.intensity =
            lightIntensity +
            Mathf.Sin(Time.time * lightPulseSpeed) *
            lightPulseAmount;
    }
}