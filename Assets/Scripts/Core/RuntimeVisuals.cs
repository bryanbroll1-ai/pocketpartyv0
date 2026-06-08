using UnityEngine;
using UnityEngine.UI;

public static class RuntimeVisuals
{
    private static Sprite cachedBlobSprite;
    private static Sprite cachedRoundedSprite;
    private static Material cachedShadowMaterial;

    // ---------------------------------------------------------------
    // Materials
    // ---------------------------------------------------------------

    public static Material CreateMaterial(string name, Color color)
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null)
        {
            shader = Shader.Find("Standard");
        }

        if (shader == null)
        {
            shader = Shader.Find("Unlit/Color");
        }

        var material = new Material(shader);
        material.name = name;
        SetMaterialColor(material, color);

        // Give every generated surface a soft, consistent stylized response
        // instead of the dead-flat default. This is the single biggest reason
        // the old look read as "cheap": pure lambert with no spec.
        material.enableInstancing = true;
        SetSmoothness(material, 0.22f);
        SetMetallic(material, 0f);
        return material;
    }

    // Full control variant for surfaces that want sheen / metal / glow.
    public static Material CreateMaterialAdvanced(string name, Color color, float smoothness, float metallic, Color emission)
    {
        Material material = CreateMaterial(name, color);
        SetSmoothness(material, smoothness);
        SetMetallic(material, metallic);
        if (emission.maxColorComponent > 0.001f)
        {
            SetEmission(material, emission);
        }

        return material;
    }

    public static void SetMaterialColor(Material material, Color color)
    {
        if (material == null)
        {
            return;
        }

        bool applied = false;
        if (material.HasProperty("_BaseColor"))
        {
            material.SetColor("_BaseColor", color);
            applied = true;
        }

        if (material.HasProperty("_Color"))
        {
            material.SetColor("_Color", color);
            applied = true;
        }

        if (!applied)
        {
            Debug.LogWarning($"Material {material.name} does not expose a known color property.");
        }
    }

    public static void SetSmoothness(Material material, float smoothness)
    {
        if (material == null)
        {
            return;
        }

        if (material.HasProperty("_Glossiness"))
        {
            material.SetFloat("_Glossiness", smoothness);
        }

        if (material.HasProperty("_Smoothness"))
        {
            material.SetFloat("_Smoothness", smoothness);
        }
    }

    public static void SetMetallic(Material material, float metallic)
    {
        if (material != null && material.HasProperty("_Metallic"))
        {
            material.SetFloat("_Metallic", metallic);
        }
    }

    public static void SetEmission(Material material, Color emission)
    {
        if (material == null || !material.HasProperty("_EmissionColor"))
        {
            return;
        }

        material.EnableKeyword("_EMISSION");
        material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
        material.SetColor("_EmissionColor", emission);
    }

    // ---------------------------------------------------------------
    // Atmosphere
    // ---------------------------------------------------------------

    // A warm-to-cool procedural gradient sky. Far cheaper to read than a
    // flat camera clear colour and instantly gives the board some depth.
    public static Material CreateGradientSkybox(Color skyTint, Color groundColor)
    {
        Shader shader = Shader.Find("Skybox/Procedural");
        if (shader == null)
        {
            return null;
        }

        var material = new Material(shader) { name = "Party Sky" };
        if (material.HasProperty("_SkyTint")) material.SetColor("_SkyTint", skyTint);
        if (material.HasProperty("_GroundColor")) material.SetColor("_GroundColor", groundColor);
        if (material.HasProperty("_AtmosphereThickness")) material.SetFloat("_AtmosphereThickness", 0.6f);
        if (material.HasProperty("_Exposure")) material.SetFloat("_Exposure", 1.15f);
        if (material.HasProperty("_SunSize")) material.SetFloat("_SunSize", 0.05f);
        return material;
    }

    // ---------------------------------------------------------------
    // Grounding shadows (the "blob" under every piece) - sells the 3D
    // ---------------------------------------------------------------

    // Attaches a soft, flat shadow blob under a world object. Returns the
    // transform so callers can scale it (e.g. shrink while a piece hops).
    public static Transform AttachGroundShadow(Transform parent, float radius, float yOffset = 0.02f)
    {
        var shadow = GameObject.CreatePrimitive(PrimitiveType.Quad);
        shadow.name = "Ground Shadow";
        Object.Destroy(shadow.GetComponent<Collider>());
        shadow.transform.SetParent(parent, false);
        shadow.transform.localPosition = new Vector3(0f, yOffset, 0f);
        shadow.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);
        shadow.transform.localScale = new Vector3(radius * 2f, radius * 2f, 1f);
        shadow.GetComponent<Renderer>().material = GetShadowMaterial();
        shadow.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        shadow.GetComponent<Renderer>().receiveShadows = false;
        return shadow.transform;
    }

    private static Material GetShadowMaterial()
    {
        if (cachedShadowMaterial != null)
        {
            return cachedShadowMaterial;
        }

        Shader shader = Shader.Find("Unlit/Transparent");
        if (shader == null)
        {
            shader = Shader.Find("Sprites/Default");
        }

        cachedShadowMaterial = new Material(shader) { name = "Soft Shadow" };
        // Bake the darkness into the texture so we don't depend on a _Color
        // tint that Unlit/Transparent does not expose.
        cachedShadowMaterial.mainTexture = CreateBlobTexture(128, 0.32f);
        if (cachedShadowMaterial.HasProperty("_Color"))
        {
            cachedShadowMaterial.SetColor("_Color", Color.white);
        }

        return cachedShadowMaterial;
    }

    private static Texture2D CreateBlobTexture(int size, float peakAlpha)
    {
        var texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        var center = new Vector2((size - 1) * 0.5f, (size - 1) * 0.5f);
        float radius = size * 0.5f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float d = Vector2.Distance(new Vector2(x, y), center) / radius;
                float a = Mathf.Clamp01(1f - d);
                a = a * a * peakAlpha;
                texture.SetPixel(x, y, new Color(0f, 0f, 0f, a));
            }
        }

        texture.Apply();
        texture.filterMode = FilterMode.Bilinear;
        texture.wrapMode = TextureWrapMode.Clamp;
        return texture;
    }

    // ---------------------------------------------------------------
    // UI sprites
    // ---------------------------------------------------------------

    public static Sprite CreateCircleSprite(int size = 96)
    {
        var texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        var center = new Vector2((size - 1) * 0.5f, (size - 1) * 0.5f);
        float radius = size * 0.46f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), center);
                float alpha = Mathf.Clamp01(radius + 1.5f - distance);
                texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
            }
        }

        texture.Apply();
        texture.filterMode = FilterMode.Bilinear;
        return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
    }

    // Soft radial blob with a smooth falloff (used for ground shadows + glows).
    public static Sprite GetBlobSprite(int size = 128)
    {
        if (cachedBlobSprite != null)
        {
            return cachedBlobSprite;
        }

        var texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        var center = new Vector2((size - 1) * 0.5f, (size - 1) * 0.5f);
        float radius = size * 0.5f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float d = Vector2.Distance(new Vector2(x, y), center) / radius;
                float a = Mathf.Clamp01(1f - d);
                a = a * a; // soft falloff
                texture.SetPixel(x, y, new Color(1f, 1f, 1f, a));
            }
        }

        texture.Apply();
        texture.filterMode = FilterMode.Bilinear;
        texture.wrapMode = TextureWrapMode.Clamp;
        cachedBlobSprite = Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
        return cachedBlobSprite;
    }

    // Rounded-rectangle sprite set up for 9-slice so corners stay crisp at any
    // size. Swapping panels/buttons to this instantly removes the "flat box" feel.
    public static Sprite GetRoundedRectSprite(int size = 64, int cornerRadius = 22)
    {
        if (cachedRoundedSprite != null)
        {
            return cachedRoundedSprite;
        }

        var texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        int r = Mathf.Clamp(cornerRadius, 1, size / 2);

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float alpha = RoundedRectAlpha(x, y, size, size, r);
                texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
            }
        }

        texture.Apply();
        texture.filterMode = FilterMode.Bilinear;
        texture.wrapMode = TextureWrapMode.Clamp;
        var border = new Vector4(r, r, r, r);
        cachedRoundedSprite = Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect, border);
        return cachedRoundedSprite;
    }

    private static float RoundedRectAlpha(int x, int y, int w, int h, int r)
    {
        float cx = Mathf.Clamp(x, r, w - 1 - r);
        float cy = Mathf.Clamp(y, r, h - 1 - r);
        float dist = Vector2.Distance(new Vector2(x, y), new Vector2(cx, cy));
        return Mathf.Clamp01(r - dist + 0.5f);
    }

    public static Image AddCircle(RectTransform parent, string name, Color color, Vector2 anchor, Vector2 size, Vector2 offset, Sprite circleSprite)
    {
        var circleObject = new GameObject(name, typeof(RectTransform), typeof(Image));
        circleObject.transform.SetParent(parent, false);
        var rect = circleObject.GetComponent<RectTransform>();
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.sizeDelta = size;
        rect.anchoredPosition = offset;

        var image = circleObject.GetComponent<Image>();
        image.sprite = circleSprite;
        image.color = color;
        image.raycastTarget = false;
        return image;
    }
}
