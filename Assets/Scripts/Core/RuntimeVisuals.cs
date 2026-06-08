using UnityEngine;
using UnityEngine.UI;

public static class RuntimeVisuals
{
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
