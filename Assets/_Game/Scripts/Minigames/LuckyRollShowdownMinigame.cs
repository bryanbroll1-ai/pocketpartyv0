using System.Collections.Generic;
using UnityEngine;

public class LuckyRollShowdownMinigame : BaseMinigame
{
    [SerializeField] private float rollSpeed = 13f;

    private readonly List<LuckyRollPlayerPanel> panels = new List<LuckyRollPlayerPanel>();
    private readonly HashSet<int> activePlayers = new HashSet<int>();
    private readonly HashSet<int> stoppedPlayers = new HashSet<int>();
    private bool tieBreakerActive;
    private bool tieBreakerUsed;

    public void RegisterPanels(IEnumerable<LuckyRollPlayerPanel> playerPanels)
    {
        panels.Clear();
        if (playerPanels == null)
        {
            return;
        }

        foreach (LuckyRollPlayerPanel panel in playerPanels)
        {
            if (panel != null)
            {
                panels.Add(panel);
            }
        }
    }

    protected override void OnGameStarted()
    {
        activePlayers.Clear();
        stoppedPlayers.Clear();
        tieBreakerActive = false;
        tieBreakerUsed = false;

        for (int i = 0; i < Players.Count; i++)
        {
            activePlayers.Add(Players[i].PlayerId);
            Scores.SetScore(Players[i].PlayerId, 0);
            GetPanel(Players[i].PlayerId)?.SetRolling();
        }
    }

    protected override void OnGameTick(float deltaTime)
    {
        for (int i = 0; i < Players.Count; i++)
        {
            PlayerMinigameData player = Players[i];
            LuckyRollPlayerPanel panel = GetPanel(player.PlayerId);
            if (panel == null)
            {
                continue;
            }

            if (activePlayers.Contains(player.PlayerId) && !stoppedPlayers.Contains(player.PlayerId))
            {
                panel.Display.TickRolling(deltaTime, rollSpeed + player.PlayerId * 1.4f + (tieBreakerActive ? 4f : 0f));
                PlayerInputState input = GetInput(player);
                player.Controller?.ApplyInput(input, deltaTime);
                if (input.ActionDown || input.TapDown)
                {
                    StopPlayer(player, panel);
                }
            }
        }

        if (AllActivePlayersStopped())
        {
            ResolveRoundOrStartTieBreaker();
        }
    }

    protected override void OnGameEnded()
    {
        for (int i = 0; i < Players.Count; i++)
        {
            PlayerMinigameData player = Players[i];
            if (activePlayers.Contains(player.PlayerId) && !stoppedPlayers.Contains(player.PlayerId))
            {
                LuckyRollPlayerPanel panel = GetPanel(player.PlayerId);
                if (panel != null)
                {
                    StopPlayer(player, panel);
                }
            }
        }

        if (!tieBreakerUsed && TieBreakerHandler.HasHighestTie(Players))
        {
            List<PlayerMinigameData> tied = TieBreakerHandler.FindHighestTie(Players);
            for (int i = 0; i < tied.Count; i++)
            {
                int fallbackScore = 6 - i;
                Scores.SetScore(tied[i].PlayerId, fallbackScore);
                GetPanel(tied[i].PlayerId)?.SetStopped(fallbackScore);
            }
        }
    }

    protected override MinigameResult BuildResult()
    {
        List<PlayerMinigameData> ranked = TieBreakerHandler.RankFinal(Players);
        MinigameResult result = new MinigameResult("Lucky Roll Showdown");
        result.SetPlayerResults(ranked);
        return result;
    }

    private void StopPlayer(PlayerMinigameData player, LuckyRollPlayerPanel panel)
    {
        int value = panel.Display.Stop();
        Scores.SetScore(player.PlayerId, value);
        stoppedPlayers.Add(player.PlayerId);
        panel.SetStopped(value);
    }

    private void ResolveRoundOrStartTieBreaker()
    {
        if (!tieBreakerUsed && TieBreakerHandler.HasHighestTie(Players))
        {
            StartTieBreaker(TieBreakerHandler.FindHighestTie(Players));
            return;
        }

        RequestEnd();
    }

    private void StartTieBreaker(IReadOnlyList<PlayerMinigameData> tiedPlayers)
    {
        tieBreakerActive = true;
        tieBreakerUsed = true;
        activePlayers.Clear();
        stoppedPlayers.Clear();

        for (int i = 0; i < tiedPlayers.Count; i++)
        {
            PlayerMinigameData player = tiedPlayers[i];
            activePlayers.Add(player.PlayerId);
            Scores.SetScore(player.PlayerId, 0);
            GetPanel(player.PlayerId)?.SetTieBreaker();
        }
    }

    private bool AllActivePlayersStopped()
    {
        if (activePlayers.Count == 0)
        {
            return false;
        }

        foreach (int playerId in activePlayers)
        {
            if (!stoppedPlayers.Contains(playerId))
            {
                return false;
            }
        }

        return true;
    }

    private LuckyRollPlayerPanel GetPanel(int playerId)
    {
        return playerId >= 0 && playerId < panels.Count ? panels[playerId] : null;
    }
}
