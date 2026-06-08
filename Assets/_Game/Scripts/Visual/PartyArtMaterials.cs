using System.Collections.Generic;
using UnityEngine;

public static class PartyArtMaterials
{
    private static readonly Dictionary<PartyArtMaterialType, Material> Materials = new Dictionary<PartyArtMaterialType, Material>();

    public static Material Get(PartyArtMaterialType type)
    {
        if (Materials.TryGetValue(type, out Material material) && material != null)
        {
            return material;
        }

        material = Create(type);
        Materials[type] = material;
        return material;
    }

    public static Material Create(PartyArtMaterialType type)
    {
        Color color = PartyArtPalette.GetColor(type);
        Material material = RuntimeVisuals.CreateMaterial($"Party {type}", color);
        material.enableInstancing = true;
        ApplyCartoonSurface(material, type, color);
        return material;
    }

    public static Material CreateTinted(string name, Color color)
    {
        Material material = RuntimeVisuals.CreateMaterial(name, color);
        material.enableInstancing = true;
        ApplySmoothness(material, 0.42f);
        return material;
    }

    private static void ApplyCartoonSurface(Material material, PartyArtMaterialType type, Color color)
    {
        ApplySmoothness(material, type == PartyArtMaterialType.Water ? 0.78f : 0.44f);
        if (type == PartyArtMaterialType.Lava)
        {
            ApplyEmission(material, new Color(1f, 0.34f, 0.08f) * 0.45f);
        }
        else if (type == PartyArtMaterialType.Water)
        {
            ApplyEmission(material, new Color(0.10f, 0.35f, 0.55f) * 0.18f);
        }
        else if (type == PartyArtMaterialType.PlayerBlue || type == PartyArtMaterialType.PlayerRed || type == PartyArtMaterialType.PlayerGreen || type == PartyArtMaterialType.PlayerYellow)
        {
            ApplyEmission(material, color * 0.12f);
        }
        else if (type == PartyArtMaterialType.WhiteAccent || type == PartyArtMaterialType.Platform || type == PartyArtMaterialType.UIPanel)
        {
            ApplyEmission(material, color * 0.05f);
        }
    }

    private static void ApplySmoothness(Material material, float smoothness)
    {
        if (material.HasProperty("_Glossiness"))
        {
            material.SetFloat("_Glossiness", smoothness);
        }

        if (material.HasProperty("_Smoothness"))
        {
            material.SetFloat("_Smoothness", smoothness);
        }
    }

    private static void ApplyEmission(Material material, Color emissionColor)
    {
        if (!material.HasProperty("_EmissionColor"))
        {
            return;
        }

        material.EnableKeyword("_EMISSION");
        material.SetColor("_EmissionColor", emissionColor);
    }
}
