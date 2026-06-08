using System.Collections.Generic;

public class ScoreSystem
{
    private readonly List<PlayerMinigameData> players = new List<PlayerMinigameData>();

    public IReadOnlyList<PlayerMinigameData> Players => players;

    public void Initialize(IEnumerable<PlayerMinigameData> playerData)
    {
        players.Clear();
        if (playerData == null)
        {
            return;
        }

        players.AddRange(playerData);
    }

    public void AddScore(int playerId, int amount)
    {
        PlayerMinigameData player = Find(playerId);
        if (player != null && !player.IsEliminated)
        {
            player.AddScore(amount);
        }
    }

    public void SetScore(int playerId, int score)
    {
        PlayerMinigameData player = Find(playerId);
        if (player != null)
        {
            player.SetScore(score);
        }
    }

    public void SetLives(int playerId, int lives)
    {
        Find(playerId)?.SetLives(lives);
    }

    public PlayerMinigameData Find(int playerId)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].PlayerId == playerId)
            {
                return players[i];
            }
        }

        return null;
    }
}
