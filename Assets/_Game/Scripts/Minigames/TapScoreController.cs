using System.Collections.Generic;
using UnityEngine;

public class TapScoreController
{
    private readonly Dictionary<int, int> tapCounts = new Dictionary<int, int>();

    public float HeightPerTap { get; set; } = 0.10f;
    public float MaxVisualHeight { get; set; } = 3.45f;

    public void Reset(IReadOnlyList<PlayerMinigameData> players)
    {
        tapCounts.Clear();
        if (players == null)
        {
            return;
        }

        for (int i = 0; i < players.Count; i++)
        {
            tapCounts[players[i].PlayerId] = 0;
            players[i].SetScore(0);
        }
    }

    public float RegisterTap(PlayerMinigameData player)
    {
        if (player == null)
        {
            return 0f;
        }

        int next = GetTapCount(player.PlayerId) + 1;
        tapCounts[player.PlayerId] = next;
        player.SetScore(next);
        return GetVisualHeight(player.PlayerId);
    }

    public int GetTapCount(int playerId)
    {
        return tapCounts.TryGetValue(playerId, out int count) ? count : 0;
    }

    public float GetVisualHeight(int playerId)
    {
        return Mathf.Clamp(GetTapCount(playerId) * HeightPerTap, 0f, MaxVisualHeight);
    }
}
