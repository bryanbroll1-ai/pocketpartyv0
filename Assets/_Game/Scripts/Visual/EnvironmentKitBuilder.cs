using UnityEngine;

public class EnvironmentKitBuilder : MonoBehaviour
{
    [SerializeField] private EnvironmentKitType kitType;
    [SerializeField] private bool buildOnAwake = true;
    [SerializeField] private bool includeLabel = true;

    public void Configure(EnvironmentKitType type, bool showLabel = true)
    {
        kitType = type;
        includeLabel = showLabel;
    }

    private void Awake()
    {
        if (buildOnAwake)
        {
            Build();
        }
    }

    public void Build()
    {
        ClearChildren();
        switch (kitType)
        {
            case EnvironmentKitType.FarmRail:
                BuildFarmRail();
                break;
            case EnvironmentKitType.SidePlatform:
                BuildSidePlatform();
                break;
            case EnvironmentKitType.StageBoss:
                BuildStageBoss();
                break;
            case EnvironmentKitType.SkyArena:
                BuildSkyArena();
                break;
            case EnvironmentKitType.FieldAnimal:
                BuildFieldAnimal();
                break;
            case EnvironmentKitType.Bridge:
                BuildBridge();
                break;
            case EnvironmentKitType.LavaArena:
                BuildLavaArena();
                break;
            default:
                BuildKitchen();
                break;
        }

        if (includeLabel)
        {
            AddLabel(kitType.ToString());
        }
    }

    private void BuildKitchen()
    {
        AddKitBase(PartyArtMaterialType.KitchenCounter);
        AddCube("Kitchen Floor", new Vector3(0f, 0.02f, 0f), new Vector3(4.45f, 0.14f, 3.05f), PartyArtMaterialType.Platform);
        AddCube("Kitchen Wall", new Vector3(0f, 1.30f, 1.46f), new Vector3(4.45f, 2.45f, 0.18f), PartyArtMaterialType.UIPanel);
        AddCube("Window Blue Glass", new Vector3(-1.25f, 1.55f, 1.33f), new Vector3(1.05f, 0.72f, 0.08f), PartyArtMaterialType.Water);
        AddCube("Back Cabinets", new Vector3(0.55f, 2.15f, 1.30f), new Vector3(3.15f, 0.54f, 0.26f), PartyArtMaterialType.Wood);
        AddCube("Work Counter", new Vector3(0f, 0.62f, 0.55f), new Vector3(4.45f, 0.54f, 0.92f), PartyArtMaterialType.KitchenCounter);
        for (int i = 0; i < 4; i++)
        {
            float x = -1.65f + i * 1.1f;
            AddCube("Player Station", new Vector3(x, 0.96f, 0.12f), new Vector3(0.78f, 0.08f, 0.58f), PlayerMaterial(i));
            AddCube("Cutting Board", new Vector3(x, 1.05f, -0.12f), new Vector3(0.50f, 0.05f, 0.34f), PartyArtMaterialType.Wood);
            AddBowl(new Vector3(x + 0.24f, 1.17f, 0.16f), PlayerColor(i));
            AddSphere("Food Placeholder", new Vector3(x - 0.20f, 1.17f, 0.14f), Vector3.one * 0.12f, PlayerColor((i + 1) % 4));
        }
    }

    private void BuildFarmRail()
    {
        AddKitBase(PartyArtMaterialType.Grass);
        AddCube("Farm Grass", new Vector3(0f, 0.02f, 0f), new Vector3(5.0f, 0.14f, 3.45f), PartyArtMaterialType.Grass);
        AddCube("Dirt Road", new Vector3(0f, 0.08f, 0f), new Vector3(5.0f, 0.06f, 1.0f), PartyArtMaterialType.Dirt);
        AddCube("Rail A", new Vector3(0f, 0.18f, -0.28f), new Vector3(4.7f, 0.06f, 0.07f), PartyArtMaterialType.Metal);
        AddCube("Rail B", new Vector3(0f, 0.18f, 0.28f), new Vector3(4.7f, 0.06f, 0.07f), PartyArtMaterialType.Metal);
        AddCube("Transport Cart", new Vector3(-1.25f, 0.44f, 0f), new Vector3(0.92f, 0.46f, 0.66f), PartyArtMaterialType.Wood);
        AddSphere("Cart Wheel L", new Vector3(-1.56f, 0.22f, -0.36f), Vector3.one * 0.16f, PartyArtPalette.DarkAccent);
        AddSphere("Cart Wheel R", new Vector3(-0.94f, 0.22f, -0.36f), Vector3.one * 0.16f, PartyArtPalette.DarkAccent);
        AddSphere("Cart Wheel L", new Vector3(-1.56f, 0.22f, 0.36f), Vector3.one * 0.16f, PartyArtPalette.DarkAccent);
        AddSphere("Cart Wheel R", new Vector3(-0.94f, 0.22f, 0.36f), Vector3.one * 0.16f, PartyArtPalette.DarkAccent);
        AddFence(-2.35f);
        AddFence(2.35f);
        AddTree(new Vector3(-1.8f, 0f, 1.25f));
        AddTree(new Vector3(1.8f, 0f, -1.25f));
        AddBush(new Vector3(1.95f, 0f, 1.15f));
        AddFlower(new Vector3(-2.25f, 0f, -1.05f), PartyArtPalette.PlayerRed);
        AddFlower(new Vector3(2.15f, 0f, 1.05f), PartyArtPalette.PlayerYellow);
    }

    private void BuildSidePlatform()
    {
        AddKitBase(PartyArtMaterialType.Water);
        AddCube("Side Wall", new Vector3(0f, 1.26f, 1.0f), new Vector3(4.8f, 2.35f, 0.20f), PartyArtMaterialType.Water);
        AddCube("Horizontal Platform", new Vector3(0f, 0.28f, -0.25f), new Vector3(4.25f, 0.26f, 1.55f), PartyArtMaterialType.Stone);
        AddCube("Fall Object A", new Vector3(-1.25f, 1.95f, -0.25f), new Vector3(0.38f, 0.38f, 0.38f), PartyArtMaterialType.PlayerRed);
        AddCube("Fall Object B", new Vector3(0.25f, 2.25f, -0.25f), new Vector3(0.38f, 0.38f, 0.38f), PartyArtMaterialType.PlayerYellow);
        AddVine(new Vector3(-2.15f, 0.28f, 0.78f));
        AddVine(new Vector3(2.15f, 0.28f, 0.78f));
        AddCube("Safe Run Lane", new Vector3(0f, 0.48f, -0.26f), new Vector3(3.55f, 0.06f, 0.38f), PartyArtMaterialType.Platform);
        AddCube("Readable Lane Stripe", new Vector3(0f, 0.54f, -0.26f), new Vector3(3.25f, 0.035f, 0.08f), PartyArtMaterialType.WhiteAccent);
    }

    private void BuildStageBoss()
    {
        AddKitBase(PartyArtMaterialType.PlayerRed);
        AddCube("Stage Floor", new Vector3(0f, 0.02f, 0f), new Vector3(5.0f, 0.18f, 3.05f), PartyArtMaterialType.Wood);
        AddCube("Warm Backdrop", new Vector3(0f, 1.33f, 1.35f), new Vector3(5.0f, 2.55f, 0.22f), PartyArtMaterialType.PlayerRed);
        AddCube("Curtain Header", new Vector3(0f, 2.65f, 1.24f), new Vector3(5.3f, 0.34f, 0.36f), PartyArtMaterialType.PlayerYellow);
        AddSphere("Friendly Monster Body", new Vector3(0f, 1.12f, 0.70f), new Vector3(0.92f, 0.72f, 0.48f), PartyArtPalette.PlayerGreen);
        AddSphere("Friendly Monster Eye L", new Vector3(-0.24f, 1.25f, 0.30f), Vector3.one * 0.08f, PartyArtPalette.WhiteAccent);
        AddSphere("Friendly Monster Eye R", new Vector3(0.24f, 1.25f, 0.30f), Vector3.one * 0.08f, PartyArtPalette.WhiteAccent);
        AddSphere("Friendly Monster Nose", new Vector3(0f, 1.08f, 0.25f), Vector3.one * 0.07f, PartyArtPalette.PlayerYellow);
        AddCube("Friendly Smile", new Vector3(0f, 0.96f, 0.25f), new Vector3(0.36f, 0.035f, 0.035f), PartyArtMaterialType.DarkAccent);
        for (int i = 0; i < 4; i++)
        {
            AddCube("Signal Pad", new Vector3(-1.65f + i * 1.1f, 0.20f, -0.85f), new Vector3(0.72f, 0.08f, 0.56f), PlayerMaterial(i));
        }
    }

    private void BuildSkyArena()
    {
        AddKitBase(PartyArtMaterialType.Sky);
        AddCube("Sky Backdrop Pad", new Vector3(0f, -0.02f, 0f), new Vector3(5.2f, 0.08f, 3.4f), PartyArtMaterialType.Sky);
        AddCylinder("Floating Platform", new Vector3(0f, 0.22f, 0f), new Vector3(1.85f, 0.28f, 1.85f), PartyArtMaterialType.Platform);
        AddCube("Zone Blue", new Vector3(-0.85f, 0.55f, 0f), new Vector3(0.75f, 0.08f, 1.30f), PartyArtMaterialType.PlayerBlue);
        AddCube("Zone Red", new Vector3(0.85f, 0.55f, 0f), new Vector3(0.75f, 0.08f, 1.30f), PartyArtMaterialType.PlayerRed);
        AddCube("Boundary Ring N", new Vector3(0f, 0.66f, 0.98f), new Vector3(3.3f, 0.08f, 0.10f), PartyArtMaterialType.WhiteAccent);
        AddCube("Boundary Ring S", new Vector3(0f, 0.66f, -0.98f), new Vector3(3.3f, 0.08f, 0.10f), PartyArtMaterialType.WhiteAccent);
        AddCloud(new Vector3(-1.9f, 0.62f, 1.2f));
        AddCloud(new Vector3(2.0f, 0.70f, -1.25f));
    }

    private void BuildFieldAnimal()
    {
        AddKitBase(PartyArtMaterialType.Grass);
        AddCube("Field Grass", new Vector3(0f, 0.02f, 0f), new Vector3(5.15f, 0.14f, 3.45f), PartyArtMaterialType.Grass);
        AddCube("Winding Path", new Vector3(0f, 0.08f, 0f), new Vector3(1.6f, 0.07f, 3.5f), PartyArtMaterialType.Dirt);
        AddCube("Stone Fence L", new Vector3(-1.05f, 0.28f, 0f), new Vector3(0.22f, 0.38f, 3.25f), PartyArtMaterialType.Stone);
        AddCube("Stone Fence R", new Vector3(1.05f, 0.28f, 0f), new Vector3(0.22f, 0.38f, 3.25f), PartyArtMaterialType.Stone);
        for (int i = 0; i < 5; i++)
        {
            AddAnimal(new Vector3(-0.45f + (i % 2) * 0.9f, 0.02f, -1.35f + i * 0.62f));
        }
        AddTree(new Vector3(-2.0f, 0f, 1.2f));
        AddBush(new Vector3(2.1f, 0f, -1.2f));
        AddFlower(new Vector3(-1.85f, 0f, -1.35f), PartyArtPalette.PlayerYellow);
        AddFlower(new Vector3(1.85f, 0f, 1.35f), PartyArtPalette.PlayerBlue);
    }

    private void BuildBridge()
    {
        AddKitBase(PartyArtMaterialType.Water);
        AddCube("Water Under Bridge", new Vector3(0f, -0.08f, 0f), new Vector3(5.15f, 0.10f, 3.45f), PartyArtMaterialType.Water);
        AddCube("Wood Bridge Deck", new Vector3(0f, 0.22f, 0f), new Vector3(4.5f, 0.22f, 1.45f), PartyArtMaterialType.Wood);
        for (int i = 0; i < 5; i++)
        {
            AddCube("Bridge Plank Highlight", new Vector3(-1.8f + i * 0.9f, 0.36f, 0f), new Vector3(0.06f, 0.04f, 1.30f), PartyArtMaterialType.Platform);
        }
        AddCube("Left Rail", new Vector3(0f, 0.62f, -0.82f), new Vector3(4.55f, 0.16f, 0.12f), PartyArtMaterialType.Wood);
        AddCube("Right Rail", new Vector3(0f, 0.62f, 0.82f), new Vector3(4.55f, 0.16f, 0.12f), PartyArtMaterialType.Wood);
        AddCube("Run Lane A", new Vector3(0f, 0.40f, -0.28f), new Vector3(4.0f, 0.06f, 0.12f), PartyArtMaterialType.Platform);
        AddCube("Run Lane B", new Vector3(0f, 0.40f, 0.28f), new Vector3(4.0f, 0.06f, 0.12f), PartyArtMaterialType.Platform);
        AddCube("Soft Obstacle", new Vector3(-1.1f, 0.60f, 0.10f), new Vector3(0.44f, 0.32f, 0.36f), PartyArtMaterialType.PlayerGreen);
        AddCube("Soft Obstacle", new Vector3(1.2f, 0.60f, -0.12f), new Vector3(0.44f, 0.32f, 0.36f), PartyArtMaterialType.PlayerRed);
    }

    private void BuildLavaArena()
    {
        AddKitBase(PartyArtMaterialType.Lava);
        AddCube("Cartoon Lava", new Vector3(0f, -0.08f, 0f), new Vector3(5.15f, 0.12f, 3.45f), PartyArtMaterialType.Lava);
        AddCylinder("Round Stone Platform", new Vector3(0f, 0.32f, 0f), new Vector3(1.55f, 0.40f, 1.55f), PartyArtMaterialType.Stone);
        AddCylinder("Warm Rim", new Vector3(0f, 0.75f, 0f), new Vector3(1.68f, 0.07f, 1.68f), PartyArtMaterialType.Platform);
        AddCylinder("Safe Center", new Vector3(0f, 0.84f, 0f), new Vector3(0.70f, 0.04f, 0.70f), PartyArtMaterialType.WhiteAccent);
        AddSphere("Hot Bubble A", new Vector3(-1.85f, -0.05f, 0.72f), Vector3.one * 0.18f, PartyArtPalette.Platform);
        AddSphere("Hot Bubble B", new Vector3(1.75f, -0.05f, -0.80f), Vector3.one * 0.16f, PartyArtPalette.Platform);
        AddSphere("Cartoon Rock", new Vector3(-2.2f, 0.06f, -0.95f), new Vector3(0.34f, 0.24f, 0.30f), PartyArtPalette.Stone);
    }

    private void AddLabel(string text)
    {
        AddCube("Label Plate", new Vector3(0f, 2.42f, -1.62f), new Vector3(2.35f, 0.08f, 0.36f), PartyArtMaterialType.UIPanel);
        GameObject labelObject = new GameObject($"{text} Label");
        labelObject.transform.SetParent(transform, false);
        labelObject.transform.localPosition = new Vector3(0f, 2.53f, -1.68f);
        labelObject.transform.localRotation = Quaternion.Euler(62f, 0f, 0f);
        TextMesh mesh = labelObject.AddComponent<TextMesh>();
        mesh.text = text;
        mesh.fontSize = 44;
        mesh.characterSize = 0.095f;
        mesh.anchor = TextAnchor.MiddleCenter;
        mesh.alignment = TextAlignment.Center;
        mesh.color = PartyArtPalette.DarkAccent;
    }

    private void AddKitBase(PartyArtMaterialType accent)
    {
        AddCube("Kit Rounded Base", new Vector3(0f, -0.11f, 0f), new Vector3(5.45f, 0.10f, 3.75f), PartyArtMaterialType.WhiteAccent);
        AddCube("Kit Accent Trim Front", new Vector3(0f, -0.02f, -1.87f), new Vector3(5.15f, 0.08f, 0.18f), accent);
        AddCube("Kit Accent Trim Back", new Vector3(0f, -0.02f, 1.87f), new Vector3(5.15f, 0.08f, 0.18f), accent);
    }

    private void AddFence(float z)
    {
        AddCube("Fence Rail", new Vector3(0f, 0.46f, z), new Vector3(4.5f, 0.12f, 0.10f), PartyArtMaterialType.Wood);
        for (int i = 0; i < 5; i++)
        {
            AddCube("Fence Post", new Vector3(-2.0f + i, 0.34f, z), new Vector3(0.12f, 0.46f, 0.12f), PartyArtMaterialType.Wood);
        }
    }

    private void AddTree(Vector3 position)
    {
        AddCylinder("Tree Trunk", position + new Vector3(0f, 0.36f, 0f), new Vector3(0.12f, 0.36f, 0.12f), PartyArtMaterialType.Wood);
        AddSphere("Tree Top", position + new Vector3(0f, 0.92f, 0f), new Vector3(0.46f, 0.40f, 0.46f), PartyArtPalette.Grass);
    }

    private void AddBush(Vector3 position)
    {
        AddSphere("Round Bush", position + new Vector3(0f, 0.24f, 0f), new Vector3(0.42f, 0.24f, 0.34f), PartyArtPalette.Grass);
    }

    private void AddFlower(Vector3 position, Color color)
    {
        AddCylinder("Flower Stem", position + new Vector3(0f, 0.16f, 0f), new Vector3(0.035f, 0.16f, 0.035f), PartyArtMaterialType.Grass);
        AddSphere("Flower Head", position + new Vector3(0f, 0.36f, 0f), Vector3.one * 0.11f, color);
    }

    private void AddAnimal(Vector3 position)
    {
        AddSphere("Cute Animal Body", position + new Vector3(0f, 0.28f, 0f), new Vector3(0.34f, 0.22f, 0.26f), PartyArtPalette.WhiteAccent);
        AddSphere("Cute Animal Head", position + new Vector3(0.28f, 0.33f, 0f), new Vector3(0.14f, 0.13f, 0.13f), PartyArtPalette.WhiteAccent);
        AddSphere("Cute Animal Nose", position + new Vector3(0.40f, 0.32f, -0.02f), Vector3.one * 0.045f, PartyArtPalette.DarkAccent);
    }

    private void AddCloud(Vector3 position)
    {
        AddSphere("Cloud A", position, new Vector3(0.42f, 0.20f, 0.20f), PartyArtPalette.WhiteAccent);
        AddSphere("Cloud B", position + new Vector3(0.28f, 0.03f, 0f), new Vector3(0.30f, 0.16f, 0.16f), PartyArtPalette.WhiteAccent);
        AddSphere("Cloud C", position + new Vector3(-0.28f, 0.02f, 0f), new Vector3(0.28f, 0.15f, 0.15f), PartyArtPalette.WhiteAccent);
    }

    private void AddVine(Vector3 position)
    {
        AddCylinder("Vine Stem", position + new Vector3(0f, 0.70f, 0f), new Vector3(0.05f, 0.72f, 0.05f), PartyArtMaterialType.Grass);
        AddCube("Vine Leaf", position + new Vector3(-0.16f, 0.94f, 0f), new Vector3(0.28f, 0.07f, 0.16f), PartyArtMaterialType.Grass);
        AddCube("Vine Leaf", position + new Vector3(0.16f, 0.54f, 0f), new Vector3(0.28f, 0.07f, 0.16f), PartyArtMaterialType.Grass);
    }

    private void AddBowl(Vector3 position, Color color)
    {
        AddCylinder("Bowl", position, new Vector3(0.20f, 0.07f, 0.20f), PartyArtMaterials.CreateTinted("Bowl", color));
    }

    private GameObject AddCube(string name, Vector3 localPosition, Vector3 localScale, PartyArtMaterialType materialType)
    {
        return AddPrimitive(name, PrimitiveType.Cube, localPosition, localScale, PartyArtMaterials.Get(materialType));
    }

    private GameObject AddCube(string name, Vector3 localPosition, Vector3 localScale, Material material)
    {
        return AddPrimitive(name, PrimitiveType.Cube, localPosition, localScale, material);
    }

    private GameObject AddCylinder(string name, Vector3 localPosition, Vector3 localScale, PartyArtMaterialType materialType)
    {
        return AddPrimitive(name, PrimitiveType.Cylinder, localPosition, localScale, PartyArtMaterials.Get(materialType));
    }

    private GameObject AddCylinder(string name, Vector3 localPosition, Vector3 localScale, Material material)
    {
        return AddPrimitive(name, PrimitiveType.Cylinder, localPosition, localScale, material);
    }

    private GameObject AddSphere(string name, Vector3 localPosition, Vector3 localScale, Color color)
    {
        return AddPrimitive(name, PrimitiveType.Sphere, localPosition, localScale, PartyArtMaterials.CreateTinted(name, color));
    }

    private GameObject AddPrimitive(string name, PrimitiveType primitiveType, Vector3 localPosition, Vector3 localScale, Material material)
    {
        GameObject instance = GameObject.CreatePrimitive(primitiveType);
        instance.name = name;
        instance.transform.SetParent(transform, false);
        instance.transform.localPosition = localPosition;
        instance.transform.localScale = localScale;
        instance.GetComponent<Renderer>().material = material;
        Collider collider = instance.GetComponent<Collider>();
        if (collider != null)
        {
            Destroy(collider);
        }

        return instance;
    }

    private void ClearChildren()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }

    private PartyArtMaterialType PlayerMaterial(int index)
    {
        switch (index % 4)
        {
            case 0:
                return PartyArtMaterialType.PlayerBlue;
            case 1:
                return PartyArtMaterialType.PlayerRed;
            case 2:
                return PartyArtMaterialType.PlayerGreen;
            default:
                return PartyArtMaterialType.PlayerYellow;
        }
    }

    private Color PlayerColor(int index)
    {
        return PartyArtPalette.PlayerColor(index);
    }
}
