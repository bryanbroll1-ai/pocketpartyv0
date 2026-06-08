using UnityEngine;

// Runtime particle bursts built entirely in code (no prefabs / assets).
// Used for coin gains, minigame wins, landings, etc. Each call spawns a
// short-lived, self-destroying particle system.
public static class PartyFx
{
    private static Material cachedParticleMaterial;

    private static Material ParticleMaterial
    {
        get
        {
            if (cachedParticleMaterial != null)
            {
                return cachedParticleMaterial;
            }

            Shader shader = Shader.Find("Particles/Standard Unlit");
            if (shader == null) shader = Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply");
            if (shader == null) shader = Shader.Find("Sprites/Default");

            cachedParticleMaterial = new Material(shader) { name = "Party FX Particle" };
            cachedParticleMaterial.mainTexture = RuntimeVisuals.GetBlobSprite().texture;
            return cachedParticleMaterial;
        }
    }

    // Golden coins popping up out of a tile. Amount scales the count a little.
    public static void CoinBurst(Vector3 worldPosition, int amount)
    {
        int count = Mathf.Clamp(8 + Mathf.Abs(amount) * 3, 10, 28);
        Spawn(worldPosition + Vector3.up * 0.4f, count, PartyArtPalette.Gold, 0.10f, 0.16f,
            speed: 3.2f, gravity: 2.6f, lifetime: 0.85f, coneAngle: 22f, emissive: true);
    }

    // Multicolour celebration burst for wins.
    public static void Confetti(Vector3 worldPosition)
    {
        SpawnMulticolor(worldPosition + Vector3.up * 0.6f, 40, 0.09f, 0.15f, 4.5f, 2.0f, 1.6f, 32f);
    }

    // Soft dust puff for landings and coin losses.
    public static void Poof(Vector3 worldPosition, Color color)
    {
        Spawn(worldPosition + Vector3.up * 0.1f, 12, color, 0.14f, 0.24f,
            speed: 1.3f, gravity: -0.2f, lifetime: 0.55f, coneAngle: 55f, emissive: false);
    }

    // Small upward sparkles, e.g. to flag a special tile.
    public static void Sparkle(Vector3 worldPosition, Color color)
    {
        Spawn(worldPosition + Vector3.up * 0.5f, 14, color, 0.06f, 0.11f,
            speed: 1.8f, gravity: 0.4f, lifetime: 0.9f, coneAngle: 35f, emissive: true);
    }

    private static void Spawn(Vector3 position, int count, Color color, float minSize, float maxSize,
        float speed, float gravity, float lifetime, float coneAngle, bool emissive)
    {
        GameObject go = NewSystem(position, out ParticleSystem ps);

        var main = ps.main;
        main.loop = false;
        main.playOnAwake = true;
        main.duration = lifetime;
        main.startLifetime = lifetime;
        main.startSpeed = speed;
        main.startSize = new ParticleSystem.MinMaxCurve(minSize, maxSize);
        main.gravityModifier = gravity;
        main.startColor = emissive ? color * 1.6f : color;
        main.maxParticles = count + 4;
        main.stopAction = ParticleSystemStopAction.Destroy;

        ConfigureBurst(ps, count, coneAngle);
        FadeOut(ps);

        go.SetActive(true);
        Object.Destroy(go, lifetime + 0.6f);
    }

    private static void SpawnMulticolor(Vector3 position, int count, float minSize, float maxSize,
        float speed, float gravity, float lifetime, float coneAngle)
    {
        GameObject go = NewSystem(position, out ParticleSystem ps);

        var main = ps.main;
        main.loop = false;
        main.playOnAwake = true;
        main.duration = lifetime;
        main.startLifetime = lifetime;
        main.startSpeed = new ParticleSystem.MinMaxCurve(speed * 0.6f, speed);
        main.startSize = new ParticleSystem.MinMaxCurve(minSize, maxSize);
        main.gravityModifier = gravity;
        main.maxParticles = count + 4;
        main.stopAction = ParticleSystemStopAction.Destroy;

        var grad = new ParticleSystem.MinMaxGradient(MakeConfettiGradient());
        grad.mode = ParticleSystemGradientMode.RandomColor;
        main.startColor = grad;

        ConfigureBurst(ps, count, coneAngle);
        FadeOut(ps);

        go.SetActive(true);
        Object.Destroy(go, lifetime + 0.6f);
    }

    private static GameObject NewSystem(Vector3 position, out ParticleSystem ps)
    {
        var go = new GameObject("Party FX");
        go.SetActive(false); // configure before Awake plays it
        go.transform.position = position;
        ps = go.AddComponent<ParticleSystem>();

        var renderer = go.GetComponent<ParticleSystemRenderer>();
        renderer.material = ParticleMaterial;
        renderer.renderMode = ParticleSystemRenderMode.Billboard;
        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        renderer.receiveShadows = false;
        return go;
    }

    private static void ConfigureBurst(ParticleSystem ps, int count, float coneAngle)
    {
        var emission = ps.emission;
        emission.rateOverTime = 0f;
        emission.SetBursts(new[] { new ParticleSystem.Burst(0f, (short)count) });

        var shape = ps.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = coneAngle;
        shape.radius = 0.08f;
        shape.rotation = new Vector3(-90f, 0f, 0f); // burst upward
    }

    private static void FadeOut(ParticleSystem ps)
    {
        var col = ps.colorOverLifetime;
        col.enabled = true;
        var gradient = new Gradient();
        gradient.SetKeys(
            new[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(Color.white, 1f) },
            new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 0.6f), new GradientAlphaKey(0f, 1f) });
        col.color = gradient;

        var size = ps.sizeOverLifetime;
        size.enabled = true;
        size.size = new ParticleSystem.MinMaxCurve(1f, new AnimationCurve(
            new Keyframe(0f, 0.6f), new Keyframe(0.25f, 1f), new Keyframe(1f, 0.2f)));
    }

    private static Gradient MakeConfettiGradient()
    {
        var g = new Gradient();
        g.SetKeys(
            new[]
            {
                new GradientColorKey(PartyArtPalette.PlayerBlue, 0.0f),
                new GradientColorKey(PartyArtPalette.PlayerRed, 0.33f),
                new GradientColorKey(PartyArtPalette.PlayerGreen, 0.66f),
                new GradientColorKey(PartyArtPalette.PlayerYellow, 1.0f)
            },
            new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) });
        return g;
    }
}
