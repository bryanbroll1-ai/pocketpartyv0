using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HoldReleaseMinigame : MinigameBase
{
    private Text status;
    private Image chargeFill;
    private float charge;
    private float target;
    private bool released;

    public override void StartMinigame(IReadOnlyList<PlayerController> players, UIManager ui, System.Action<MinigameResult> onComplete)
    {
        base.StartMinigame(players, ui, onComplete);
        RuntimeMinigameArena.Show(MinigameArenaStyle.BridgeDuel, players);
        target = Random.Range(0.55f, 0.82f);
        ContentRoot = UI.ShowMinigameScreen("Hold & Release", "Hold to charge the core, release near the sweet spot.", MinigameTheme.PowerCore);
        var corePanel = UI.CreatePanel(ContentRoot, "Power Core Panel", new Color(0.04f, 0.08f, 0.06f, 0.70f), 330f);
        chargeFill = UI.CreateProgressBar(corePanel, "Charge Meter", new Color(0.02f, 0.04f, 0.04f, 0.85f), new Color(0.34f, 0.90f, 0.52f), 116f);
        UI.CreateLabel(corePanel, $"Perfect zone: {target:0.00}", 42, TextAnchor.MiddleCenter);
        status = UI.CreateLabel(ContentRoot, "", 52, TextAnchor.MiddleCenter);
    }

    private void Update()
    {
        if (!IsRunning || released)
        {
            return;
        }

        if (PointerHeld())
        {
            charge = Mathf.PingPong(Time.unscaledTime * 0.72f, 1f);
        }

        status.text = $"Power: {charge:0.00}";
        chargeFill.fillAmount = charge;

        if (PointerUpThisFrame())
        {
            released = true;
            Complete(charge);
        }
    }

    private void Complete(float humanCharge)
    {
        var result = new MinigameResult("Hold & Release Results");
        foreach (var player in Players)
        {
            float value = player.IsBot ? Mathf.Clamp01(target + Random.Range(-0.22f, 0.22f)) : humanCharge;
            float score = Mathf.Max(0f, 1000f - Mathf.Abs(target - value) * 1600f);
            result.Scores[player] = score;
            result.DisplayLines.Add($"{player.PlayerName}: {value:0.00} ({score:0} pts)");
        }

        result.RankHighScoreWins();
        Finish(result);
    }
}
