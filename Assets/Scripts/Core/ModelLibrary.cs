using System.Collections.Generic;
using UnityEngine;

// Optional drop-in model system. If a prefab/model named <key> exists under a
// "Resources/Models" folder, it is instantiated in place of the procedural
// primitive visuals. If not, callers fall back to the built-in generated look.
//
// This is how you swap in real art (e.g. GLB models from Meshy3D) without
// touching any gameplay code. See TUTORIAL_Modelle_ersetzen.md.
//
// Recognised keys:
//   Character_0 .. Character_3   -> per-player piece (board + minigames)
//   Character                    -> generic fallback for all players
//   Prop_Tree, Prop_Rock, Prop_Crystal, Prop_Flag, Prop_Lantern
public static class ModelLibrary
{
    private const string Root = "Models/";
    private static readonly Dictionary<string, GameObject> cache = new Dictionary<string, GameObject>();

    public static bool HasModel(string key)
    {
        return LoadPrefab(key) != null;
    }

    // Instantiates the model for the given key (with optional fallback key) as a
    // child of parent at local origin. Returns null if no model is available.
    public static GameObject TryInstantiate(string key, Transform parent, string fallbackKey = null)
    {
        GameObject prefab = LoadPrefab(key);
        if (prefab == null && !string.IsNullOrEmpty(fallbackKey))
        {
            prefab = LoadPrefab(fallbackKey);
        }

        if (prefab == null)
        {
            return null;
        }

        GameObject instance = Object.Instantiate(prefab, parent);
        instance.transform.localPosition = Vector3.zero;
        instance.transform.localRotation = Quaternion.identity;
        StripColliders(instance);
        return instance;
    }

    // Applies a player colour to an instantiated model, but only if the model
    // opts in via a ModelTint component (so textured props are left untouched).
    public static void ApplyTint(GameObject instance, Color color)
    {
        if (instance == null)
        {
            return;
        }

        var tint = instance.GetComponent<ModelTint>();
        if (tint != null)
        {
            tint.Apply(color);
        }
    }

    private static GameObject LoadPrefab(string key)
    {
        if (string.IsNullOrEmpty(key))
        {
            return null;
        }

        if (cache.TryGetValue(key, out GameObject cached))
        {
            return cached;
        }

        GameObject loaded = Resources.Load<GameObject>(Root + key);
        cache[key] = loaded; // cache nulls too, so missing models cost one lookup
        return loaded;
    }

    private static void StripColliders(GameObject instance)
    {
        // This prototype is collider-free; keep imported art consistent.
        foreach (var collider in instance.GetComponentsInChildren<Collider>())
        {
            Object.Destroy(collider);
        }
    }
}
