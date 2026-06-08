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

        // Real shadows are the single biggest depth cue and this scene has very
        // few objects, so we keep them on but cheap (hard, single map, short
        // distance). Soft grounding blobs handle everything beyond the range.
        QualitySettings.shadows = ShadowQuality.HardOnly;
        QualitySettings.shadowResolution = BatterySaver ? ShadowResolution.Low : ShadowResolution.Medium;
        QualitySettings.shadowDistance = 32f;
        QualitySettings.shadowCascades = 0;

        QualitySettings.softParticles = false;
        QualitySettings.realtimeReflectionProbes = false;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }
}
