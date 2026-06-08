using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultPanelController : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Text titleText;
    [SerializeField] private readonly List<Text> rows = new List<Text>();

    public void Initialize(CanvasGroup group, Text title, IReadOnlyList<Text> rowTexts)
    {
        canvasGroup = group;
        titleText = title;
        rows.Clear();
        rows.AddRange(rowTexts);
        Hide();
    }

    public void Show(MinigameResult result)
    {
        if (titleText != null)
        {
            titleText.text = result != null ? result.Title : "Results";
        }

        IReadOnlyList<PlayerMinigameData> players = result?.PlayerResults;
        for (int i = 0; i < rows.Count; i++)
        {
            if (players != null && i < players.Count)
            {
                PlayerMinigameData player = players[i];
                rows[i].text = $"{player.Rank}. {player.PlayerName}   {player.Score}";
                rows[i].color = player.PlayerColor;
            }
            else
            {
                rows[i].text = string.Empty;
            }
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
        }
    }

    public void Hide()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
        }
    }
}
