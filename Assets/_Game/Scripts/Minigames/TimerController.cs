using UnityEngine;
using UnityEngine.UI;

public class TimerController : MonoBehaviour
{
    [SerializeField] private Text timerText;
    [SerializeField] private Image fillImage;

    private float duration = 1f;

    public void Initialize(Text text, Image fill)
    {
        timerText = text;
        fillImage = fill;
    }

    public void SetDuration(float seconds)
    {
        duration = Mathf.Max(0.01f, seconds);
        SetRemaining(seconds);
    }

    public void SetRemaining(float seconds)
    {
        float clamped = Mathf.Max(0f, seconds);
        if (timerText != null)
        {
            timerText.text = $"{clamped:00.0}";
        }

        if (fillImage != null)
        {
            fillImage.fillAmount = Mathf.Clamp01(clamped / duration);
        }
    }
}
