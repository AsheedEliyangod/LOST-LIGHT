using UnityEngine;

public class HiddenSymbol : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LanternSystem lanternSystem;
    [SerializeField] private Transform player;
    [SerializeField] private SpriteRenderer symbolRenderer;
    [SerializeField] private GameObject portalObject;

    [Header("Reveal Settings")]
    [SerializeField] private float revealDistance = 4.5f;
    [SerializeField] private float fadeSpeed = 5f;

    [Header("Activation Settings")]
    [SerializeField] private float activationHorizontalDistance = 2.5f;

    [Header("Debug")]
    [SerializeField] private bool revealed;
    [SerializeField] private bool activated;

    private float currentAlpha;

    private void Awake()
    {
        if (symbolRenderer == null)
        {
            symbolRenderer = GetComponent<SpriteRenderer>();
        }

        currentAlpha = 0f;
        activated = false;

        SetAlpha(0f);

        if (portalObject != null)
        {
            portalObject.SetActive(false);
        }
    }

    private void Update()
    {
        CheckReveal();
        CheckActivation();
        UpdateVisual();
    }

    private void CheckReveal()
    {
        if (activated)
        {
            revealed = true;
            return;
        }

        if (lanternSystem == null || player == null)
        {
            revealed = false;
            return;
        }

        float distance = Vector2.Distance(
            player.position,
            transform.position
        );

        revealed =
            lanternSystem.IsLanternOn &&
            lanternSystem.IsStrongMode &&
            distance <= revealDistance;
    }

    private void CheckActivation()
    {
        if (activated)
            return;

        if (!revealed)
            return;

        if (player == null)
            return;

        // Only check horizontal distance.
        float horizontalDistance =
            Mathf.Abs(player.position.x - transform.position.x);

        if (horizontalDistance > activationHorizontalDistance)
            return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            ActivateSymbol();
        }
    }

    public void ActivateSymbol()
    {
        if (activated)
            return;

        if (!revealed)
            return;

        activated = true;
        revealed = true;

        SetAlpha(1f);

        if (portalObject != null)
        {
            portalObject.SetActive(true);
        }

        Debug.Log("Hidden symbol activated - Portal opened!");
    }

    private void UpdateVisual()
    {
        float targetAlpha = revealed ? 1f : 0f;

        currentAlpha = Mathf.MoveTowards(
            currentAlpha,
            targetAlpha,
            fadeSpeed * Time.deltaTime
        );

        SetAlpha(currentAlpha);
    }

    private void SetAlpha(float alpha)
    {
        if (symbolRenderer == null)
            return;

        Color color = symbolRenderer.color;
        color.a = alpha;
        symbolRenderer.color = color;
    }
}