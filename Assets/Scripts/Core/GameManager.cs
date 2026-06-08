using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private readonly List<PlayerController> players = new List<PlayerController>();
    private readonly string[] playerNames = { "Bloop", "Nomi", "Taro", "Pippa" };
    private readonly Color[] playerColors =
    {
        new Color(0.24f, 0.63f, 0.95f),
        new Color(0.96f, 0.45f, 0.62f),
        new Color(0.34f, 0.78f, 0.48f),
        new Color(0.98f, 0.75f, 0.28f)
    };

    private PerformanceSettings performanceSettings;
    private UIManager uiManager;
    private BoardManager boardManager;
    private TurnManager turnManager;
    private DiceRoller diceRoller;
    private CoinManager coinManager;
    private MinigameManager minigameManager;
    private AudioManager audioManager;
    private ResultsPanel resultsPanel;
    private CameraFollowController cameraFollow;

    private int selectedPlayerCount = 4;
    private int totalRounds = 8;
    private int turnsSinceMinigame;
    private bool waitingForRoll;
    private string turnStatus;

    public void Initialize(
        PerformanceSettings performance,
        UIManager ui,
        BoardManager board,
        TurnManager turns,
        DiceRoller dice,
        CoinManager coins,
        MinigameManager minigames,
        AudioManager audio,
        ResultsPanel results,
        CameraFollowController cameraController)
    {
        performanceSettings = performance;
        uiManager = ui;
        boardManager = board;
        turnManager = turns;
        diceRoller = dice;
        coinManager = coins;
        minigameManager = minigames;
        audioManager = audio;
        resultsPanel = results;
        cameraFollow = cameraController;

        ShowMainMenu();
    }

    public void ShowMainMenu()
    {
        ClearPlayerObjects();
        if (cameraFollow != null)
        {
            cameraFollow.ShowBoardOverview();
        }

        uiManager.ShowMainMenu(ShowSetup, ShowSettings, QuitGame, StartFrameworkTest);
    }

    private void ShowSetup()
    {
        uiManager.ShowSetup(selectedPlayerCount, totalRounds, StartGame, ShowMainMenu);
    }

    private void ShowSettings()
    {
        uiManager.ShowSettings(performanceSettings, ShowMainMenu);
    }

    private void QuitGame()
    {
        Application.Quit();
    }

    private void StartGame(int playerCount, int rounds)
    {
        selectedPlayerCount = playerCount;
        totalRounds = rounds;
        turnsSinceMinigame = 0;
        CreatePlayers(selectedPlayerCount);
        boardManager.BuildBoard(players);
        turnManager.StartTurns(players);
        cameraFollow.ShowBoardOverview();
        uiManager.ShowBoardHud(RollDice);
        BeginTurn();
    }

    private void StartFrameworkTest()
    {
        CreatePlayers(4);
        audioManager.PlayMinigameStart();
        minigameManager.StartMinigame(MinigameType.FrameworkDummy, players, uiManager, OnFrameworkTestComplete);
    }

    private void OnFrameworkTestComplete(MinigameResult result)
    {
        uiManager.ShowMinigameResults(result, ShowMainMenu);
    }

    private void CreatePlayers(int count)
    {
        ClearPlayerObjects();
        var parent = new GameObject("Players").transform;

        for (int i = 0; i < count; i++)
        {
            var playerObject = new GameObject(playerNames[i]);
            playerObject.transform.SetParent(parent);
            var player = playerObject.AddComponent<PlayerController>();
            player.Initialize(i, playerNames[i], playerColors[i], i != 0);
            if (player.IsBot)
            {
                playerObject.AddComponent<BotController>();
            }

            players.Add(player);
        }
    }

    private void ClearPlayerObjects()
    {
        foreach (var player in players)
        {
            if (player != null)
            {
                Destroy(player.gameObject);
            }
        }

        players.Clear();
        var oldParent = GameObject.Find("Players");
        if (oldParent != null)
        {
            Destroy(oldParent);
        }
    }

    private void BeginTurn()
    {
        if (turnManager.CurrentRound > totalRounds)
        {
            ShowFinalResults();
            return;
        }

        waitingForRoll = true;
        PlayerController currentPlayer = turnManager.CurrentPlayer;
        boardManager.SetActivePlayer(currentPlayer);
        cameraFollow.SetTarget(boardManager.GetMarkerTransform(currentPlayer));
        turnStatus = currentPlayer.IsBot ? $"{currentPlayer.PlayerName} is thinking..." : "Tap Roll Dice.";
        UpdateHud();

        if (currentPlayer.IsBot)
        {
            StartCoroutine(BotRollRoutine());
        }
    }

    private IEnumerator BotRollRoutine()
    {
        yield return new WaitForSeconds(0.75f);
        RollDice();
    }

    private void RollDice()
    {
        if (!waitingForRoll)
        {
            return;
        }

        waitingForRoll = false;
        audioManager.PlayButton();
        int roll = diceRoller.Roll();
        PlayerController currentPlayer = turnManager.CurrentPlayer;
        turnStatus = $"{currentPlayer.PlayerName} rolled {roll}.";
        UpdateHud();
        StartCoroutine(ResolveTurnRoutine(currentPlayer, roll));
    }

    private IEnumerator ResolveTurnRoutine(PlayerController player, int roll)
    {
        yield return new WaitForSeconds(0.25f);
        yield return boardManager.MovePlayer(player, roll);
        BoardTile tile = boardManager.GetTileForPlayer(player);
        bool startMinigame = ApplyTileEffect(player, tile);
        turnsSinceMinigame++;
        UpdateHud();

        if (!startMinigame && turnsSinceMinigame >= players.Count * 2)
        {
            startMinigame = true;
        }

        if (startMinigame)
        {
            turnsSinceMinigame = 0;
            yield return new WaitForSeconds(0.7f);
            StartMinigame();
        }
        else
        {
            yield return new WaitForSeconds(0.7f);
            FinishTurn();
        }
    }

    private bool ApplyTileEffect(PlayerController player, BoardTile tile)
    {
        if (tile == null)
        {
            return false;
        }

        switch (tile.TileType)
        {
            case BoardTileType.CoinPlus:
                coinManager.AddCoins(player, 3);
                audioManager.PlayCoin();
                turnStatus = $"{player.PlayerName} gained 3 coins.";
                return false;
            case BoardTileType.CoinMinus:
                coinManager.AddCoins(player, -2);
                turnStatus = $"{player.PlayerName} lost 2 coins.";
                return false;
            case BoardTileType.Event:
                int swing = Random.value > 0.5f ? 5 : -3;
                coinManager.AddCoins(player, swing);
                turnStatus = swing > 0 ? $"{player.PlayerName} found {swing} coins." : $"{player.PlayerName} hit a coin tax.";
                return false;
            case BoardTileType.Minigame:
                turnStatus = "Minigame tile!";
                return true;
            default:
                turnStatus = "Nothing happened.";
                return false;
        }
    }

    private void StartMinigame()
    {
        audioManager.PlayMinigameStart();
        minigameManager.StartNextMinigame(players, uiManager, OnMinigameComplete);
    }

    private void OnMinigameComplete(MinigameResult result)
    {
        coinManager.AwardPlacementCoins(result);
        result.DisplayLines.Insert(0, "Awards: 10 / 6 / 3 / 1 coins");
        uiManager.ShowMinigameResults(result, () =>
        {
            uiManager.ShowBoardHud(RollDice);
            FinishTurn();
        });
    }

    private void FinishTurn()
    {
        turnManager.AdvanceTurn();
        BeginTurn();
    }

    private void ShowFinalResults()
    {
        var rankedPlayers = players.OrderByDescending(player => player.Coins).ToList();
        resultsPanel.Show(uiManager, rankedPlayers, ShowMainMenu);
    }

    private void UpdateHud()
    {
        uiManager.UpdateBoardHud(players, turnManager.CurrentPlayer, turnManager.CurrentRound, totalRounds, turnStatus, waitingForRoll);
    }
}
