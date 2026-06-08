using System.Collections.Generic;
using System.Linq;

public static class RankingSystem
{
    public static List<PlayerMinigameData> RankByScore(IEnumerable<PlayerMinigameData> players)
    {
        return ApplyRanks(players.OrderByDescending(player => player.Score).ThenBy(player => player.PlayerId).ToList(), player => player.Score);
    }

    public static List<PlayerMinigameData> RankByScoreUnique(IEnumerable<PlayerMinigameData> players)
    {
        List<PlayerMinigameData> ordered = players.OrderByDescending(player => player.Score).ThenBy(player => player.PlayerId).ToList();
        for (int i = 0; i < ordered.Count; i++)
        {
            ordered[i].Rank = i + 1;
        }

        return ordered;
    }

    public static List<PlayerMinigameData> RankBySurvivalTime(IEnumerable<PlayerMinigameData> players)
    {
        return ApplyRanks(players.OrderByDescending(player => player.SurvivalTime).ThenBy(player => player.PlayerId).ToList(), player => player.SurvivalTime);
    }

    public static List<PlayerMinigameData> RankByDistance(IEnumerable<PlayerMinigameData> players)
    {
        return ApplyRanks(players.OrderByDescending(player => player.Distance).ThenBy(player => player.PlayerId).ToList(), player => player.Distance);
    }

    private static List<PlayerMinigameData> ApplyRanks<T>(List<PlayerMinigameData> ordered, System.Func<PlayerMinigameData, T> valueSelector)
    {
        int currentRank = 0;
        T previousValue = default;
        bool hasPrevious = false;
        EqualityComparer<T> comparer = EqualityComparer<T>.Default;

        for (int i = 0; i < ordered.Count; i++)
        {
            T value = valueSelector(ordered[i]);
            if (!hasPrevious || !comparer.Equals(value, previousValue))
            {
                currentRank = i + 1;
                previousValue = value;
                hasPrevious = true;
            }

            ordered[i].Rank = currentRank;
        }

        return ordered;
    }
}
