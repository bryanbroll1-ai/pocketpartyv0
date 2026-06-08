using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TapTargetsMinigame : MinigameBase
{
    private const float Duration = 15f;
    private RectTransform playArea;
    private Button targetButton;
    private Text status;
    private float endTime;
    private int hits;
    private int penalties;

    public override void StartMinigame(IReadOnlyList<PlayerController> players, UIManager ui, System.Action<MinigameResult> onComplete)
    {
        base.StartMinigame(players, ui, onComplete);
        RuntimeMinigameArena.Show(MinigameArenaStyle.NeonSplit, players);
        endTime = Time.unscaledTime + Duration;
        ContentRoot = UI.ShowMinigameScreen("Tap Targets", "Pop bright targets before the clock runs out.", MinigameTheme.TargetField);
        status = UI.CreateLabel(ContentRoot, "", 40, TextAnchor.MiddleCenter);
        playArea = CreatePlayArea(ContentRoot);
        SpawnTarget();
    }

    private void Update()
    {
        if (!IsRunning)
        {
            return;
        }

        if (Time.unscaledTime >= endTime)
        {
            Complete();
            return;
        }

        status.text = $"Hits: {hits}  Misses: {penalties}\nTime: {Mathf.Max(0f, endTime - Time.unscaledTime):0.0}s";
    }

    private RectTransform CreatePlayArea(RectTransform parent)
    {
        var areaObject = new GameObject("Target Area", typeof(RectTransform), typeof(Image), typeof(LayoutElement));
        areaObject.transform.SetParent(parent, false);
        var image = areaObject.GetComponent<Image>();
        image.color = new Color(0.06f, 0.03f, 0.05f, 0.44f);
        var missButton = areaObject.AddComponent<Button>();
        missButton.transition = Selectable.Transition.None;
        missButton.onClick.AddListener(() =>
        {
            if (IsRunning)
            {
                penalties++;
            }
        });
        var element = areaObject.GetComponent<LayoutElement>();
        element.preferredHeight = 520;
        element.flexibleHeight = 0f;
        var rect = areaObject.GetComponent<RectTransform>();
        UI.CreateDecorativeCircle(rect, "Target Field Accent A", new Color(0.93f, 0.35f, 0.35f, 0.24f), new Vector2(0.14f, 0.82f), new Vector2(260f, 260f), Vector2.zero);
        UI.CreateDecorativeCircle(rect, "Target Field Accent B", new Color(0.98f, 0.75f, 0.28f, 0.18f), new Vector2(0.86f, 0.20f), new Vector2(210f, 210f), Vector2.zero);
        return areaObject.GetComponent<RectTransform>();
    }

    private void SpawnTarget()
    {
        if (targetButton != null)
        {
            Destroy(targetButton.gameObject);
        }

        targetButton = UI.CreateButton(playArea, "Tap", OnTargetTapped);
        targetButton.GetComponent<Image>().color = new Color(Random.Range(0.78f, 1f), Random.Range(0.28f, 0.72f), Random.Range(0.24f, 0.58f));
        var rect = targetButton.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(190, 190);
        rect.anchoredPosition = new Vector2(Random.Range(-330f, 330f), Random.Range(-170f, 170f));
    }

    private void OnTargetTapped()
    {
        if (!IsRunning)
        {
            return;
        }

        hits++;
        SpawnTarget();
    }

    private void Complete()
    {
        var result = new MinigameResult("Tap Targets Results");
        foreach (var player in Players)
        {
            float score = player.IsBot ? Random.Range(8, 22) : hits - penalties;
            result.Scores[player] = score;
            result.DisplayLines.Add($"{player.PlayerName}: {score:0} pts");
        }

        result.RankHighScoreWins();
        Finish(result);
    }
}
