using System.Collections.Generic;
using UnityEngine;

public static class RuntimeMinigameArena
{
    private const string RootName = "Runtime Minigame Arena";
    private static GameObject root;
    private static CameraFollowController cameraFollow;

    public static void Show(MinigameArenaStyle style, IReadOnlyList<PlayerController> players)
    {
        Clear(false);
        root = new GameObject(RootName);
        ConfigureCamera();
        CreateLightRig();

        switch (style)
        {
            case MinigameArenaStyle.VineClimb:
                BuildVineClimb(players);
                break;
            case MinigameArenaStyle.BridgeDuel:
                BuildBridgeDuel(players);
                break;
            case MinigameArenaStyle.PlankRaft:
                BuildPlankRaft(players);
                break;
            case MinigameArenaStyle.NeonSplit:
                BuildNeonSplit(players);
                break;
            case MinigameArenaStyle.StageKitchenFarm:
                BuildStageKitchenFarm(players);
                break;
            case MinigameArenaStyle.SkiRace:
                BuildSkiRace(players);
                break;
            case MinigameArenaStyle.ScooterLine:
                BuildScooterLine(players);
                break;
            case MinigameArenaStyle.ParasolWall:
                BuildParasolWall(players);
                break;
            case MinigameArenaStyle.CastleStage:
                BuildCastleStage(players);
                break;
            case MinigameArenaStyle.PhotoPanels:
                BuildPhotoPanels(players);
                break;
            case MinigameArenaStyle.StackLights:
                BuildStackLights(players);
                break;
            case MinigameArenaStyle.SheepTrail:
                BuildSheepTrail(players);
                break;
            case MinigameArenaStyle.MinecartTrack:
                BuildMinecartTrack(players);
                break;
            case MinigameArenaStyle.KitchenCounters:
                BuildKitchenCounters(players);
                break;
            default:
                BuildLavaCircle(players);
                break;
        }
    }

    public static void Clear()
    {
        Clear(true);
    }

    private static void Clear(bool restoreCamera)
    {
        GameObject existing = GameObject.Find(RootName);
        if (existing != null)
        {
            Object.Destroy(existing);
        }

        root = null;
        MinigamePlayerSpawnSystem.Clear();
        if (restoreCamera && cameraFollow != null)
        {
            cameraFollow.enabled = true;
        }
    }

    private static void ConfigureCamera()
    {
        Camera camera = Camera.main;
        if (camera == null)
        {
            return;
        }

        cameraFollow = camera.GetComponent<CameraFollowController>();
        if (cameraFollow != null)
        {
            cameraFollow.enabled = false;
        }

        camera.orthographic = false;
        camera.fieldOfView = 38f;
        camera.backgroundColor = new Color(0.10f, 0.16f, 0.22f);
        camera.transform.position = new Vector3(0f, 6.4f, -8.8f);
        camera.transform.rotation = Quaternion.LookRotation(new Vector3(0f, 0.75f, 0f) - camera.transform.position, Vector3.up);
    }

    private static void CreateLightRig()
    {
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(0.72f, 0.76f, 0.80f);

        var key = new GameObject("Arena Key Light");
        key.transform.SetParent(root.transform);
        var keyLight = key.AddComponent<Light>();
        keyLight.type = LightType.Directional;
        keyLight.shadows = LightShadows.None;
        keyLight.intensity = 1.05f;
        keyLight.color = new Color(1f, 0.92f, 0.78f);
        key.transform.rotation = Quaternion.Euler(46f, -28f, 0f);
    }

    private static void BuildLavaCircle(IReadOnlyList<PlayerController> players)
    {
        AddCube("Lava Sea", new Vector3(0f, -0.42f, 0f), new Vector3(12f, 0.08f, 12f), new Color(1f, 0.28f, 0.05f));
        for (int i = 0; i < 18; i++)
        {
            AddCube("Lava Glow Patch", new Vector3(Random.Range(-5.2f, 5.2f), -0.36f, Random.Range(-5.2f, 5.2f)), new Vector3(Random.Range(0.6f, 1.8f), 0.03f, Random.Range(0.25f, 0.7f)), new Color(1f, Random.Range(0.50f, 0.82f), 0.06f), Random.Range(0f, 180f));
        }

        AddCylinder("Stone Arena Drum", new Vector3(0f, -0.10f, 0f), new Vector3(2.35f, 0.62f, 2.35f), new Color(0.45f, 0.34f, 0.28f));
        AddCylinder("Carved Arena Top", new Vector3(0f, 0.56f, 0f), new Vector3(2.26f, 0.10f, 2.26f), new Color(0.56f, 0.54f, 0.52f));
        AddCylinder("Hot Rim", new Vector3(0f, 0.69f, 0f), new Vector3(2.37f, 0.04f, 2.37f), new Color(0.94f, 0.40f, 0.16f));
        AddRadialStoneLines(2.14f, 16);
        AddRockCluster(-4.3f, -2.6f);
        AddRockCluster(4.4f, 2.7f);
        AddPlayerFigures(players, 1.25f, 0.90f);
    }

    private static void BuildVineClimb(IReadOnlyList<PlayerController> players)
    {
        AddCube("Sky Back Wall", new Vector3(0f, 1.8f, 1.3f), new Vector3(8.8f, 5.2f, 0.12f), new Color(0.30f, 0.62f, 0.96f));
        AddCube("Grass Ground", new Vector3(0f, -0.20f, 0f), new Vector3(8.8f, 0.18f, 3.8f), new Color(0.30f, 0.68f, 0.34f));
        for (int lane = 0; lane < 4; lane++)
        {
            float x = -3f + lane * 2f;
            AddCube("Lane Panel", new Vector3(x, 1.15f, 0.92f), new Vector3(1.24f, 3.8f, 0.10f), new Color(0.52f, 0.74f, 0.88f));
            AddVineColumn(x, 0.94f);
            AddCloud(new Vector3(x + 0.46f, 3.35f - lane * 0.32f, 0.72f));
        }

        AddPlayerFigures(players, 2.55f, 0.24f);
    }

    private static void BuildBridgeDuel(IReadOnlyList<PlayerController> players)
    {
        AddCube("Lava Field", new Vector3(0f, -0.46f, 0f), new Vector3(12f, 0.08f, 9.4f), new Color(1f, 0.24f, 0.04f));
        AddCylinder("Red Hex Platform", new Vector3(-2.55f, 0.22f, 0f), new Vector3(1.62f, 0.34f, 1.62f), new Color(0.82f, 0.34f, 0.25f));
        AddCylinder("Blue Hex Platform", new Vector3(2.55f, 0.22f, 0f), new Vector3(1.62f, 0.34f, 1.62f), new Color(0.25f, 0.50f, 0.88f));
        AddCube("Narrow Stone Bridge", new Vector3(0f, 0.56f, 0f), new Vector3(3.85f, 0.22f, 0.50f), new Color(0.38f, 0.35f, 0.32f), -8f);
        AddCylinder("Center Pillar", new Vector3(0f, 0.10f, 0.10f), new Vector3(0.48f, 0.86f, 0.48f), new Color(0.36f, 0.31f, 0.27f));
        AddRockCluster(-4.8f, 2.7f);
        AddRockCluster(4.7f, -2.8f);
        AddPlayerFigures(players, 1.36f, 0.75f);
    }

    private static void BuildPlankRaft(IReadOnlyList<PlayerController> players)
    {
        AddCube("River Plane", new Vector3(0f, -0.30f, 0f), new Vector3(10.5f, 0.06f, 8.0f), new Color(0.10f, 0.45f, 0.58f));
        for (int i = 0; i < 9; i++)
        {
            float x = -3.2f + i * 0.8f;
            AddCube("Raft Plank", new Vector3(x, 0.10f, 0f), new Vector3(0.72f, 0.14f, 4.2f), new Color(0.70f, 0.55f, 0.34f), Random.Range(-2f, 2f));
        }

        AddCylinder("Rolling Log", new Vector3(-2.7f, 0.58f, 0.15f), new Vector3(0.42f, 1.65f, 0.42f), new Color(0.48f, 0.25f, 0.13f), 90f);
        AddCube("Log Strap A", new Vector3(-2.7f, 0.59f, -0.70f), new Vector3(0.92f, 0.08f, 0.12f), new Color(0.20f, 0.18f, 0.16f));
        AddCube("Log Strap B", new Vector3(-2.7f, 0.59f, 1.00f), new Vector3(0.92f, 0.08f, 0.12f), new Color(0.20f, 0.18f, 0.16f));
        AddPlayerFigures(players, 1.05f, 0.34f);
    }

    private static void BuildNeonSplit(IReadOnlyList<PlayerController> players)
    {
        AddCube("Cloud Floor", new Vector3(0f, -0.34f, 0f), new Vector3(9.8f, 0.06f, 7.4f), new Color(0.64f, 0.85f, 0.96f));
        AddCube("Red Team Pad", new Vector3(-1.65f, 0.02f, 0f), new Vector3(3.2f, 0.18f, 4.4f), new Color(0.90f, 0.22f, 0.38f));
        AddCube("Blue Team Pad", new Vector3(1.65f, 0.02f, 0f), new Vector3(3.2f, 0.18f, 4.4f), new Color(0.22f, 0.50f, 0.92f));
        AddCube("Center Split", new Vector3(0f, 0.16f, 0f), new Vector3(0.18f, 0.14f, 4.55f), new Color(0.98f, 0.86f, 0.24f));
        AddCube("Hazard Border North", new Vector3(0f, 0.16f, 2.28f), new Vector3(6.85f, 0.16f, 0.22f), new Color(0.10f, 0.10f, 0.11f));
        AddCube("Hazard Border South", new Vector3(0f, 0.16f, -2.28f), new Vector3(6.85f, 0.16f, 0.22f), new Color(0.10f, 0.10f, 0.11f));
        AddPlayerFigures(players, 1.20f, 0.32f);
    }

    private static void BuildStageKitchenFarm(IReadOnlyList<PlayerController> players)
    {
        AddCube("Stage Backdrop", new Vector3(0f, 2.0f, 1.35f), new Vector3(8.8f, 3.8f, 0.20f), new Color(0.24f, 0.18f, 0.20f));
        AddCube("Kitchen Counter", new Vector3(0f, 0.42f, -0.22f), new Vector3(6.7f, 0.42f, 1.32f), new Color(0.76f, 0.62f, 0.42f));
        for (int i = 0; i < 4; i++)
        {
            AddCube("Cooking Station", new Vector3(-2.55f + i * 1.7f, 0.82f, -0.24f), new Vector3(1.20f, 0.10f, 1.05f), new Color(0.88f, 0.86f, 0.78f));
            AddCylinder("Ingredient Bowl", new Vector3(-2.55f + i * 1.7f, 1.00f, -0.34f), new Vector3(0.24f, 0.08f, 0.24f), PickPlayerColor(i));
        }

        AddCube("Farm Lane", new Vector3(0f, -0.08f, -2.28f), new Vector3(7.8f, 0.12f, 1.25f), new Color(0.70f, 0.55f, 0.36f));
        for (int i = 0; i < 7; i++)
        {
            AddSheep(new Vector3(-3.0f + i * 1.0f, 0.18f, -2.35f + (i % 2) * 0.38f));
        }

        AddCube("Mine Track", new Vector3(0f, 0.02f, -3.30f), new Vector3(7.8f, 0.07f, 0.10f), new Color(0.22f, 0.20f, 0.18f));
        AddCube("Mine Cart", new Vector3(-2.6f, 0.32f, -3.30f), new Vector3(0.84f, 0.46f, 0.58f), new Color(0.37f, 0.42f, 0.45f));
        AddPlayerFigures(players, 1.15f, 1.18f);
    }

    private static void BuildSkiRace(IReadOnlyList<PlayerController> players)
    {
        Camera camera = Camera.main;
        if (camera != null)
        {
            camera.backgroundColor = new Color(0.72f, 0.84f, 0.92f);
            camera.transform.position = new Vector3(0f, 4.8f, -8.6f);
            camera.transform.rotation = Quaternion.LookRotation(new Vector3(0f, 0.55f, 0.35f) - camera.transform.position, Vector3.up);
        }

        AddCube("Snow Bridge Deck", new Vector3(0f, -0.16f, 0.85f), new Vector3(8.8f, 0.18f, 7.2f), new Color(0.78f, 0.82f, 0.84f));
        AddCube("Far City Fog", new Vector3(0f, 1.8f, 3.2f), new Vector3(9.8f, 2.5f, 0.18f), new Color(0.64f, 0.72f, 0.78f));
        AddCube("Left Guard Rail", new Vector3(-4.2f, 0.35f, 0.85f), new Vector3(0.18f, 0.45f, 6.8f), new Color(0.52f, 0.58f, 0.62f));
        AddCube("Right Guard Rail", new Vector3(4.2f, 0.35f, 0.85f), new Vector3(0.18f, 0.45f, 6.8f), new Color(0.52f, 0.58f, 0.62f));

        Color[] laneColors =
        {
            new Color(0.20f, 0.66f, 0.96f),
            new Color(0.96f, 0.24f, 0.26f),
            new Color(0.25f, 0.78f, 0.28f),
            new Color(1f, 0.76f, 0.18f)
        };

        for (int i = 0; i < 4; i++)
        {
            float x = -3.0f + i * 2.0f;
            AddCube("Ski Lane Stripe", new Vector3(x, -0.02f, 0.40f), new Vector3(0.92f, 0.035f, 5.7f), laneColors[i]);
            AddCube("Distance Plate", new Vector3(x, 0.25f, -2.85f), new Vector3(1.16f, 0.42f, 0.12f), new Color(0.06f, 0.12f, 0.16f));
            AddCube("Ski Pair Left", new Vector3(x - 0.13f, 0.22f, -0.22f), new Vector3(0.06f, 0.035f, 1.15f), laneColors[i]);
            AddCube("Ski Pair Right", new Vector3(x + 0.13f, 0.22f, -0.22f), new Vector3(0.06f, 0.035f, 1.15f), laneColors[i]);
        }

        AddPlayerFiguresInLine(players, 0.64f, -0.30f, 0.36f);
    }

    private static void BuildScooterLine(IReadOnlyList<PlayerController> players)
    {
        Camera camera = Camera.main;
        if (camera != null)
        {
            camera.backgroundColor = new Color(0.55f, 0.78f, 0.96f);
            camera.transform.position = new Vector3(0f, 3.85f, -7.3f);
            camera.transform.rotation = Quaternion.LookRotation(new Vector3(0f, 1.05f, 0.15f) - camera.transform.position, Vector3.up);
        }

        AddCube("Town Plaza Ground", new Vector3(0f, -0.20f, 0f), new Vector3(9.6f, 0.16f, 5.7f), new Color(0.74f, 0.60f, 0.43f));
        AddCube("Shop Wall", new Vector3(0f, 1.36f, 1.72f), new Vector3(9.6f, 2.85f, 0.22f), new Color(0.77f, 0.45f, 0.32f));
        AddCube("Awning Beam", new Vector3(0f, 2.22f, 1.48f), new Vector3(9.7f, 0.30f, 0.42f), new Color(0.18f, 0.30f, 0.42f));

        Color[] shopColors =
        {
            new Color(0.18f, 0.64f, 0.93f),
            new Color(0.95f, 0.72f, 0.20f),
            new Color(0.84f, 0.22f, 0.78f),
            new Color(0.94f, 0.25f, 0.22f)
        };

        for (int i = 0; i < 4; i++)
        {
            float x = -3.15f + i * 2.1f;
            AddCube("Scooter Shop Front", new Vector3(x, 1.14f, 1.55f), new Vector3(1.74f, 1.42f, 0.18f), shopColors[i]);
            AddCylinder("Round Shop Sign", new Vector3(x, 2.10f, 1.38f), new Vector3(0.44f, 0.06f, 0.44f), new Color(1f, 0.93f, 0.42f), 90f);
            AddCube("Scooter Start Stripe", new Vector3(x, -0.06f, -1.64f), new Vector3(1.20f, 0.04f, 0.12f), shopColors[i]);
            AddScooter(new Vector3(x, 0.18f, -0.92f), shopColors[i]);
        }

        AddPlayerFiguresInLine(players, 0.92f, -0.94f, 0.74f);
    }

    private static void BuildParasolWall(IReadOnlyList<PlayerController> players)
    {
        AddCube("Aqua Tile Wall", new Vector3(0f, 1.55f, 1.10f), new Vector3(8.4f, 4.0f, 0.18f), new Color(0.22f, 0.72f, 0.84f));
        AddCube("Drop Ledge", new Vector3(0f, -0.18f, -0.10f), new Vector3(8.2f, 0.16f, 3.2f), new Color(0.14f, 0.56f, 0.66f));
        for (int i = 0; i < 6; i++)
        {
            AddVineColumn(-4.0f + i * 1.6f, 0.92f);
        }

        Color[] parasols =
        {
            new Color(0.95f, 0.24f, 0.24f),
            new Color(0.25f, 0.55f, 0.95f),
            new Color(0.28f, 0.80f, 0.34f),
            new Color(1f, 0.76f, 0.20f)
        };

        for (int i = 0; i < 4; i++)
        {
            float x = -3.0f + i * 2.0f;
            AddCylinder("Parasol Canopy", new Vector3(x, 1.15f, -0.46f), new Vector3(0.48f, 0.08f, 0.48f), parasols[i]);
            AddCube("Parasol Handle", new Vector3(x, 0.74f, -0.46f), new Vector3(0.05f, 0.54f, 0.05f), new Color(0.92f, 0.92f, 0.86f));
        }

        AddPlayerFiguresInLine(players, 1.05f, -0.52f, 0.26f);
    }

    private static void BuildCastleStage(IReadOnlyList<PlayerController> players)
    {
        AddCube("Castle Chamber Backdrop", new Vector3(0f, 1.55f, 1.20f), new Vector3(8.8f, 3.8f, 0.24f), new Color(0.25f, 0.18f, 0.20f));
        AddCube("Stage Floor", new Vector3(0f, -0.16f, -0.30f), new Vector3(7.2f, 0.20f, 3.7f), new Color(0.68f, 0.56f, 0.42f));
        AddCube("Front Stairs", new Vector3(0f, 0.05f, -2.02f), new Vector3(5.6f, 0.22f, 0.52f), new Color(0.54f, 0.48f, 0.43f));
        AddCube("Monster Face", new Vector3(0f, 2.00f, 1.02f), new Vector3(2.3f, 1.4f, 0.22f), new Color(0.76f, 0.48f, 0.22f));
        AddCylinder("Monster Left Horn", new Vector3(-0.92f, 2.78f, 0.90f), new Vector3(0.15f, 0.42f, 0.15f), new Color(0.94f, 0.86f, 0.62f), 25f);
        AddCylinder("Monster Right Horn", new Vector3(0.92f, 2.78f, 0.90f), new Vector3(0.15f, 0.42f, 0.15f), new Color(0.94f, 0.86f, 0.62f), -25f);
        AddPlayerFiguresInLine(players, 1.10f, -0.58f, 0.34f);
    }

    private static void BuildPhotoPanels(IReadOnlyList<PlayerController> players)
    {
        AddCube("Hill Field", new Vector3(0f, -0.28f, 0f), new Vector3(9.4f, 0.12f, 5.8f), new Color(0.44f, 0.72f, 0.34f));
        AddCube("Blue Sky", new Vector3(0f, 1.80f, 1.60f), new Vector3(9.4f, 3.4f, 0.16f), new Color(0.42f, 0.72f, 0.96f));
        for (int i = 0; i < 4; i++)
        {
            float x = i % 2 == 0 ? -2.1f : 2.1f;
            float y = i < 2 ? 1.55f : 0.35f;
            AddCube("Photo Frame", new Vector3(x, y, -0.35f), new Vector3(3.0f, 1.05f, 0.14f), new Color(0.95f, 0.95f, 0.86f));
            AddCube("Photo Sky Print", new Vector3(x, y + 0.08f, -0.44f), new Vector3(2.64f, 0.70f, 0.10f), new Color(0.52f, 0.78f, 0.96f));
            AddSphere("Photo Face", new Vector3(x, y - 0.16f, -0.55f), new Vector3(0.22f, 0.22f, 0.06f), PickPlayerColor(i));
        }

        AddPlayerFigures(players, 0.72f, 0.14f);
    }

    private static void BuildStackLights(IReadOnlyList<PlayerController> players)
    {
        AddCube("Night Plaza", new Vector3(0f, -0.20f, 0f), new Vector3(9.4f, 0.12f, 5.6f), new Color(0.13f, 0.13f, 0.17f));
        AddCube("Mansion Glow", new Vector3(0f, 1.25f, 1.75f), new Vector3(8.6f, 2.1f, 0.18f), new Color(0.22f, 0.24f, 0.28f));
        Color[] stackColors = { new Color(0.20f, 0.62f, 1f), new Color(1f, 0.78f, 0.20f), new Color(0.30f, 0.88f, 0.44f), new Color(0.95f, 0.28f, 0.70f) };
        for (int i = 0; i < 4; i++)
        {
            float x = -3.0f + i * 2.0f;
            for (int j = 0; j < 6; j++)
            {
                AddCube("Glow Stack Block", new Vector3(x, 0.05f + j * 0.26f, -0.60f), new Vector3(0.42f, 0.20f, 0.42f), stackColors[i]);
            }
        }

        AddPlayerFiguresInLine(players, 0.94f, -1.70f, 0.18f);
    }

    private static void BuildSheepTrail(IReadOnlyList<PlayerController> players)
    {
        AddCube("Pasture", new Vector3(0f, -0.28f, 0f), new Vector3(9.8f, 0.12f, 6.2f), new Color(0.42f, 0.74f, 0.34f));
        AddCube("Dirt Lane", new Vector3(0f, -0.16f, -0.10f), new Vector3(3.5f, 0.08f, 5.8f), new Color(0.70f, 0.56f, 0.38f));
        AddCube("Left Stone Wall", new Vector3(-2.05f, 0.22f, -0.10f), new Vector3(0.30f, 0.45f, 5.9f), new Color(0.70f, 0.69f, 0.62f));
        AddCube("Right Stone Wall", new Vector3(2.05f, 0.22f, -0.10f), new Vector3(0.30f, 0.45f, 5.9f), new Color(0.70f, 0.69f, 0.62f));
        for (int i = 0; i < 15; i++)
        {
            AddSheep(new Vector3(Random.Range(-1.25f, 1.25f), 0.04f, -2.4f + i * 0.34f));
        }

        AddPlayerFiguresInLine(players, 0.44f, -2.35f, 0.20f);
    }

    private static void BuildMinecartTrack(IReadOnlyList<PlayerController> players)
    {
        AddCube("Orchard Ground", new Vector3(0f, -0.25f, 0f), new Vector3(9.6f, 0.12f, 6.0f), new Color(0.52f, 0.74f, 0.36f));
        AddCube("Rail Bed", new Vector3(0f, -0.10f, -0.20f), new Vector3(7.8f, 0.08f, 0.92f), new Color(0.55f, 0.42f, 0.28f));
        AddCube("Rail A", new Vector3(0f, 0.00f, -0.48f), new Vector3(7.8f, 0.08f, 0.08f), new Color(0.20f, 0.20f, 0.18f));
        AddCube("Rail B", new Vector3(0f, 0.00f, 0.08f), new Vector3(7.8f, 0.08f, 0.08f), new Color(0.20f, 0.20f, 0.18f));
        AddCube("Mine Cart Wagon", new Vector3(-2.8f, 0.36f, -0.20f), new Vector3(1.05f, 0.55f, 0.74f), new Color(0.45f, 0.50f, 0.50f));
        for (int i = 0; i < 8; i++)
        {
            AddCube("Orchard Tree Trunk", new Vector3(-4.0f + i * 1.14f, 0.28f, 1.85f), new Vector3(0.13f, 0.56f, 0.13f), new Color(0.43f, 0.28f, 0.15f));
            AddSphere("Orchard Tree Top", new Vector3(-4.0f + i * 1.14f, 0.88f, 1.85f), new Vector3(0.42f, 0.36f, 0.42f), new Color(0.20f, 0.56f, 0.24f));
        }

        AddPlayerFiguresInLine(players, 0.50f, -1.55f, 0.20f);
    }

    private static void BuildKitchenCounters(IReadOnlyList<PlayerController> players)
    {
        AddCube("Kitchen Wall", new Vector3(0f, 1.70f, 1.30f), new Vector3(9.6f, 3.2f, 0.18f), new Color(0.78f, 0.72f, 0.62f));
        AddCube("Cabinet Row", new Vector3(0f, 2.55f, 1.12f), new Vector3(8.8f, 0.70f, 0.22f), new Color(0.74f, 0.48f, 0.26f));
        AddCube("Counter Base", new Vector3(0f, 0.18f, -0.30f), new Vector3(8.8f, 0.52f, 2.0f), new Color(0.88f, 0.82f, 0.70f));
        for (int i = 0; i < 4; i++)
        {
            float x = -3.15f + i * 2.1f;
            AddCube("Cutting Board", new Vector3(x, 0.58f, -0.35f), new Vector3(1.24f, 0.08f, 0.88f), new Color(0.62f, 0.42f, 0.24f));
            AddSphere("Food Item", new Vector3(x + 0.26f, 0.75f, -0.42f), new Vector3(0.18f, 0.14f, 0.18f), PickPlayerColor(i));
            AddCube("Knife", new Vector3(x - 0.22f, 0.73f, -0.20f), new Vector3(0.52f, 0.04f, 0.09f), new Color(0.82f, 0.86f, 0.88f), 18f);
        }

        AddPlayerFiguresInLine(players, 0.92f, -1.18f, 0.58f);
    }

    private static void AddRadialStoneLines(float radius, int count)
    {
        for (int i = 0; i < count; i++)
        {
            float angle = 360f / count * i;
            var line = AddCube("Arena Groove", Vector3.zero, new Vector3(0.035f, 0.018f, radius), new Color(0.32f, 0.31f, 0.30f), angle);
            line.transform.localPosition = Quaternion.Euler(0f, angle, 0f) * new Vector3(0f, 0.72f, radius * 0.5f);
        }
    }

    private static void AddVineColumn(float x, float z)
    {
        AddCylinder("Vine Stem", new Vector3(x, 1.40f, z), new Vector3(0.09f, 2.1f, 0.09f), new Color(0.10f, 0.48f, 0.18f));
        for (int i = 0; i < 9; i++)
        {
            float y = -0.20f + i * 0.42f;
            float side = i % 2 == 0 ? -1f : 1f;
            AddCube("Vine Leaf", new Vector3(x + side * 0.20f, y, z), new Vector3(0.42f, 0.10f, 0.22f), new Color(0.20f, 0.66f, 0.26f), side * 24f);
        }
    }

    private static void AddCloud(Vector3 position)
    {
        AddSphere("Cloud Puff A", position, new Vector3(0.28f, 0.15f, 0.12f), Color.white);
        AddSphere("Cloud Puff B", position + new Vector3(0.22f, 0.02f, 0f), new Vector3(0.22f, 0.12f, 0.10f), Color.white);
        AddSphere("Cloud Puff C", position + new Vector3(-0.22f, 0.00f, 0f), new Vector3(0.20f, 0.11f, 0.10f), Color.white);
    }

    private static void AddRockCluster(float x, float z)
    {
        AddSphere("Arena Rock", new Vector3(x, 0.00f, z), new Vector3(0.44f, 0.30f, 0.36f), new Color(0.35f, 0.31f, 0.29f));
        AddSphere("Arena Rock", new Vector3(x + 0.42f, -0.02f, z + 0.22f), new Vector3(0.30f, 0.22f, 0.28f), new Color(0.30f, 0.28f, 0.27f));
    }

    private static void AddSheep(Vector3 position)
    {
        AddSphere("Wool Puff", position + new Vector3(0f, 0.34f, 0f), new Vector3(0.28f, 0.22f, 0.24f), new Color(0.94f, 0.92f, 0.84f));
        AddSphere("Sheep Head", position + new Vector3(0.27f, 0.34f, 0f), new Vector3(0.14f, 0.12f, 0.12f), new Color(0.30f, 0.24f, 0.20f));
    }

    private static void AddScooter(Vector3 position, Color color)
    {
        AddCube("Scooter Body", position + new Vector3(0f, 0.18f, 0f), new Vector3(0.56f, 0.22f, 0.82f), color);
        AddCube("Scooter Handle", position + new Vector3(0f, 0.64f, 0.30f), new Vector3(0.54f, 0.08f, 0.08f), new Color(0.10f, 0.11f, 0.12f));
        AddCube("Scooter Stem", position + new Vector3(0f, 0.44f, 0.24f), new Vector3(0.08f, 0.42f, 0.08f), new Color(0.10f, 0.11f, 0.12f));
        AddCylinder("Front Wheel", position + new Vector3(0f, -0.02f, 0.42f), new Vector3(0.22f, 0.06f, 0.22f), new Color(0.06f, 0.07f, 0.08f), 90f);
        AddCylinder("Back Wheel", position + new Vector3(0f, -0.02f, -0.36f), new Vector3(0.22f, 0.06f, 0.22f), new Color(0.06f, 0.07f, 0.08f), 90f);
    }

    private static void AddPlayerFigures(IReadOnlyList<PlayerController> players, float radius, float y)
    {
        if (players == null)
        {
            return;
        }

        for (int i = 0; i < players.Count; i++)
        {
            float angle = Mathf.PI * 2f * i / players.Count + Mathf.PI * 0.25f;
            Vector3 position = new Vector3(Mathf.Cos(angle) * radius, y, Mathf.Sin(angle) * radius * 0.72f);
            AddPlayerFigure(players[i], position);
        }
    }

    private static void AddPlayerFiguresInLine(IReadOnlyList<PlayerController> players, float spacing, float z, float y)
    {
        if (players == null)
        {
            return;
        }

        float startX = -(players.Count - 1) * spacing * 0.5f;
        for (int i = 0; i < players.Count; i++)
        {
            AddPlayerFigure(players[i], new Vector3(startX + i * spacing, y, z));
        }
    }

    private static void AddPlayerFigure(PlayerController player, Vector3 position)
    {
        Color color = player != null ? player.PlayerColor : Color.white;
        AddSphere("Player Head", position + new Vector3(0f, 0.42f, 0f), Vector3.one * 0.16f, new Color(0.96f, 0.78f, 0.58f));
        AddCylinder("Player Body", position + new Vector3(0f, 0.18f, 0f), new Vector3(0.18f, 0.22f, 0.18f), color);
        AddCube("Player Shadow", position + new Vector3(0f, -0.02f, 0f), new Vector3(0.48f, 0.025f, 0.30f), new Color(0f, 0f, 0f, 0.55f));
    }

    private static GameObject AddCube(string name, Vector3 position, Vector3 scale, Color color, float yaw = 0f)
    {
        GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        SetupPrimitive(gameObject, name, position, scale, color);
        gameObject.transform.rotation = Quaternion.Euler(0f, yaw, 0f);
        return gameObject;
    }

    private static GameObject AddCylinder(string name, Vector3 position, Vector3 scale, Color color, float roll = 0f)
    {
        GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        SetupPrimitive(gameObject, name, position, scale, color);
        gameObject.transform.rotation = Quaternion.Euler(0f, 0f, roll);
        return gameObject;
    }

    private static GameObject AddSphere(string name, Vector3 position, Vector3 scale, Color color)
    {
        GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        SetupPrimitive(gameObject, name, position, scale, color);
        return gameObject;
    }

    private static void SetupPrimitive(GameObject gameObject, string name, Vector3 position, Vector3 scale, Color color)
    {
        gameObject.name = name;
        gameObject.transform.SetParent(root.transform);
        gameObject.transform.position = position;
        gameObject.transform.localScale = scale;
        gameObject.GetComponent<Renderer>().material = RuntimeVisuals.CreateMaterial(name, color);

        var collider = gameObject.GetComponent<Collider>();
        if (collider != null)
        {
            Object.Destroy(collider);
        }
    }

    private static Color PickPlayerColor(int index)
    {
        Color[] colors =
        {
            new Color(0.20f, 0.64f, 0.95f),
            new Color(0.95f, 0.28f, 0.45f),
            new Color(0.32f, 0.76f, 0.34f),
            new Color(0.98f, 0.74f, 0.22f)
        };

        return colors[index % colors.Length];
    }
}
