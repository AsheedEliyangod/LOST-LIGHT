using System.Collections;
using TMPro;
using UnityEngine;

public class RewardFeedbackUI : MonoBehaviour
{
    [Header("Reward Texts")]
    [SerializeField] private TMP_Text healthRewardText;
    [SerializeField] private TMP_Text lanternRewardText;
    [SerializeField] private TMP_Text xpRewardText;

    [Header("Animation")]
    [SerializeField] private float displayDuration = 1f;
    [SerializeField] private float moveDistance = 25f;

    private Coroutine healthRoutine;
    private Coroutine lanternRoutine;
    private Coroutine xpRoutine;

    public void ShowRewards(
        float healthAmount,
        float lanternAmount,
        int xpAmount)
    {
        if (healthAmount > 0f &&
            healthRewardText != null)
        {
            if (healthRoutine != null)
                StopCoroutine(healthRoutine);

            healthRoutine = StartCoroutine(
                ShowRewardText(
                    healthRewardText,
                    "+" + Mathf.RoundToInt(healthAmount)
                )
            );
        }

        if (lanternAmount > 0f &&
            lanternRewardText != null)
        {
            if (lanternRoutine != null)
                StopCoroutine(lanternRoutine);

            lanternRoutine = StartCoroutine(
                ShowRewardText(
                    lanternRewardText,
                    "+" + Mathf.RoundToInt(lanternAmount)
                )
            );
        }

        if (xpAmount > 0 &&
            xpRewardText != null)
        {
            if (xpRoutine != null)
                StopCoroutine(xpRoutine);

            xpRoutine = StartCoroutine(
                ShowRewardText(
                    xpRewardText,
                    "+" + xpAmount + " XP"
                )
            );
        }
    }

    private IEnumerator ShowRewardText(
        TMP_Text rewardText,
        string message)
    {
        GameObject textObject =
            rewardText.gameObject;

        RectTransform rect =
            rewardText.rectTransform;

        Vector2 startPosition =
            rect.anchoredPosition;

        Vector2 targetPosition =
            startPosition +
            Vector2.up * moveDistance;

        rewardText.text = message;

        Color color =
            rewardText.color;

        color.a = 1f;

        rewardText.color = color;

        textObject.SetActive(true);

        float timer = 0f;

        while (timer < displayDuration)
        {
            timer += Time.deltaTime;

            float progress =
                timer / displayDuration;

            rect.anchoredPosition =
                Vector2.Lerp(
                    startPosition,
                    targetPosition,
                    progress
                );

            // Keep visible for first half,
            // then fade away.
            float alpha = 1f;

            if (progress > 0.5f)
            {
                alpha =
                    Mathf.Lerp(
                        1f,
                        0f,
                        (progress - 0.5f) * 2f
                    );
            }

            color.a = alpha;
            rewardText.color = color;

            yield return null;
        }

        rect.anchoredPosition =
            startPosition;

        color.a = 1f;
        rewardText.color = color;

        textObject.SetActive(false);
    }
}