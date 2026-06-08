using UnityEngine;

// Add this component to an imported character model's root if you want it to be
// recoloured per player (blue / red / green / yellow). Setting the base colour
// multiplies the albedo texture, so a neutral/white-ish model tints cleanly.
//
// Leave it OFF for props or models that already have their final colours.
public class ModelTint : MonoBehaviour
{
    [Tooltip("0 = keep original colour, 1 = full player colour.")]
    [Range(0f, 1f)]
    [SerializeField] private float strength = 1f;

    [Tooltip("Only tint these renderers. If empty, every renderer on the model is tinted.")]
    [SerializeField] private Renderer[] targets;

    public void Apply(Color color)
    {
        Color tint = Color.Lerp(Color.white, color, strength);
        Renderer[] renderers = (targets != null && targets.Length > 0)
            ? targets
            : GetComponentsInChildren<Renderer>();

        foreach (var renderer in renderers)
        {
            if (renderer != null)
            {
                RuntimeVisuals.SetMaterialColor(renderer.material, tint);
            }
        }
    }
}
