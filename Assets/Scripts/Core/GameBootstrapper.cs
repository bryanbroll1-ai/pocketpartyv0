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
        camera.fieldOfView = 42f;
        camera.backgroundColor = new Color(0.16f, 0.29f, 0.38f);
        camera.transform.position = new Vector3(0f, 6.8f, -7.2f);
        camera.transform.rotation = Quaternion.LookRotation(new Vector3(0f, 0.4f, 0f) - camera.transform.position, Vector3.up);
    }

    private void EnsureLighting()
    {
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(0.70f, 0.76f, 0.80f);

        Light light = FindAnyObjectByType<Light>();
        if (light == null)
        {
            var lightObject = new GameObject("Soft Sun");
            light = lightObject.AddComponent<Light>();
            light.type = LightType.Directional;
        }

        light.name = "Soft Sun";
        light.type = LightType.Directional;
        light.shadows = LightShadows.None;
        light.intensity = 0.85f;
        light.color = new Color(1f, 0.93f, 0.82f);
        light.transform.rotation = Quaternion.Euler(45f, -35f, 0f);
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
