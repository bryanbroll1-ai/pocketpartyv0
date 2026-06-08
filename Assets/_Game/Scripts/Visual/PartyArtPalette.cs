using UnityEngine;

public static class PartyArtPalette
{
    public static readonly Color Grass = new Color(0.47f, 0.84f, 0.39f);
    public static readonly Color Dirt = new Color(0.84f, 0.64f, 0.38f);
    public static readonly Color Wood = new Color(0.76f, 0.52f, 0.30f);
    public static readonly Color Stone = new Color(0.74f, 0.79f, 0.83f);
    public static readonly Color Platform = new Color(1.00f, 0.86f, 0.48f);
    public static readonly Color KitchenCounter = new Color(1.00f, 0.80f, 0.58f);
    public static readonly Color Water = new Color(0.23f, 0.72f, 0.96f);
    public static readonly Color Lava = new Color(1.00f, 0.48f, 0.14f);
    public static readonly Color UIPanel = new Color(1.00f, 0.97f, 0.89f);
    public static readonly Color PlayerBlue = new Color(0.15f, 0.57f, 1.00f);
    public static readonly Color PlayerRed = new Color(1.00f, 0.25f, 0.32f);
    public static readonly Color PlayerGreen = new Color(0.25f, 0.80f, 0.36f);
    public static readonly Color PlayerYellow = new Color(1.00f, 0.78f, 0.12f);
    public static readonly Color Skin = new Color(0.96f, 0.76f, 0.58f);
    public static readonly Color DarkAccent = new Color(0.22f, 0.24f, 0.28f);
    public static readonly Color WhiteAccent = new Color(1.00f, 0.98f, 0.92f);
    public static readonly Color Metal = new Color(0.70f, 0.78f, 0.84f);
    public static readonly Color Sky = new Color(0.64f, 0.88f, 1.00f);

    public static Color GetColor(PartyArtMaterialType type)
    {
        switch (type)
        {
            case PartyArtMaterialType.Grass:
                return Grass;
            case PartyArtMaterialType.Dirt:
                return Dirt;
            case PartyArtMaterialType.Wood:
                return Wood;
            case PartyArtMaterialType.Stone:
                return Stone;
            case PartyArtMaterialType.Platform:
                return Platform;
            case PartyArtMaterialType.KitchenCounter:
                return KitchenCounter;
            case PartyArtMaterialType.Water:
                return Water;
            case PartyArtMaterialType.Lava:
                return Lava;
            case PartyArtMaterialType.UIPanel:
                return UIPanel;
            case PartyArtMaterialType.PlayerBlue:
                return PlayerBlue;
            case PartyArtMaterialType.PlayerRed:
                return PlayerRed;
            case PartyArtMaterialType.PlayerGreen:
                return PlayerGreen;
            case PartyArtMaterialType.PlayerYellow:
                return PlayerYellow;
            case PartyArtMaterialType.Skin:
                return Skin;
            case PartyArtMaterialType.DarkAccent:
                return DarkAccent;
            case PartyArtMaterialType.WhiteAccent:
                return WhiteAccent;
            case PartyArtMaterialType.Metal:
                return Metal;
            case PartyArtMaterialType.Sky:
                return Sky;
            default:
                return WhiteAccent;
        }
    }

    public static Color PlayerColor(int index)
    {
        switch (index % 4)
        {
            case 0:
                return PlayerBlue;
            case 1:
                return PlayerRed;
            case 2:
                return PlayerGreen;
            default:
                return PlayerYellow;
        }
    }
}
