using UnityEngine;

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
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(0.88f, 0.92f, 0.96f);
        RenderSettings.fog = false;

        Light existingLight = FindAnyObjectByType<Light>();
        if (existingLight == null)
        {
            GameObject lightObject = new GameObject("Bright Cartoon Sun");
            existingLight = lightObject.AddComponent<Light>();
        }

        existingLight.name = "Bright Cartoon Sun";
        existingLight.type = LightType.Directional;
        existingLight.shadows = LightShadows.None;
        existingLight.intensity = 1.22f;
        existingLight.color = new Color(1f, 0.96f, 0.86f);
        existingLight.transform.rotation = Quaternion.Euler(52f, -36f, 0f);
    }
}
