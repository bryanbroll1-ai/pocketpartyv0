using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FrameworkDummyMinigame : MinigameBase
{
    private readonly Dictionary<PlayerController, float> scores = new Dictionary<PlayerController, float>();
    private Text countdownText;
    private Text timerText;
    private Text scoreText;

    protected override float CountdownDuration => 3f;
    protected override float GameDuration => 25f;

    public override void StartMinigame(IReadOnlyList<PlayerController> players, UIManager ui, System.Action<MinigameResult> onComplete)
    {
        base.StartMinigame(players, ui, onComplete);
        RuntimeMinigameArena.Show(MinigameArenaStyle.NeonSplit, players);
        MinigamePlayerSpawnSystem.SpawnPlayers(players, new Vector3(0f, 0.44f, -0.75f), 0.88f);

        ContentRoot = UI.ShowMinigameScreen("Framework Test", "Tap, hold, or swipe to charge your marker.", MinigameTheme.DashTrack);
        var panel = UI.CreatePanel(ContentRoot, "Framework Test HUD", new Color(0.03f, 0.04f, 0.05f, 0.62f), 330f);
        countdownText = UI.CreateLabel(panel, "3", 72, TextAnchor.MiddleCenter);
        timerText = UI.CreateLabel(panel, $"{GameDuration:0.0}s", 42, TextAnchor.MiddleCenter);
        scoreText = UI.CreateLabel(panel, "", 38, TextAnchor.MiddleCenter);

        scores.Clear();
        foreach (var player in Players)
        {
            scores[player] = 0f;
        }

        UpdateScoreText();
        StartCoroutine(RunCountdownAndTimer(countdownText, timerText));
    }

    protected override void OnMinigameStarted()
    {
        countdownText.text = "Go";
    }

    protected override void OnMinigameTimerTick(float remainingTime)
    {
        foreach (var player in Players)
        {
            if (player.IsBot)
            {
                scores[player] += Time.unscaledDeltaTime * Random.Range(18f, 28f);
                continue;
            }

            MinigameInputState input = GetInput(player);
            if (input.TapDown)
            {
                scores[player] += 80f;
            }

            if (input.Hold)
            {
                scores[player] += Time.unscaledDeltaTime * 42f;
            }

            if (input.Swipe)
            {
                scores[player] += Mathf.Clamp(input.SwipeDelta.magnitude * 0.75f, 100f, 260f);
            }
        }

        UpdateScoreText();
    }

    protected override void OnMinigameTimeExpired()
    {
        var result = new MinigameResult("Framework Test Results");
        foreach (var player in Players)
        {
            float playerScore = scores.ContainsKey(player) ? scores[player] : 0f;
            result.Scores[player] = playerScore;
            result.DisplayLines.Add($"{player.PlayerName}: {playerScore:0} charge");
        }

        result.RankHighScoreWins();
        Finish(result);
    }

    private void UpdateScoreText()
    {
        if (scoreText == null)
        {
            return;
        }

        var builder = new System.Text.StringBuilder();
        foreach (var player in Players)
        {
            float playerScore = scores.ContainsKey(player) ? scores[player] : 0f;
            builder.Append($"{player.PlayerName}: {playerScore:0}  ");
        }

        scoreText.text = builder.ToString();
    }
}
