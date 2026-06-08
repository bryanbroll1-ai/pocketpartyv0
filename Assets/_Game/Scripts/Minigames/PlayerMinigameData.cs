using UnityEngine;

public class PlayerMinigameData
{
    public int PlayerId { get; }
    public string PlayerName { get; }
    public Color PlayerColor { get; }
    public int Score { get; private set; }
    public int Lives { get; private set; }
    public bool IsEliminated { get; private set; }
    public int Rank { get; set; }
    public float SurvivalTime { get; private set; }
    public float Distance { get; private set; }
    public GameObject AvatarObject { get; set; }
    public PlayerMinigameController Controller { get; set; }

    public PlayerMinigameData(int playerId, string playerName, Color playerColor, int lives = 3)
    {
        PlayerId = playerId;
        PlayerName = playerName;
        PlayerColor = playerColor;
        Lives = lives;
    }

    public void AddScore(int amount)
    {
        Score += amount;
    }

    public void SetScore(int score)
    {
        Score = Mathf.Max(0, score);
    }

    public void SetLives(int lives)
    {
        Lives = Mathf.Max(0, lives);
        IsEliminated = Lives == 0;
    }

    public void SetEliminated(bool eliminated)
    {
        IsEliminated = eliminated;
        if (eliminated && Lives > 0)
        {
            Lives = 0;
        }
    }

    public void AddSurvivalTime(float deltaTime)
    {
        if (!IsEliminated)
        {
            SurvivalTime += Mathf.Max(0f, deltaTime);
        }
    }

    public void SetDistance(float distance)
    {
        Distance = Mathf.Max(Distance, distance);
    }

    public static PlayerMinigameData[] CreateDefaultFour()
    {
        return new[]
        {
            new PlayerMinigameData(0, "P1", PartyArtPalette.PlayerBlue),
            new PlayerMinigameData(1, "P2", PartyArtPalette.PlayerRed),
            new PlayerMinigameData(2, "P3", PartyArtPalette.PlayerGreen),
            new PlayerMinigameData(3, "P4", PartyArtPalette.PlayerYellow)
        };
    }
}
