using System.Collections;
using UnityEngine;

// Lightweight "game feel" helper. Provides quick scale punches, hops and a
// shared coroutine runner so any system can add juice without owning a
// MonoBehaviour. Everything is allocation-light and self-cleaning.
public static class PartyJuice
{
    private static PartyJuiceRunner runner;

    private static PartyJuiceRunner Runner
    {
        get
        {
            if (runner == null)
            {
                var go = new GameObject("PartyJuiceRunner");
                Object.DontDestroyOnLoad(go);
                go.hideFlags = HideFlags.HideInHierarchy;
                runner = go.AddComponent<PartyJuiceRunner>();
            }

            return runner;
        }
    }

    public static Coroutine Run(IEnumerator routine)
    {
        return Runner.StartCoroutine(routine);
    }

    // Quick overshoot-and-settle scale punch on a transform's local scale.
    public static void PopScale(Transform target, float strength = 0.35f, float duration = 0.32f)
    {
        if (target != null)
        {
            Runner.StartCoroutine(PopScaleRoutine(target, target.localScale, strength, duration));
        }
    }

    private static IEnumerator PopScaleRoutine(Transform target, Vector3 baseScale, float strength, float duration)
    {
        float t = 0f;
        while (t < duration && target != null)
        {
            t += Time.deltaTime;
            float n = Mathf.Clamp01(t / duration);
            // Decaying spring: overshoot then settle.
            float spring = Mathf.Sin(n * Mathf.PI * 2.2f) * (1f - n);
            target.localScale = baseScale * (1f + spring * strength);
            yield return null;
        }

        if (target != null)
        {
            target.localScale = baseScale;
        }
    }

    // A single arc hop in world space (height in metres).
    public static void Hop(Transform target, float height = 0.4f, float duration = 0.35f)
    {
        if (target != null)
        {
            Runner.StartCoroutine(HopRoutine(target, target.position, height, duration));
        }
    }

    private static IEnumerator HopRoutine(Transform target, Vector3 ground, float height, float duration)
    {
        float t = 0f;
        while (t < duration && target != null)
        {
            t += Time.deltaTime;
            float n = Mathf.Clamp01(t / duration);
            Vector3 p = target.position;
            p.y = ground.y + Mathf.Sin(n * Mathf.PI) * height;
            target.position = p;
            yield return null;
        }

        if (target != null)
        {
            Vector3 p = target.position;
            p.y = ground.y;
            target.position = p;
        }
    }
}

// Hidden runner that also drives camera shake decay.
public class PartyJuiceRunner : MonoBehaviour
{
}
