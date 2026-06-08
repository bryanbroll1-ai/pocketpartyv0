using UnityEngine;

// Central colour library for the procedurally generated look.
// Refreshed for a richer, more cohesive "premium toy" party-game feel:
// punchier player colours, less muddy environment tones, and a set of
// helper functions (Shade / Tint / Saturate) so every generator can derive
// consistent light/dark variants instead of hand-picking random colours.
public static class PartyArtPalette
{
    // Environment
    public static readonly Color Grass = new Color(0.42f, 0.80f, 0.36f);
    public static readonly Color GrassDeep = new Color(0.24f, 0.62f, 0.34f);
    public static readonly Color Dirt = new Color(0.80f, 0.58f, 0.34f);
    public static readonly Color Wood = new Color(0.72f, 0.48f, 0.28f);
    public static readonly Color Stone = new Color(0.78f, 0.82f, 0.88f);
    public static readonly Color Platform = new Color(1.00f, 0.84f, 0.46f);
    public static readonly Color KitchenCounter = new Color(1.00f, 0.78f, 0.56f);
    public static readonly Color Water = new Color(0.20f, 0.66f, 0.95f);
    public static readonly Color WaterDeep = new Color(0.09f, 0.40f, 0.66f);
    public static readonly Color Lava = new Color(1.00f, 0.42f, 0.12f);
    public static readonly Color Sand = new Color(0.96f, 0.86f, 0.60f);

    // UI / accents
    public static readonly Color UIPanel = new Color(1.00f, 0.97f, 0.89f);
    public static readonly Color Skin = new Color(0.98f, 0.78f, 0.62f);
    public static readonly Color DarkAccent = new Color(0.17f, 0.19f, 0.24f);
    public static readonly Color WhiteAccent = new Color(1.00f, 0.99f, 0.95f);
    public static readonly Color Metal = new Color(0.74f, 0.80f, 0.86f);
    public static readonly Color Sky = new Color(0.58f, 0.84f, 1.00f);
    public static readonly Color SkyHorizon = new Color(0.86f, 0.93f, 1.00f);
    public static readonly Color Gold = new Color(1.00f, 0.82f, 0.26f);

    // Player colours (vibrant, well separated in hue and value)
    public static readonly Color PlayerBlue = new Color(0.18f, 0.55f, 1.00f);
    public static readonly Color PlayerRed = new Color(1.00f, 0.30f, 0.36f);
    public static readonly Color PlayerGreen = new Color(0.26f, 0.82f, 0.40f);
    public static readonly Color PlayerYellow = new Color(1.00f, 0.80f, 0.16f);

    public static Color GetColor(PartyArtMaterialType type)
    {
        switch (type)
        {
            case PartyArtMaterialType.Grass: return Grass;
            case PartyArtMaterialType.Dirt: return Dirt;
            case PartyArtMaterialType.Wood: return Wood;
            case PartyArtMaterialType.Stone: return Stone;
            case PartyArtMaterialType.Platform: return Platform;
            case PartyArtMaterialType.KitchenCounter: return KitchenCounter;
            case PartyArtMaterialType.Water: return Water;
            case PartyArtMaterialType.Lava: return Lava;
            case PartyArtMaterialType.UIPanel: return UIPanel;
            case PartyArtMaterialType.PlayerBlue: return PlayerBlue;
            case PartyArtMaterialType.PlayerRed: return PlayerRed;
            case PartyArtMaterialType.PlayerGreen: return PlayerGreen;
            case PartyArtMaterialType.PlayerYellow: return PlayerYellow;
            case PartyArtMaterialType.Skin: return Skin;
            case PartyArtMaterialType.DarkAccent: return DarkAccent;
            case PartyArtMaterialType.WhiteAccent: return WhiteAccent;
            case PartyArtMaterialType.Metal: return Metal;
            case PartyArtMaterialType.Sky: return Sky;
            default: return WhiteAccent;
        }
    }

    public static Color PlayerColor(int index)
    {
        switch (index % 4)
        {
            case 0: return PlayerBlue;
            case 1: return PlayerRed;
            case 2: return PlayerGreen;
            default: return PlayerYellow;
        }
    }

    // ---- Derivation helpers (keep generated variants consistent) ----

    // Darken toward black while keeping a touch of the hue. amount 0..1.
    public static Color Shade(Color c, float amount)
    {
        float k = Mathf.Clamp01(1f - amount);
        return new Color(c.r * k, c.g * k, c.b * k, c.a);
    }

    // Lighten toward white. amount 0..1.
    public static Color Tint(Color c, float amount)
    {
        float t = Mathf.Clamp01(amount);
        return new Color(
            Mathf.Lerp(c.r, 1f, t),
            Mathf.Lerp(c.g, 1f, t),
            Mathf.Lerp(c.b, 1f, t),
            c.a);
    }

    // Push saturation up a little so generated props stay lively under fill light.
    public static Color Saturate(Color c, float amount)
    {
        Color.RGBToHSV(c, out float h, out float s, out float v);
        s = Mathf.Clamp01(s + amount);
        Color outColor = Color.HSVToRGB(h, s, v);
        outColor.a = c.a;
        return outColor;
    }
}
