using UnityEngine;

// Gentle pulsing glow + breathing scale for beacons, crystals, lanterns and
// the active-player ring. Reads the material's current emission as the base.
public class PulseEmissive : MonoBehaviour
{
    [SerializeField] private float speed = 2.2f;
    [SerializeField] private float emissionPulse = 0.6f;
    [SerializeField] private float scalePulse = 0.06f;

    private Material material;
    private Color baseEmission;
    private Vector3 baseScale;
    private float phase;
    private bool hasEmission;

    public void Configure(float pulseSpeed, float emission, float scale)
    {
        speed = pulseSpeed;
        emissionPulse = emission;
        scalePulse = scale;
    }

    private void Start()
    {
        phase = Random.value * 6.28f;
        baseScale = transform.localScale;

        var renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            material = renderer.material;
            if (material != null && material.HasProperty("_EmissionColor"))
            {
                baseEmission = material.GetColor("_EmissionColor");
                hasEmission = baseEmission.maxColorComponent > 0.001f;
            }
        }
    }

    private void Update()
    {
        float wave = (Mathf.Sin(Time.time * speed + phase) + 1f) * 0.5f; // 0..1

        if (scalePulse > 0f)
        {
            transform.localScale = baseScale * (1f + (wave - 0.5f) * scalePulse * 2f);
        }

        if (hasEmission && material != null)
        {
            material.SetColor("_EmissionColor", baseEmission * (1f + wave * emissionPulse));
        }
    }
}
