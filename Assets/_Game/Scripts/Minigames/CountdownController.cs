using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CountdownController : MonoBehaviour
{
    [SerializeField] private Text countdownText;
    [SerializeField] private CanvasGroup canvasGroup;

    public void Initialize(Text text, CanvasGroup group)
    {
        countdownText = text;
        canvasGroup = group;
        Hide();
    }

    public void ShowValue(string value)
    {
        if (countdownText != null)
        {
            countdownText.text = value;
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
        }
    }

    public void Hide()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }
    }

    public IEnumerator PlayCountdown(float stepDuration)
    {
        string[] steps = { "3", "2", "1", "GO" };
        for (int i = 0; i < steps.Length; i++)
        {
            ShowValue(steps[i]);
            yield return new WaitForSeconds(stepDuration);
        }

        Hide();
    }
}
