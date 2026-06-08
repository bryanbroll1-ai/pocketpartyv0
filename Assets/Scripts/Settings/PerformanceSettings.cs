using UnityEngine;

public class PerformanceSettings : MonoBehaviour
{
    public bool BatterySaver { get; private set; } = true;
    public int TargetFrameRate { get; private set; } = 30;

    private void Awake()
    {
        Apply();
    }

    public void SetBatterySaver(bool enabled)
    {
        BatterySaver = enabled;
        if (enabled)
        {
            SetTargetFrameRate(30);
        }

        Apply();
    }

    public void SetTargetFrameRate(int frameRate)
    {
        TargetFrameRate = frameRate == 60 ? 60 : 30;
        if (TargetFrameRate == 60)
        {
            BatterySaver = false;
        }

        Apply();
    }

    private void Apply()
    {
        Application.targetFrameRate = TargetFrameRate;
        QualitySettings.vSyncCount = 0;
        QualitySettings.shadows = ShadowQuality.Disable;
        QualitySettings.softParticles = false;
        QualitySettings.realtimeReflectionProbes = false;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }
}
