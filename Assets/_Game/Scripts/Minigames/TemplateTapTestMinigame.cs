using System.Collections.Generic;
using UnityEngine;

public class TemplateTapTestMinigame : BaseMinigame
{
    private readonly Dictionary<int, float> pulseTimers = new Dictionary<int, float>();

    protected override void OnGameTick(float deltaTime)
    {
        for (int i = 0; i < Players.Count; i++)
        {
            PlayerMinigameData player = Players[i];
            PlayerInputState input = GetInput(player);
            player.Controller?.ApplyInput(input, deltaTime);

            if (input.TapDown || input.ActionDown)
            {
                Scores.AddScore(player.PlayerId, 1);
                pulseTimers[player.PlayerId] = 0.16f;
                if (player.Controller != null)
                {
                    player.Controller.transform.localScale = Vector3.one * 1.12f;
                }
            }

            if (pulseTimers.TryGetValue(player.PlayerId, out float pulse))
            {
                pulse -= deltaTime;
                pulseTimers[player.PlayerId] = pulse;
                if (pulse <= 0f && player.Controller != null)
                {
                    player.Controller.transform.localScale = Vector3.one;
                }
            }
        }
    }

    protected override MinigameResult BuildResult()
    {
        List<PlayerMinigameData> ranked = RankingSystem.RankByScore(Players);
        MinigameResult result = new MinigameResult("Template Tap Test");
        result.SetPlayerResults(ranked);
        return result;
    }
}
