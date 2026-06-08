using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MinigameManager : MonoBehaviour
{
    private BaseMinigame activeTemplateMinigame;

    private readonly List<MinigameType> minigameOrder = new List<MinigameType>
    {
        MinigameType.FrameworkDummy,
        MinigameType.TimeStop,
        MinigameType.ColorRush,
        MinigameType.HoldRelease,
        MinigameType.SwipeDash,
        MinigameType.TapTargets,
        MinigameType.MemoryTiles,
        MinigameType.VineClimb,
        MinigameType.PlankRaft,
        MinigameType.ParasolDrop,
        MinigameType.CastlePose,
        MinigameType.PhotoMatch,
        MinigameType.StackLights,
        MinigameType.SheepDodge,
        MinigameType.MinecartSwitch,
        MinigameType.CookingChop
    };

    private int nextMinigameIndex;

    public void LoadMinigameScene(MinigameType type)
    {
        string scenePath = GetScenePath(type);
        if (string.IsNullOrEmpty(scenePath))
        {
            Debug.LogWarning($"Minigame type {type} does not have a scene-backed launcher yet.");
            return;
        }

        SceneManager.LoadScene(scenePath);
    }

    public void StartTemplateMinigame(BaseMinigame minigame, MinigamePlayerSpawner spawner, MinigameUIController ui, MinigameInputRouter inputRouter, Action<MinigameResult> onComplete = null)
    {
        StartTemplateMinigame(minigame, spawner, ui, inputRouter, null, onComplete);
    }

    public void StartTemplateMinigame(BaseMinigame minigame, MinigamePlayerSpawner spawner, MinigameUIController ui, MinigameInputRouter inputRouter, Action<IReadOnlyList<PlayerMinigameData>> onPlayersCreated, Action<MinigameResult> onComplete = null)
    {
        if (minigame == null || spawner == null || ui == null || inputRouter == null)
        {
            Debug.LogError("Template minigame is missing a required scene reference.");
            return;
        }

        PlayerMinigameData[] players = PlayerMinigameData.CreateDefaultFour();
        spawner.SpawnPlayers(players);
        ui.Build(players);
        inputRouter.RegisterMobileZones(ui.MobileZones);
        onPlayersCreated?.Invoke(players);
        activeTemplateMinigame = minigame;
        activeTemplateMinigame.Initialize(players, ui, inputRouter, onComplete);
        activeTemplateMinigame.Begin();
    }

    public void CancelTemplateMinigame()
    {
        if (activeTemplateMinigame != null)
        {
            activeTemplateMinigame.RequestEnd();
        }
    }

    public void StartNextMinigame(IReadOnlyList<PlayerController> players, UIManager ui, Action<MinigameResult> onComplete)
    {
        MinigameType type = minigameOrder[nextMinigameIndex % minigameOrder.Count];
        nextMinigameIndex++;
        StartMinigame(type, players, ui, onComplete);
    }

    public void StartMinigame(MinigameType type, IReadOnlyList<PlayerController> players, UIManager ui, Action<MinigameResult> onComplete)
    {
        var minigameObject = new GameObject($"{type} Minigame");
        minigameObject.transform.SetParent(transform);

        MinigameBase minigame;
        switch (type)
        {
            case MinigameType.FrameworkDummy:
                minigame = minigameObject.AddComponent<FrameworkDummyMinigame>();
                break;
            case MinigameType.ColorRush:
                minigame = minigameObject.AddComponent<ColorRushMinigame>();
                break;
            case MinigameType.HoldRelease:
                minigame = minigameObject.AddComponent<HoldReleaseMinigame>();
                break;
            case MinigameType.SwipeDash:
                minigame = minigameObject.AddComponent<SwipeDashMinigame>();
                break;
            case MinigameType.TapTargets:
                minigame = minigameObject.AddComponent<TapTargetsMinigame>();
                break;
            case MinigameType.MemoryTiles:
                minigame = minigameObject.AddComponent<MemoryTilesMinigame>();
                break;
            case MinigameType.VineClimb:
            case MinigameType.PlankRaft:
            case MinigameType.ParasolDrop:
            case MinigameType.CastlePose:
            case MinigameType.PhotoMatch:
            case MinigameType.StackLights:
            case MinigameType.SheepDodge:
            case MinigameType.MinecartSwitch:
            case MinigameType.CookingChop:
                var arenaChallenge = minigameObject.AddComponent<ArenaChallengeMinigame>();
                arenaChallenge.Configure(type);
                minigame = arenaChallenge;
                break;
            default:
                minigame = minigameObject.AddComponent<TimeStopMinigame>();
                break;
        }

        minigame.StartMinigame(players, ui, onComplete);
    }

    private static string GetScenePath(MinigameType type)
    {
        switch (type)
        {
            case MinigameType.LuckyRollShowdown:
                return "Assets/_Game/Scenes/Minigames/LuckyRollShowdown.unity";
            case MinigameType.TowerTapClimb:
                return "Assets/_Game/Scenes/Minigames/TowerTapClimb.unity";
            default:
                return string.Empty;
        }
    }
}
