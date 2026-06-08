using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TimeStopMinigame : MinigameBase
{
    private Text prompt;
    private Text playerResult;
    private float startTime;
    private bool acceptingTap;

    public override void StartMinigame(System.Collections.Generic.IReadOnlyList<PlayerController> players, UIManager ui, System.Action<MinigameResult> onComplete)
    {
        base.StartMinigame(players, ui, onComplete);
        RuntimeMinigameArena.Show(MinigameArenaStyle.LavaCircle, players);
        ContentRoot = UI.ShowMinigameScreen("Time Stop", "Tap once when you feel the hidden second land.", MinigameTheme.Clockwork);
        var clockPanel = UI.CreatePanel(ContentRoot, "Clock Face Panel", new Color(0.04f, 0.05f, 0.08f, 0.72f), 430f);
        UI.CreateLabel(clockPanel, "1.000", 86, TextAnchor.MiddleCenter);
        prompt = UI.CreateLabel(clockPanel, "Ready", 62, TextAnchor.MiddleCenter);
        playerResult = UI.CreateLabel(clockPanel, "", 42, TextAnchor.MiddleCenter);
        StartCoroutine(CountdownRoutine());
    }

    private IEnumerator CountdownRoutine()
    {
        yield return new WaitForSeconds(0.6f);
        prompt.text = "Go!";
        startTime = Time.unscaledTime;
        acceptingTap = true;
    }

    private void Update()
    {
        if (!IsRunning || !acceptingTap || !PointerDownThisFrame())
        {
            return;
        }

        acceptingTap = false;
        float stoppedTime = Time.unscaledTime - startTime;
        playerResult.text = $"Stopped at {stoppedTime:0.000}s";
        StartCoroutine(CompleteAfterPause(stoppedTime));
    }

    private IEnumerator CompleteAfterPause(float humanStoppedTime)
    {
        yield return new WaitForSeconds(0.8f);

        var result = new MinigameResult("Time Stop Results");
        foreach (var player in Players)
        {
            float stoppedTime = player.IsBot ? Random.Range(0.78f, 1.22f) : humanStoppedTime;
            float difference = Mathf.Abs(1f - stoppedTime);
            float score = Mathf.Max(0f, 1000f - difference * 1000f);
            result.Scores[player] = score;
            result.DisplayLines.Add($"{player.PlayerName}: {stoppedTime:0.000}s ({difference:0.000} off)");
        }

        result.RankHighScoreWins();
        Finish(result);
    }
}
