using System.Collections.Generic;
using UnityEngine;

public class TowerTapClimbSceneBootstrapper : MonoBehaviour
{
    private readonly List<PlayerTowerMeter> meters = new List<PlayerTowerMeter>();
    private MinigameManager manager;
    private MinigamePlayerSpawner spawner;
    private MinigameUIController ui;
    private MinigameInputRouter inputRouter;
    private TowerTapClimbMinigame minigame;

    private void Awake()
    {
        SetupLighting();
        SetupCamera();
        BuildStage();
        BuildRuntimeObjects();
    }

    private void Start()
    {
        StartTowerTapClimb();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            StartTowerTapClimb();
        }
    }

    private void StartTowerTapClimb()
    {
        manager.StartTemplateMinigame(minigame, spawner, ui, inputRouter, BuildTowerMeters, result =>
        {
            Debug.Log($"Tower Tap Climb completed: {result.Title}");
        });
    }

    private void BuildTowerMeters(IReadOnlyList<PlayerMinigameData> players)
    {
        ClearMeters();
        meters.Clear();
        Vector3[] positions =
        {
            new Vector3(-2.55f, 0.18f, 0.36f),
            new Vector3(-0.85f, 0.18f, 0.36f),
            new Vector3(0.85f, 0.18f, 0.36f),
            new Vector3(2.55f, 0.18f, 0.36f)
        };

        for (int i = 0; i < players.Count; i++)
        {
            GameObject meterObject = new GameObject($"{players[i].PlayerName} Tower Meter");
            meterObject.transform.position = positions[i];
            PlayerTowerMeter meter = meterObject.AddComponent<PlayerTowerMeter>();
            meter.Build(players[i], 3.45f);
            meters.Add(meter);
        }

        minigame.RegisterMeters(meters);
    }

    private void ClearMeters()
    {
        for (int i = meters.Count - 1; i >= 0; i--)
        {
            if (meters[i] != null)
            {
                Destroy(meters[i].gameObject);
            }
        }
    }

    private void SetupLighting()
    {
        var lightingObject = new GameObject("Tower Tap Lighting");
        lightingObject.AddComponent<LightingPresetController>();
    }

    private void SetupCamera()
    {
        var cameraObject = new GameObject("Tower Tap Camera");
        cameraObject.tag = "MainCamera";
        Camera camera = cameraObject.AddComponent<Camera>();
        camera.backgroundColor = PartyArtPalette.Sky;
        camera.fieldOfView = 40f;
        camera.nearClipPlane = 0.1f;
        camera.farClipPlane = 90f;
        cameraObject.transform.position = new Vector3(0f, 5.9f, -8.6f);
        cameraObject.transform.rotation = Quaternion.LookRotation(new Vector3(0f, 1.55f, 0.18f) - cameraObject.transform.position, Vector3.up);
    }

    private void BuildRuntimeObjects()
    {
        manager = new GameObject("Tower Tap Minigame Manager").AddComponent<MinigameManager>();
        spawner = new GameObject("Tower Tap Player Spawner").AddComponent<MinigamePlayerSpawner>();
        CreateSpawnPoint(spawner.transform, 0, new Vector3(-2.55f, 0.14f, -1.35f), PartyArtMaterialType.PlayerBlue);
        CreateSpawnPoint(spawner.transform, 1, new Vector3(-0.85f, 0.14f, -1.35f), PartyArtMaterialType.PlayerRed);
        CreateSpawnPoint(spawner.transform, 2, new Vector3(0.85f, 0.14f, -1.35f), PartyArtMaterialType.PlayerGreen);
        CreateSpawnPoint(spawner.transform, 3, new Vector3(2.55f, 0.14f, -1.35f), PartyArtMaterialType.PlayerYellow);

        inputRouter = new GameObject("Tower Tap Input Router").AddComponent<MinigameInputRouter>();
        ui = new GameObject("Tower Tap UI").AddComponent<MinigameUIController>();
        minigame = new GameObject("TowerTapClimbMinigame").AddComponent<TowerTapClimbMinigame>();
    }

    private void BuildStage()
    {
        AddCube("Tower Tap Grass Base", new Vector3(0f, -0.20f, 0f), new Vector3(9.2f, 0.22f, 6.2f), PartyArtMaterialType.Grass);
        AddCube("Tower Tap Arena Floor", new Vector3(0f, 0f, 0f), new Vector3(7.2f, 0.16f, 4.15f), PartyArtMaterialType.Platform);
        AddCube("Back Tower Rail", new Vector3(0f, 0.44f, 1.96f), new Vector3(7.25f, 0.24f, 0.18f), PartyArtMaterialType.PlayerGreen);
        AddCube("Front Player Rail", new Vector3(0f, 0.38f, -1.96f), new Vector3(7.25f, 0.18f, 0.18f), PartyArtMaterialType.PlayerYellow);
        AddCube("Left Side Rail", new Vector3(-3.68f, 0.34f, 0f), new Vector3(0.18f, 0.22f, 4.10f), PartyArtMaterialType.PlayerBlue);
        AddCube("Right Side Rail", new Vector3(3.68f, 0.34f, 0f), new Vector3(0.18f, 0.22f, 4.10f), PartyArtMaterialType.PlayerRed);
        AddCylinder("Center Height Badge", new Vector3(0f, 0.20f, -0.08f), new Vector3(0.88f, 0.06f, 0.88f), PartyArtMaterialType.WhiteAccent);
        AddCylinder("Center Tap Ring", new Vector3(0f, 0.27f, -0.08f), new Vector3(1.02f, 0.035f, 1.02f), PartyArtMaterialType.PlayerYellow);
        AddLabel("Tower Tap Climb", new Vector3(0f, 0.68f, 1.88f));
        AddCloud(new Vector3(-3.28f, 0.56f, 2.20f));
        AddCloud(new Vector3(3.22f, 0.66f, 2.15f));
    }

    private void CreateSpawnPoint(Transform parent, int index, Vector3 position, PartyArtMaterialType materialType)
    {
        var pointObject = new GameObject($"P{index + 1} Spawn");
        pointObject.transform.SetParent(parent, false);
        pointObject.transform.localPosition = position;
        pointObject.AddComponent<PlayerSpawnPoint>().Configure(index);
        AddCylinder($"P{index + 1} Spawn Pad", position + new Vector3(0f, 0.05f, 0f), new Vector3(0.46f, 0.04f, 0.46f), materialType);
    }

    private void AddCloud(Vector3 position)
    {
        AddSphere("Cloud Puff A", position, new Vector3(0.42f, 0.22f, 0.22f), PartyArtPalette.WhiteAccent);
        AddSphere("Cloud Puff B", position + new Vector3(0.30f, 0.04f, 0f), new Vector3(0.30f, 0.17f, 0.17f), PartyArtPalette.WhiteAccent);
        AddSphere("Cloud Puff C", position + new Vector3(-0.30f, 0.03f, 0f), new Vector3(0.28f, 0.16f, 0.16f), PartyArtPalette.WhiteAccent);
    }

    private void AddLabel(string text, Vector3 position)
    {
        GameObject labelObject = new GameObject(text);
        labelObject.transform.position = position;
        labelObject.transform.rotation = Quaternion.Euler(64f, 0f, 0f);
        TextMesh mesh = labelObject.AddComponent<TextMesh>();
        mesh.text = text;
        mesh.fontSize = 48;
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

    private GameObject AddSphere(string name, Vector3 position, Vector3 scale, Color color)
    {
        return AddPrimitive(name, PrimitiveType.Sphere, position, scale, PartyArtMaterials.CreateTinted(name, color));
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
