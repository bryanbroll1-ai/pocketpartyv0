using UnityEngine;

public class MinigameTemplateSceneBootstrapper : MonoBehaviour
{
    private MinigameManager manager;
    private MinigamePlayerSpawner spawner;
    private MinigameUIController ui;
    private MinigameInputRouter inputRouter;
    private TemplateTapTestMinigame tapTest;

    private void Awake()
    {
        SetupLighting();
        SetupCamera();
        BuildArena();
        BuildRuntimeObjects();
    }

    private void Start()
    {
        StartTemplateTapTest();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            StartTemplateTapTest();
        }
    }

    private void StartTemplateTapTest()
    {
        manager.StartTemplateMinigame(tapTest, spawner, ui, inputRouter, result =>
        {
            Debug.Log($"Template minigame completed: {result.Title}");
        });
    }

    private void SetupLighting()
    {
        var lightingObject = new GameObject("Template Lighting");
        lightingObject.AddComponent<LightingPresetController>();
    }

    private void SetupCamera()
    {
        var cameraObject = new GameObject("Template Camera");
        cameraObject.tag = "MainCamera";
        Camera camera = cameraObject.AddComponent<Camera>();
        camera.backgroundColor = PartyArtPalette.Sky;
        camera.fieldOfView = 46f;
        camera.nearClipPlane = 0.1f;
        camera.farClipPlane = 90f;
        cameraObject.AddComponent<CameraPresetController>();
    }

    private void BuildRuntimeObjects()
    {
        manager = new GameObject("Template Minigame Manager").AddComponent<MinigameManager>();
        spawner = new GameObject("Template Player Spawner").AddComponent<MinigamePlayerSpawner>();
        spawner.transform.position = Vector3.zero;
        CreateSpawnPoint(spawner.transform, 0, new Vector3(-1.8f, 0.05f, -1.05f), PartyArtMaterialType.PlayerBlue);
        CreateSpawnPoint(spawner.transform, 1, new Vector3(-0.6f, 0.05f, -1.05f), PartyArtMaterialType.PlayerRed);
        CreateSpawnPoint(spawner.transform, 2, new Vector3(0.6f, 0.05f, -1.05f), PartyArtMaterialType.PlayerGreen);
        CreateSpawnPoint(spawner.transform, 3, new Vector3(1.8f, 0.05f, -1.05f), PartyArtMaterialType.PlayerYellow);

        inputRouter = new GameObject("Template Input Router").AddComponent<MinigameInputRouter>();
        ui = new GameObject("Template Minigame UI").AddComponent<MinigameUIController>();
        tapTest = new GameObject("Minigame_TemplateTapTest").AddComponent<TemplateTapTestMinigame>();
    }

    private void BuildArena()
    {
        AddCube("Template Grass Base", new Vector3(0f, -0.18f, 0f), new Vector3(9.2f, 0.22f, 6.0f), PartyArtMaterialType.Grass);
        AddCube("Warm Arena Floor", new Vector3(0f, 0f, 0f), new Vector3(6.6f, 0.16f, 3.9f), PartyArtMaterialType.Platform);
        AddCube("Blue Back Rail", new Vector3(0f, 0.42f, 1.95f), new Vector3(6.8f, 0.28f, 0.18f), PartyArtMaterialType.PlayerBlue);
        AddCube("Front Safe Edge", new Vector3(0f, 0.34f, -1.95f), new Vector3(6.8f, 0.18f, 0.18f), PartyArtMaterialType.WhiteAccent);
        AddCube("Left Rail", new Vector3(-3.4f, 0.36f, 0f), new Vector3(0.18f, 0.24f, 3.8f), PartyArtMaterialType.PlayerGreen);
        AddCube("Right Rail", new Vector3(3.4f, 0.36f, 0f), new Vector3(0.18f, 0.24f, 3.8f), PartyArtMaterialType.PlayerRed);
        AddCylinder("Center Tap Target", new Vector3(0f, 0.16f, 0.22f), new Vector3(0.95f, 0.08f, 0.95f), PartyArtMaterialType.WhiteAccent);
        AddCylinder("Center Color Ring", new Vector3(0f, 0.24f, 0.22f), new Vector3(1.08f, 0.035f, 1.08f), PartyArtMaterialType.PlayerYellow);
        AddCube("Editor Hint Plate", new Vector3(0f, 0.46f, 1.25f), new Vector3(3.8f, 0.08f, 0.34f), PartyArtMaterialType.UIPanel);
        AddLabel("Tap / Action Test", new Vector3(0f, 0.58f, 1.16f));
    }

    private void CreateSpawnPoint(Transform parent, int index, Vector3 position, PartyArtMaterialType materialType)
    {
        var pointObject = new GameObject($"P{index + 1} Spawn");
        pointObject.transform.SetParent(parent, false);
        pointObject.transform.localPosition = position;
        pointObject.AddComponent<PlayerSpawnPoint>().Configure(index);
        AddCylinder($"P{index + 1} Spawn Pad", position + new Vector3(0f, 0.03f, 0f), new Vector3(0.42f, 0.04f, 0.42f), materialType);
    }

    private void AddLabel(string text, Vector3 position)
    {
        GameObject labelObject = new GameObject(text);
        labelObject.transform.position = position;
        labelObject.transform.rotation = Quaternion.Euler(64f, 0f, 0f);
        TextMesh mesh = labelObject.AddComponent<TextMesh>();
        mesh.text = text;
        mesh.fontSize = 42;
        mesh.characterSize = 0.085f;
        mesh.anchor = TextAnchor.MiddleCenter;
        mesh.alignment = TextAlignment.Center;
        mesh.color = PartyArtPalette.DarkAccent;
    }

    private GameObject AddCube(string name, Vector3 position, Vector3 scale, PartyArtMaterialType materialType)
    {
        return AddPrimitive(name, PrimitiveType.Cube, position, scale, PartyArtMaterials.Get(materialType));
    }

    private GameObject AddCylinder(string name, Vector3 position, Vector3 scale, PartyArtMaterialType materialType)
    {
        return AddPrimitive(name, PrimitiveType.Cylinder, position, scale, PartyArtMaterials.Get(materialType));
    }

    private GameObject AddPrimitive(string name, PrimitiveType primitiveType, Vector3 position, Vector3 scale, Material material)
    {
        GameObject instance = GameObject.CreatePrimitive(primitiveType);
        instance.name = name;
        instance.transform.position = position;
        instance.transform.localScale = scale;
        instance.GetComponent<Renderer>().material = material;
        Collider collider = instance.GetComponent<Collider>();
        if (collider != null)
        {
            Destroy(collider);
        }

        return instance;
    }
}
