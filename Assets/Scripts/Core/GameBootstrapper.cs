using UnityEngine;
using UnityEngine.EventSystems;

public class GameBootstrapper : MonoBehaviour
{
    private static bool initialized;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetBootstrapState()
    {
        initialized = false;
    }

    private void Awake()
    {
        if (initialized)
        {
            Destroy(gameObject);
            return;
        }

        initialized = true;
        DontDestroyOnLoad(gameObject);

        Screen.orientation = ScreenOrientation.Portrait;
        EnsureCamera();
        EnsureLighting();
        EnsureEventSystem();

        var performanceSettings = GetOrCreateManager<PerformanceSettings>("Performance Settings");
        var uiManager = GetOrCreateManager<UIManager>("UI Manager");
        var boardManager = GetOrCreateManager<BoardManager>("Board Manager");
        var turnManager = GetOrCreateManager<TurnManager>("Turn Manager");
        var diceRoller = GetOrCreateManager<DiceRoller>("Dice Roller");
        var coinManager = GetOrCreateManager<CoinManager>("Coin Manager");
        var minigameManager = GetOrCreateManager<MinigameManager>("Minigame Manager");
        var audioManager = GetOrCreateManager<AudioManager>("Audio Manager");
        var resultsPanel = GetOrCreateManager<ResultsPanel>("Results Panel");
        var gameManager = GetOrCreateManager<GameManager>("Game Manager");
        var cameraFollow = Camera.main.GetComponent<CameraFollowController>();
        if (cameraFollow == null)
        {
            cameraFollow = Camera.main.gameObject.AddComponent<CameraFollowController>();
        }

        uiManager.Initialize();
        gameManager.Initialize(performanceSettings, uiManager, boardManager, turnManager, diceRoller, coinManager, minigameManager, audioManager, resultsPanel, cameraFollow);
    }

    private void EnsureCamera()
    {
        Camera camera = Camera.main;
        if (camera == null)
        {
            var cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";
            camera = cameraObject.AddComponent<Camera>();
        }

        camera.orthographic = false;
        // Slightly tighter FOV reads more like a hand-held toy diorama and
        // reduces edge distortion on tall portrait phones.
        camera.fieldOfView = 38f;
        camera.allowHDR = true;
        camera.clearFlags = CameraClearFlags.Skybox;
        camera.backgroundColor = new Color(0.16f, 0.29f, 0.38f);
        camera.nearClipPlane = 0.1f;
        camera.farClipPlane = 140f;
        camera.transform.position = new Vector3(0f, 7.0f, -7.4f);
        camera.transform.rotation = Quaternion.LookRotation(new Vector3(0f, 0.4f, 0f) - camera.transform.position, Vector3.up);
    }

    private void EnsureLighting()
    {
        // Gradient ("trilight") ambient gives soft, free bounce light: warm
        // from the sky, cooler in shadow. Flat ambient is the main reason the
        // old look felt plasticky and even.
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
        RenderSettings.ambientSkyColor = new Color(0.82f, 0.90f, 1.00f);
        RenderSettings.ambientEquatorColor = new Color(0.66f, 0.70f, 0.74f);
        RenderSettings.ambientGroundColor = new Color(0.30f, 0.34f, 0.34f);
        RenderSettings.ambientIntensity = 1.0f;

        // Procedural gradient sky for depth instead of a flat clear colour.
        Material sky = RuntimeVisuals.CreateGradientSkybox(
            new Color(0.55f, 0.78f, 1.00f),
            new Color(0.42f, 0.46f, 0.50f));
        if (sky != null)
        {
            RenderSettings.skybox = sky;
        }

        // Light fog blends distant water/props into the horizon = more depth.
        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.Linear;
        RenderSettings.fogColor = new Color(0.78f, 0.86f, 0.93f);
        RenderSettings.fogStartDistance = 16f;
        RenderSettings.fogEndDistance = 60f;

        // Three-point rig: warm key (with soft shadows), cool fill, warm rim.
        Light key = FindAnyObjectByType<Light>();
        if (key == null)
        {
            key = new GameObject("Key Sun").AddComponent<Light>();
        }

        key.name = "Key Sun";
        key.type = LightType.Directional;
        key.color = new Color(1.00f, 0.95f, 0.84f);
        key.intensity = 1.18f;
        key.shadows = LightShadows.Soft;
        key.shadowStrength = 0.55f;
        key.shadowBias = 0.04f;
        key.shadowNormalBias = 0.5f;
        key.transform.rotation = Quaternion.Euler(48f, -34f, 0f);

        CreateFillLight("Cool Fill", new Color(0.66f, 0.78f, 1.00f), 0.42f, Quaternion.Euler(28f, 150f, 0f));
        CreateFillLight("Warm Rim", new Color(1.00f, 0.82f, 0.62f), 0.55f, Quaternion.Euler(-18f, -150f, 0f));

        DynamicGI.UpdateEnvironment();
    }

    private void CreateFillLight(string lightName, Color color, float intensity, Quaternion rotation)
    {
        if (transform.Find(lightName) != null)
        {
            return;
        }

        var fill = new GameObject(lightName).AddComponent<Light>();
        fill.transform.SetParent(transform, false);
        fill.name = lightName;
        fill.type = LightType.Directional;
        fill.color = color;
        fill.intensity = intensity;
        fill.shadows = LightShadows.None;
        fill.transform.rotation = rotation;
    }

    private void EnsureEventSystem()
    {
        if (FindAnyObjectByType<EventSystem>() != null)
        {
            return;
        }

        var eventSystemObject = new GameObject("EventSystem");
        eventSystemObject.AddComponent<EventSystem>();
        eventSystemObject.AddComponent<StandaloneInputModule>();
    }

    private T GetOrCreateManager<T>(string objectName) where T : Component
    {
        T existing = FindAnyObjectByType<T>();
        if (existing != null)
        {
            return existing;
        }

        var managerObject = new GameObject(objectName);
        return managerObject.AddComponent<T>();
    }
}
