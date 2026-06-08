using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorRushMinigame : MinigameBase
{
    private readonly List<ColorOption> options = new List<ColorOption>
    {
        new ColorOption("Red", new Color(0.90f, 0.18f, 0.20f)),
        new ColorOption("Green", new Color(0.20f, 0.72f, 0.34f)),
        new ColorOption("Blue", new Color(0.18f, 0.44f, 0.92f)),
        new ColorOption("Yellow", new Color(0.95f, 0.79f, 0.20f))
    };

    private int round;
    private int humanScore;
    private float roundStartTime;
    private ColorOption target;

    public override void StartMinigame(IReadOnlyList<PlayerController> players, UIManager ui, System.Action<MinigameResult> onComplete)
    {
        base.StartMinigame(players, ui, onComplete);
        RuntimeMinigameArena.Show(MinigameArenaStyle.ScooterLine, players);
        StartRound();
    }

    private void StartRound()
    {
        round++;
        target = options[Random.Range(0, options.Count)];
        roundStartTime = Time.unscaledTime;

        ContentRoot = UI.ShowMinigameScreen("Color Rush", $"Round {round}/5: tap {target.Name}", MinigameTheme.ColorPop);
        var splashPanel = UI.CreatePanel(ContentRoot, "Color Splash Panel", new Color(1f, 1f, 1f, 0.08f), 170f);
        UI.CreateLabel(splashPanel, target.Name.ToUpper(), 74, TextAnchor.MiddleCenter).color = target.Color;
        var row = UI.CreateRow(ContentRoot);
        foreach (var option in ShuffledOptions())
        {
            Button button = UI.CreateButton(row, option.Name, () => SelectColor(option));
            button.GetComponent<Image>().color = option.Color;
        }
    }

    private void SelectColor(ColorOption option)
    {
        if (!IsRunning)
        {
            return;
        }

        if (option.Name == target.Name)
        {
            float responseTime = Time.unscaledTime - roundStartTime;
            humanScore += Mathf.RoundToInt(Mathf.Max(100f, 1000f - responseTime * 260f));
        }
        else
        {
            humanScore -= 150;
        }

        if (round >= 5)
        {
            Complete();
        }
        else
        {
            StartRound();
        }
    }

    private void Complete()
    {
        var result = new MinigameResult("Color Rush Results");
        foreach (var player in Players)
        {
            result.Scores[player] = player.IsBot ? Random.Range(2300, 4300) : humanScore;
            result.DisplayLines.Add($"{player.PlayerName}: {result.Scores[player]:0} pts");
        }

        result.RankHighScoreWins();
        Finish(result);
    }

    private List<ColorOption> ShuffledOptions()
    {
        var shuffled = new List<ColorOption>(options);
        for (int i = 0; i < shuffled.Count; i++)
        {
            int swapIndex = Random.Range(i, shuffled.Count);
            ColorOption temp = shuffled[i];
            shuffled[i] = shuffled[swapIndex];
            shuffled[swapIndex] = temp;
        }

        return shuffled;
    }

    private struct ColorOption
    {
        public string Name;
        public Color Color;

        public ColorOption(string name, Color color)
        {
            Name = name;
            Color = color;
        }
    }
}
