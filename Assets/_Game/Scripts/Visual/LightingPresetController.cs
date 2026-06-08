using UnityEngine;

// Applies the shared "premium toy diorama" lighting in the standalone
// showcase / minigame scenes so they match the main board look.
public class LightingPresetController : MonoBehaviour
{
    [SerializeField] private bool applyOnAwake = true;

    private void Awake()
    {
        if (applyOnAwake)
        {
            ApplyBrightCartoonLighting();
        }
    }

    public void ApplyBrightCartoonLighting()
    {
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
        RenderSettings.ambientSkyColor = new Color(0.82f, 0.90f, 1.00f);
        RenderSettings.ambientEquatorColor = new Color(0.66f, 0.70f, 0.74f);
        RenderSettings.ambientGroundColor = new Color(0.30f, 0.34f, 0.34f);
        RenderSettings.ambientIntensity = 1.0f;

        Material sky = RuntimeVisuals.CreateGradientSkybox(
            new Color(0.55f, 0.78f, 1.00f),
            new Color(0.42f, 0.46f, 0.50f));
        if (sky != null)
        {
            RenderSettings.skybox = sky;
        }

        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.Linear;
        RenderSettings.fogColor = new Color(0.78f, 0.86f, 0.93f);
        RenderSettings.fogStartDistance = 16f;
        RenderSettings.fogEndDistance = 60f;

        Light key = FindAnyObjectByType<Light>();
        if (key == null)
        {
            key = new GameObject("Key Sun").AddComponent<Light>();
        }

        key.name = "Key Sun";
        key.type = LightType.Directional;
        key.shadows = LightShadows.Soft;
        key.shadowStrength = 0.55f;
        key.intensity = 1.18f;
        key.color = new Color(1.00f, 0.95f, 0.84f);
        key.transform.rotation = Quaternion.Euler(48f, -34f, 0f);

        AddFill("Cool Fill", new Color(0.66f, 0.78f, 1.00f), 0.42f, Quaternion.Euler(28f, 150f, 0f));
        AddFill("Warm Rim", new Color(1.00f, 0.82f, 0.62f), 0.55f, Quaternion.Euler(-18f, -150f, 0f));

        DynamicGI.UpdateEnvironment();
    }

    private void AddFill(string fillName, Color color, float intensity, Quaternion rotation)
    {
        Transform existing = transform.Find(fillName);
        if (existing != null)
        {
            return;
        }

        var fill = new GameObject(fillName).AddComponent<Light>();
        fill.transform.SetParent(transform, false);
        fill.name = fillName;
        fill.type = LightType.Directional;
        fill.color = color;
        fill.intensity = intensity;
        fill.shadows = LightShadows.None;
        fill.transform.rotation = rotation;
    }
}
