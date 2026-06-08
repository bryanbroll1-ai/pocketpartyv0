using System.Collections.Generic;
using UnityEngine;

public class TowerTapClimbMinigame : BaseMinigame
{
    private readonly List<PlayerTowerMeter> meters = new List<PlayerTowerMeter>();
    private readonly Dictionary<int, float> pulseTimers = new Dictionary<int, float>();
    private readonly TapScoreController tapScores = new TapScoreController();

    public void RegisterMeters(IEnumerable<PlayerTowerMeter> towerMeters)
    {
        meters.Clear();
        if (towerMeters == null)
        {
            return;
        }

        foreach (PlayerTowerMeter meter in towerMeters)
        {
            if (meter != null)
            {
                meters.Add(meter);
            }
        }
    }

    protected override void OnGameStarted()
    {
        tapScores.Reset(Players);
        pulseTimers.Clear();
        for (int i = 0; i < Players.Count; i++)
        {
            GetMeter(Players[i].PlayerId)?.SetHeight(0f, 0);
        }
    }

    protected override void OnGameTick(float deltaTime)
    {
        for (int i = 0; i < Players.Count; i++)
        {
            PlayerMinigameData player = Players[i];
            PlayerInputState input = GetInput(player);
            player.Controller?.ApplyInput(input, deltaTime);

            if (input.ActionDown || input.TapDown)
            {
                float height = tapScores.RegisterTap(player);
                PlayerTowerMeter meter = GetMeter(player.PlayerId);
                if (meter != null)
                {
                    meter.SetHeight(height, player.Score);
                    meter.Pulse();
                    pulseTimers[player.PlayerId] = 0.10f;
                }
            }

            if (pulseTimers.TryGetValue(player.PlayerId, out float pulse))
            {
                pulse -= deltaTime;
                pulseTimers[player.PlayerId] = pulse;
                if (pulse <= 0f)
                {
                    GetMeter(player.PlayerId)?.Relax();
                }
            }
        }
    }

    protected override MinigameResult BuildResult()
    {
        List<PlayerMinigameData> ranked = RankingSystem.RankByScoreUnique(Players);
        MinigameResult result = new MinigameResult("Tower Tap Climb");
        result.SetPlayerResults(ranked);
        return result;
    }

    private PlayerTowerMeter GetMeter(int playerId)
    {
        return playerId >= 0 && playerId < meters.Count ? meters[playerId] : null;
    }
}
