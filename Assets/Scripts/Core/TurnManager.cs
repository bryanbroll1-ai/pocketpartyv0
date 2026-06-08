using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public int CurrentRound { get; private set; } = 1;
    public int CurrentTurnIndex { get; private set; }
    public PlayerController CurrentPlayer => players.Count == 0 ? null : players[CurrentTurnIndex];

    private readonly List<PlayerController> players = new List<PlayerController>();

    public void StartTurns(IReadOnlyList<PlayerController> activePlayers)
    {
        players.Clear();
        players.AddRange(activePlayers);
        CurrentRound = 1;
        CurrentTurnIndex = 0;
    }

    public bool AdvanceTurn()
    {
        if (players.Count == 0)
        {
            return false;
        }

        CurrentTurnIndex++;
        if (CurrentTurnIndex >= players.Count)
        {
            CurrentTurnIndex = 0;
            CurrentRound++;
            return true;
        }

        return false;
    }
}
