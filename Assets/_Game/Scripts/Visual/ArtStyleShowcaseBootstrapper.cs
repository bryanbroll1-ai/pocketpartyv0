using UnityEngine;

public class ArtStyleShowcaseBootstrapper : MonoBehaviour
{
    private Transform firstAvatar;

    private void Awake()
    {
        SetupLighting();
        CameraPresetController cameraPreset = SetupCamera();
        BuildWorld();
        BuildAvatars();
        if (cameraPreset != null && firstAvatar != null)
        {
            cameraPreset.SetFollowTarget(firstAvatar);
        }

        BuildDemoUi();
    }

    private void SetupLighting()
    {
        var lightingObject = new GameObject("Showcase Lighting Preset");
        lightingObject.AddComponent<LightingPresetController>();
    }

    private CameraPresetController SetupCamera()
    {
        var cameraObject = new GameObject("Showcase Camera");
        cameraObject.tag = "MainCamera";
        var camera = cameraObject.AddComponent<Camera>();
        camera.backgroundColor = PartyArtPalette.Sky;
        camera.fieldOfView = 48f;
        camera.nearClipPlane = 0.1f;
        camera.farClipPlane = 120f;
        cameraObject.transform.position = new Vector3(0f, 10.2f, -13.2f);
        cameraObject.transform.rotation = Quaternion.LookRotation(new Vector3(0f, 0.55f, 0.25f) - cameraObject.transform.position, Vector3.up);
        return cameraObject.AddComponent<CameraPresetController>();
    }

    private void BuildWorld()
    {
        AddCube("Showcase Grass Base", new Vector3(0f, -0.20f, 0f), new Vector3(18.8f, 0.22f, 15.4f), PartyArtMaterialType.Grass);
        AddCube("Warm Gallery Path", new Vector3(0f, -0.06f, -0.18f), new Vector3(17.0f, 0.06f, 1.05f), PartyArtMaterialType.Platform);
        AddCube("Vertical Gallery Path", new Vector3(0f, -0.05f, 0f), new Vector3(1.05f, 0.06f, 13.3f), PartyArtMaterialType.Platform);
        AddCylinder("Avatar Showcase Podium", new Vector3(0f, 0.04f, -6.12f), new Vector3(2.75f, 0.14f, 1.18f), PartyArtMaterialType.WhiteAccent);
        AddCylinder("Podium Color Rim", new Vector3(0f, 0.14f, -6.12f), new Vector3(2.88f, 0.05f, 1.28f), PartyArtMaterialType.PlayerBlue);

        CreateKit(EnvironmentKitType.Kitchen, new Vector3(-5.8f, 0f, 4.6f));
        CreateKit(EnvironmentKitType.FarmRail, new Vector3(0f, 0f, 4.6f));
        CreateKit(EnvironmentKitType.SidePlatform, new Vector3(5.8f, 0f, 4.6f));
        CreateKit(EnvironmentKitType.StageBoss, new Vector3(-5.8f, 0f, 0.1f));
        CreateKit(EnvironmentKitType.SkyArena, new Vector3(0f, 0f, 0.1f));
        CreateKit(EnvironmentKitType.FieldAnimal, new Vector3(5.8f, 0f, 0.1f));
        CreateKit(EnvironmentKitType.Bridge, new Vector3(-2.9f, 0f, -4.35f));
        CreateKit(EnvironmentKitType.LavaArena, new Vector3(2.9f, 0f, -4.35f));
    }

    private void CreateKit(EnvironmentKitType type, Vector3 position)
    {
        GameObject kitObject = new GameObject($"{type} Kit");
        kitObject.transform.position = position;
        EnvironmentKitBuilder kit = kitObject.AddComponent<EnvironmentKitBuilder>();
        kit.Configure(type);
        kit.Build();
    }

    private void BuildAvatars()
    {
        Color[] colors =
        {
            PartyArtPalette.PlayerBlue,
            PartyArtPalette.PlayerRed,
            PartyArtPalette.PlayerGreen,
            PartyArtPalette.PlayerYellow
        };

        for (int i = 0; i < 4; i++)
        {
            var avatar = new GameObject($"Showcase Player {i + 1}");
            avatar.transform.position = new Vector3(-1.95f + i * 1.3f, 0.18f, -6.12f);
            if (i == 0)
            {
                firstAvatar = avatar.transform;
            }

            var visual = avatar.AddComponent<PlayerVisualController>();
            visual.Configure(i, colors[i]);
            visual.SetMood(i == 0 ? PlayerAvatarMood.Win : PlayerAvatarMood.Idle);
        }
    }

    private void BuildDemoUi()
    {
        PartyUIShowcaseFactory.BuildShowcaseUi();
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
