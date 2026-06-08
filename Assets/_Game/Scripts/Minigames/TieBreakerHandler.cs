using System.Collections.Generic;
using System.Linq;

public static class TieBreakerHandler
{
    public static List<PlayerMinigameData> FindHighestTie(IReadOnlyList<PlayerMinigameData> players)
    {
        if (players == null || players.Count == 0)
        {
            return new List<PlayerMinigameData>();
        }

        int highest = players.Max(player => player.Score);
        return players.Where(player => player.Score == highest).OrderBy(player => player.PlayerId).ToList();
    }

    public static bool HasHighestTie(IReadOnlyList<PlayerMinigameData> players)
    {
        return FindHighestTie(players).Count > 1;
    }

    public static List<PlayerMinigameData> RankFinal(IReadOnlyList<PlayerMinigameData> players)
    {
        List<PlayerMinigameData> ordered = players
            .OrderByDescending(player => player.Score)
            .ThenBy(player => player.PlayerId)
            .ToList();

        for (int i = 0; i < ordered.Count; i++)
        {
            ordered[i].Rank = i + 1;
        }

        return ordered;
    }
}
