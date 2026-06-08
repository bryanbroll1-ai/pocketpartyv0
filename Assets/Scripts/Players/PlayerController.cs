using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int PlayerIndex { get; private set; }
    public string PlayerName { get; private set; }
    public Color PlayerColor { get; private set; }
    public bool IsBot { get; private set; }
    public int Coins { get; private set; }
    public int TileIndex { get; set; }

    public void Initialize(int playerIndex, string playerName, Color playerColor, bool isBot)
    {
        PlayerIndex = playerIndex;
        PlayerName = playerName;
        PlayerColor = playerColor;
        IsBot = isBot;
        Coins = 10;
        TileIndex = 0;
        name = playerName;
    }

    public void AddCoins(int amount)
    {
        Coins = Mathf.Max(0, Coins + amount);
    }
}
