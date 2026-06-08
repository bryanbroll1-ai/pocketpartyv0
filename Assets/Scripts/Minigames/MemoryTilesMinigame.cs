using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MemoryTilesMinigame : MinigameBase
{
    private readonly List<Button> buttons = new List<Button>();
    private readonly List<int> sequence = new List<int>();
    private int inputIndex;
    private int correctInputs;
    private float inputStartTime;
    private bool acceptingInput;

    public override void StartMinigame(IReadOnlyList<PlayerController> players, UIManager ui, System.Action<MinigameResult> onComplete)
    {
        base.StartMinigame(players, ui, onComplete);
        RuntimeMinigameArena.Show(MinigameArenaStyle.StageKitchenFarm, players);
        ContentRoot = UI.ShowMinigameScreen("Memory Tiles", "Watch the glow pattern, then repeat it.", MinigameTheme.MemoryGlow);
        CreateGrid();
        BuildSequence();
        StartCoroutine(ShowSequenceRoutine());
    }

    private void CreateGrid()
    {
        buttons.Clear();
        for (int rowIndex = 0; rowIndex < 2; rowIndex++)
        {
            var row = UI.CreateRow(ContentRoot);
            for (int column = 0; column < 3; column++)
            {
                int tileIndex = rowIndex * 3 + column;
                Button button = UI.CreateButton(row, (tileIndex + 1).ToString(), () => SelectTile(tileIndex));
                button.GetComponent<Image>().color = GetTileColor(tileIndex);
                buttons.Add(button);
            }
        }
    }

    private void BuildSequence()
    {
        int length = Random.Range(3, 7);
        for (int i = 0; i < length; i++)
        {
            sequence.Add(Random.Range(0, buttons.Count));
        }
    }

    private IEnumerator ShowSequenceRoutine()
    {
        acceptingInput = false;
        yield return new WaitForSeconds(0.7f);
        foreach (int index in sequence)
        {
            var image = buttons[index].GetComponent<Image>();
            image.color = new Color(0.97f, 0.74f, 0.24f);
            yield return new WaitForSeconds(0.42f);
            image.color = GetTileColor(index);
            yield return new WaitForSeconds(0.18f);
        }

        inputStartTime = Time.unscaledTime;
        acceptingInput = true;
    }

    private void SelectTile(int tileIndex)
    {
        if (!IsRunning || !acceptingInput)
        {
            return;
        }

        if (sequence[inputIndex] == tileIndex)
        {
            correctInputs++;
            inputIndex++;
            if (inputIndex >= sequence.Count)
            {
                Complete();
            }
        }
        else
        {
            Complete();
        }
    }

    private void Complete()
    {
        float speedBonus = Mathf.Max(0f, 600f - (Time.unscaledTime - inputStartTime) * 80f);
        var result = new MinigameResult("Memory Tiles Results");
        foreach (var player in Players)
        {
            float score = player.IsBot ? Random.Range(2, sequence.Count + 1) * 220f : correctInputs * 250f + speedBonus;
            result.Scores[player] = score;
            result.DisplayLines.Add($"{player.PlayerName}: {score:0} pts");
        }

        result.RankHighScoreWins();
        Finish(result);
    }

    private Color GetTileColor(int index)
    {
        Color[] colors =
        {
            new Color(0.18f, 0.52f, 0.86f),
            new Color(0.42f, 0.34f, 0.88f),
            new Color(0.18f, 0.72f, 0.58f),
            new Color(0.92f, 0.44f, 0.58f),
            new Color(0.92f, 0.68f, 0.24f),
            new Color(0.32f, 0.76f, 0.92f)
        };

        return colors[index % colors.Length];
    }
}
