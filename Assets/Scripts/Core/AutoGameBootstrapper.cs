using UnityEngine;
using UnityEngine.SceneManagement;

public static class AutoGameBootstrapper
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void CreateBootstrapper()
    {
        string activeSceneName = SceneManager.GetActiveScene().name;
        if (activeSceneName == "ArtStyle_Showcase" || activeSceneName == "Minigame_Template" || activeSceneName == "LuckyRollShowdown" || activeSceneName == "TowerTapClimb")
        {
            return;
        }

        if (Object.FindAnyObjectByType<GameBootstrapper>() != null)
        {
            return;
        }

        var bootstrapperObject = new GameObject("GameBootstrapper");
        Object.DontDestroyOnLoad(bootstrapperObject);
        bootstrapperObject.AddComponent<GameBootstrapper>();
    }
}
