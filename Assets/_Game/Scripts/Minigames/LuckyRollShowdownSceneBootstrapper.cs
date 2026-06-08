using System.Collections.Generic;
using UnityEngine;

public class LuckyRollShowdownSceneBootstrapper : MonoBehaviour
{
    private readonly List<LuckyRollPlayerPanel> panels = new List<LuckyRollPlayerPanel>();
    private MinigameManager manager;
    private MinigamePlayerSpawner spawner;
    private MinigameUIController ui;
    private MinigameInputRouter inputRouter;
    private LuckyRollShowdownMinigame minigame;

    private void Awake()
    {
        SetupLighting();
        SetupCamera();
        BuildStage();
        BuildRuntimeObjects();
    }

    private void Start()
    {
        StartLuckyRoll();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            StartLuckyRoll();
        }
    }

    private void StartLuckyRoll()
    {
        manager.StartTemplateMinigame(minigame, spawner, ui, inputRouter, BuildPlayerPanels, result =>
        {
            Debug.Log($"Lucky Roll completed: {result.Title}");
        });
    }

    private void BuildPlayerPanels(IReadOnlyList<PlayerMinigameData> players)
    {
        ClearPanels();
        panels.Clear();
        Vector3[] positions =
        {
            new Vector3(-2.45f, 0.08f, 0.70f),
            new Vector3(-0.82f, 0.08f, 0.70f),
            new Vector3(0.82f, 0.08f, 0.70f),
            new Vector3(2.45f, 0.08f, 0.70f)
        };

        for (int i = 0; i < players.Count; i++)
        {
            GameObject panelObject = new GameObject($"{players[i].PlayerName} Lucky Roll Panel");
            panelObject.transform.position = positions[i];
            LuckyRollPlayerPanel panel = panelObject.AddComponent<LuckyRollPlayerPanel>();
            panel.Build(players[i]);
            panels.Add(panel);
        }

        minigame.RegisterPanels(panels);
    }

    private void ClearPanels()
    {
        for (int i = panels.Count - 1; i >= 0; i--)
        {
            if (panels[i] != null)
            {
                Destroy(panels[i].gameObject);
            }
        }
    }

    private void SetupLighting()
    {
        var lightingObject = new GameObject("Lucky Roll Lighting");
        lightingObject.AddComponent<LightingPresetController>();
    }

    private void SetupCamera()
    {
        var cameraObject = new GameObject("Lucky Roll Camera");
        cameraObject.tag = "MainCamera";
        Camera camera = cameraObject.AddComponent<Camera>();
        camera.backgroundColor = PartyArtPalette.Sky;
        camera.fieldOfView = 42f;
        camera.nearClipPlane = 0.1f;
        camera.farClipPlane = 90f;
        cameraObject.transform.position = new Vector3(0f, 5.4f, -7.7f);
        cameraObject.transform.rotation = Quaternion.LookRotation(new Vector3(0f, 0.85f, 0.15f) - cameraObject.transform.position, Vector3.up);
    }

    private void BuildRuntimeObjects()
    {
        manager = new GameObject("Lucky Roll Minigame Manager").AddComponent<MinigameManager>();
        spawner = new GameObject("Lucky Roll Player Spawner").AddComponent<MinigamePlayerSpawner>();
        CreateSpawnPoint(spawner.transform, 0, new Vector3(-2.45f, 0.12f, -1.10f), PartyArtMaterialType.PlayerBlue);
        CreateSpawnPoint(spawner.transform, 1, new Vector3(-0.82f, 0.12f, -1.10f), PartyArtMaterialType.PlayerRed);
        CreateSpawnPoint(spawner.transform, 2, new Vector3(0.82f, 0.12f, -1.10f), PartyArtMaterialType.PlayerGreen);
        CreateSpawnPoint(spawner.transform, 3, new Vector3(2.45f, 0.12f, -1.10f), PartyArtMaterialType.PlayerYellow);

        inputRouter = new GameObject("Lucky Roll Input Router").AddComponent<MinigameInputRouter>();
        ui = new GameObject("Lucky Roll UI").AddComponent<MinigameUIController>();
        minigame = new GameObject("LuckyRollShowdownMinigame").AddComponent<LuckyRollShowdownMinigame>();
    }

    private void BuildStage()
    {
        AddCube("Sky Arena Base", new Vector3(0f, -0.20f, 0f), new Vector3(8.8f, 0.24f, 5.8f), PartyArtMaterialType.Sky);
        AddCylinder("Floating Party Platform", new Vector3(0f, 0f, 0f), new Vector3(3.75f, 0.28f, 2.15f), PartyArtMaterialType.Platform);
        AddCylinder("Soft White Inner Platform", new Vector3(0f, 0.21f, 0f), new Vector3(3.35f, 0.07f, 1.88f), PartyArtMaterialType.WhiteAccent);
        AddCube("Back Number Rail", new Vector3(0f, 0.44f, 1.72f), new Vector3(6.9f, 0.22f, 0.18f), PartyArtMaterialType.PlayerBlue);
        AddCube("Front Player Rail", new Vector3(0f, 0.39f, -1.72f), new Vector3(6.9f, 0.18f, 0.18f), PartyArtMaterialType.PlayerYellow);
        AddCube("Left Confetti Block", new Vector3(-3.82f, 0.30f, 0.25f), new Vector3(0.22f, 0.42f, 2.8f), PartyArtMaterialType.PlayerGreen);
        AddCube("Right Confetti Block", new Vector3(3.82f, 0.30f, 0.25f), new Vector3(0.22f, 0.42f, 2.8f), PartyArtMaterialType.PlayerRed);
        AddCloud(new Vector3(-3.2f, 0.55f, 2.15f));
        AddCloud(new Vector3(3.1f, 0.65f, 2.05f));
        AddLabel("Lucky Roll Showdown", new Vector3(0f, 0.68f, 1.70f));
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
