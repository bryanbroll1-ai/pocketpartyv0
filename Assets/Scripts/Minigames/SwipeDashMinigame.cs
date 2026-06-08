using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwipeDashMinigame : MinigameBase
{
    private const float Duration = 10f;
    private Text status;
    private Image distanceFill;
    private float endTime;
    private float distance;
    private Vector2 swipeStart;
    private bool trackingSwipe;

    public override void StartMinigame(IReadOnlyList<PlayerController> players, UIManager ui, System.Action<MinigameResult> onComplete)
    {
        base.StartMinigame(players, ui, onComplete);
        RuntimeMinigameArena.Show(MinigameArenaStyle.SkiRace, players);
        endTime = Time.unscaledTime + Duration;
        ContentRoot = UI.ShowMinigameScreen("Swipe Dash", "Swipe to launch your runner along the neon track.", MinigameTheme.DashTrack);
        var trackPanel = UI.CreatePanel(ContentRoot, "Dash Track Panel", new Color(0.03f, 0.08f, 0.11f, 0.74f), 310f);
        distanceFill = UI.CreateProgressBar(trackPanel, "Track Progress", new Color(0.02f, 0.03f, 0.05f, 0.92f), new Color(0.20f, 0.72f, 0.95f), 96f);
        UI.CreateLabel(trackPanel, "Swipe lanes", 42, TextAnchor.MiddleCenter);
        status = UI.CreateLabel(ContentRoot, "", 52, TextAnchor.MiddleCenter);
    }

    private void Update()
    {
        if (!IsRunning)
        {
            return;
        }

        if (Time.unscaledTime >= endTime)
        {
            Complete();
            return;
        }

        if (PointerDownThisFrame())
        {
            swipeStart = PointerPosition();
            trackingSwipe = true;
        }
        else if (trackingSwipe && PointerUpThisFrame())
        {
            Vector2 delta = PointerPosition() - swipeStart;
            if (delta.magnitude > 70f && (delta.y > 30f || Mathf.Abs(delta.x) > 70f))
            {
                distance += 1f;
            }

            trackingSwipe = false;
        }

        status.text = $"Distance: {distance:0}\nTime: {Mathf.Max(0f, endTime - Time.unscaledTime):0.0}s";
        distanceFill.fillAmount = Mathf.Clamp01(distance / 24f);
    }

    private void Complete()
    {
        var result = new MinigameResult("Swipe Dash Results");
        foreach (var player in Players)
        {
            float score = player.IsBot ? Random.Range(12, 27) : distance;
            result.Scores[player] = score;
            result.DisplayLines.Add($"{player.PlayerName}: {score:0} dashes");
        }

        result.RankHighScoreWins();
        Finish(result);
    }
}
