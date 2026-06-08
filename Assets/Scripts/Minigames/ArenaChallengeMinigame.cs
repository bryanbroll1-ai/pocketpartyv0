using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArenaChallengeMinigame : MinigameBase
{
    private readonly List<Button> buttons = new List<Button>();
    private MinigameType challengeType;
    private MinigameArenaStyle arenaStyle;
    private MinigameTheme theme;
    private Text status;
    private string title;
    private string prompt;
    private string[] actions;
    private int targetIndex;
    private int round;
    private int score;
    private float roundStartTime;

    public void Configure(MinigameType type)
    {
        challengeType = type;
    }

    public override void StartMinigame(IReadOnlyList<PlayerController> players, UIManager ui, System.Action<MinigameResult> onComplete)
    {
        base.StartMinigame(players, ui, onComplete);
        ConfigureChallenge();
        RuntimeMinigameArena.Show(arenaStyle, players);
        ContentRoot = UI.ShowMinigameScreen(title, prompt, theme);

        var controlPanel = UI.CreatePanel(ContentRoot, $"{title} Controls", new Color(0.03f, 0.04f, 0.05f, 0.58f), 300f);
        status = UI.CreateLabel(controlPanel, "", 44, TextAnchor.MiddleCenter);
        var row = UI.CreateRow(controlPanel);
        buttons.Clear();
        for (int i = 0; i < actions.Length; i++)
        {
            int actionIndex = i;
            buttons.Add(UI.CreateButton(row, actions[i], () => SelectAction(actionIndex)));
        }

        StartRound();
    }

    private void ConfigureChallenge()
    {
        theme = MinigameTheme.DashTrack;
        switch (challengeType)
        {
            case MinigameType.VineClimb:
                title = "Vine Climb";
                prompt = "Pick the called vine lane before the clouds pass.";
                arenaStyle = MinigameArenaStyle.VineClimb;
                theme = MinigameTheme.ColorPop;
                actions = new[] { "Left", "Mid L", "Mid R", "Right" };
                break;
            case MinigameType.PlankRaft:
                title = "Log Roll Raft";
                prompt = "React to the rolling log on the wooden raft.";
                arenaStyle = MinigameArenaStyle.PlankRaft;
                theme = MinigameTheme.TargetField;
                actions = new[] { "Hop", "Duck", "Left", "Right" };
                break;
            case MinigameType.ParasolDrop:
                title = "Parasol Drop";
                prompt = "Match the parachute color as the wall rushes by.";
                arenaStyle = MinigameArenaStyle.ParasolWall;
                theme = MinigameTheme.MemoryGlow;
                actions = new[] { "Red", "Blue", "Green", "Yellow" };
                break;
            case MinigameType.CastlePose:
                title = "Castle Pose";
                prompt = "Copy the stage pose before the curtain snaps shut.";
                arenaStyle = MinigameArenaStyle.CastleStage;
                theme = MinigameTheme.PowerCore;
                actions = new[] { "Up", "Down", "Left", "Right" };
                break;
            case MinigameType.PhotoMatch:
                title = "Photo Match";
                prompt = "Choose the photo panel that matches the center shot.";
                arenaStyle = MinigameArenaStyle.PhotoPanels;
                theme = MinigameTheme.Clockwork;
                actions = new[] { "A", "B", "C", "D" };
                break;
            case MinigameType.StackLights:
                title = "Stack Lights";
                prompt = "Hit the glowing stack column as it peaks.";
                arenaStyle = MinigameArenaStyle.StackLights;
                theme = MinigameTheme.DashTrack;
                actions = new[] { "Blue", "Gold", "Green", "Pink" };
                break;
            case MinigameType.SheepDodge:
                title = "Sheep Dodge";
                prompt = "Dodge the flock in the dirt lane.";
                arenaStyle = MinigameArenaStyle.SheepTrail;
                theme = MinigameTheme.ColorPop;
                actions = new[] { "Left", "Right", "Jump", "Hold" };
                break;
            case MinigameType.MinecartSwitch:
                title = "Minecart Switch";
                prompt = "Set the track arrows before the cart reaches them.";
                arenaStyle = MinigameArenaStyle.MinecartTrack;
                theme = MinigameTheme.PowerCore;
                actions = new[] { "Up", "Down", "Left", "Right" };
                break;
            default:
                title = "Cooking Chop";
                prompt = "Follow the kitchen callout at your counter.";
                arenaStyle = MinigameArenaStyle.KitchenCounters;
                theme = MinigameTheme.TargetField;
                actions = new[] { "Chop", "Stir", "Flip", "Serve" };
                break;
        }
    }

    private void StartRound()
    {
        round++;
        targetIndex = Random.Range(0, actions.Length);
        roundStartTime = Time.unscaledTime;
        status.text = $"Round {round}/5: {actions[targetIndex]}";

        for (int i = 0; i < buttons.Count; i++)
        {
            ColorBlock colors = buttons[i].colors;
            colors.normalColor = i == targetIndex ? new Color(0.18f, 0.31f, 0.36f, 0.98f) : new Color(0.10f, 0.15f, 0.19f, 0.96f);
            buttons[i].colors = colors;
        }
    }

    private void SelectAction(int actionIndex)
    {
        if (!IsRunning)
        {
            return;
        }

        float reactionTime = Time.unscaledTime - roundStartTime;
        if (actionIndex == targetIndex)
        {
            score += Mathf.RoundToInt(Mathf.Max(180f, 950f - reactionTime * 210f));
        }
        else
        {
            score = Mathf.Max(0, score - 120);
        }

        if (round >= 5)
        {
            Complete();
            return;
        }

        StartRound();
    }

    private void Complete()
    {
        var result = new MinigameResult($"{title} Results");
        foreach (var player in Players)
        {
            int playerScore = player.IsBot ? Random.Range(2300, 4300) : score;
            result.Scores[player] = playerScore;
            result.DisplayLines.Add($"{player.PlayerName}: {playerScore} pts");
        }

        result.RankHighScoreWins();
        Finish(result);
    }
}
