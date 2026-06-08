using System.Collections.Generic;
using System.Linq;

public class MinigameResult
{
    public string Title { get; }
    public Dictionary<PlayerController, float> Scores { get; } = new Dictionary<PlayerController, float>();
    public List<PlayerController> Placements { get; private set; } = new List<PlayerController>();
    public List<string> DisplayLines { get; } = new List<string>();
    public List<PlayerMinigameData> PlayerResults { get; private set; } = new List<PlayerMinigameData>();
    public bool ReturnToBoardRequested { get; set; }

    public MinigameResult(string title)
    {
        Title = title;
    }

    public void RankHighScoreWins()
    {
        Placements = Scores.OrderByDescending(pair => pair.Value).Select(pair => pair.Key).ToList();
    }

    public void SetPlayerResults(IEnumerable<PlayerMinigameData> rankedPlayers)
    {
        PlayerResults = rankedPlayers != null ? rankedPlayers.ToList() : new List<PlayerMinigameData>();
        DisplayLines.Clear();
        foreach (PlayerMinigameData player in PlayerResults)
        {
            DisplayLines.Add($"{player.Rank}. {player.PlayerName} - {player.Score}");
        }
    }
}
