using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MinigameUIController : MonoBehaviour
{
    private readonly List<Text> scoreTexts = new List<Text>();
    private readonly List<MobileInputZone> mobileZones = new List<MobileInputZone>();
    private Font font;

    public TimerController Timer { get; private set; }
    public CountdownController Countdown { get; private set; }
    public ResultPanelController ResultPanel { get; private set; }
    public IReadOnlyList<MobileInputZone> MobileZones => mobileZones;

    public void Build(IReadOnlyList<PlayerMinigameData> players)
    {
        ClearChildren();
        EnsureEventSystem();
        font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        Canvas canvas = gameObject.GetComponent<Canvas>();
        if (canvas == null)
        {
            canvas = gameObject.AddComponent<Canvas>();
        }

        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        if (gameObject.GetComponent<GraphicRaycaster>() == null)
        {
            gameObject.AddComponent<GraphicRaycaster>();
        }

        CanvasScaler scaler = gameObject.GetComponent<CanvasScaler>();
        if (scaler == null)
        {
            scaler = gameObject.AddComponent<CanvasScaler>();
        }

        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        scaler.matchWidthOrHeight = 0.5f;

        BuildTimer(canvas.transform);
        BuildScores(canvas.transform, players);
        BuildCountdown(canvas.transform);
        BuildResultPanel(canvas.transform);
        BuildMobileZones(canvas.transform, players);
    }

    public void UpdateScores(IReadOnlyList<PlayerMinigameData> players)
    {
        for (int i = 0; i < scoreTexts.Count; i++)
        {
            if (players != null && i < players.Count)
            {
                scoreTexts[i].text = $"{players[i].PlayerName}  {players[i].Score}";
            }
        }
    }

    public void ShowResults(MinigameResult result)
    {
        ResultPanel?.Show(result);
    }

    private void BuildTimer(Transform parent)
    {
        GameObject panel = CreatePanel("Timer Panel", parent, new Vector2(0.5f, 1f), new Vector2(270f, 86f), new Vector2(0f, -62f), PartyArtPalette.PlayerYellow);
        Text text = CreateText("Timer Text", panel.transform, "20.0", 44, PartyArtPalette.DarkAccent);
        GameObject fillObject = new GameObject("Timer Fill", typeof(RectTransform), typeof(Image));
        fillObject.transform.SetParent(panel.transform, false);
        RectTransform fillRect = fillObject.GetComponent<RectTransform>();
        fillRect.anchorMin = new Vector2(0.08f, 0f);
        fillRect.anchorMax = new Vector2(0.92f, 0f);
        fillRect.sizeDelta = new Vector2(0f, 8f);
        fillRect.anchoredPosition = new Vector2(0f, 8f);
        Image fill = fillObject.GetComponent<Image>();
        fill.color = PartyArtPalette.PlayerGreen;
        fill.type = Image.Type.Filled;
        fill.fillMethod = Image.FillMethod.Horizontal;
        Timer = panel.AddComponent<TimerController>();
        Timer.Initialize(text, fill);
    }

    private void BuildScores(Transform parent, IReadOnlyList<PlayerMinigameData> players)
    {
        scoreTexts.Clear();
        for (int i = 0; i < 4; i++)
        {
            bool left = i < 2;
            Vector2 anchor = left ? new Vector2(0f, 1f) : new Vector2(1f, 1f);
            Vector2 offset = new Vector2(left ? 126f : -126f, -138f - (i % 2) * 84f);
            Color color = players != null && i < players.Count ? players[i].PlayerColor : PartyArtPalette.PlayerColor(i);
            GameObject panel = CreatePanel($"P{i + 1} Score", parent, anchor, new Vector2(198f, 70f), offset, color);
            scoreTexts.Add(CreateText("Score Text", panel.transform, $"P{i + 1}  0", 30, PartyArtPalette.DarkAccent));
        }
    }

    private void BuildCountdown(Transform parent)
    {
        GameObject panel = CreatePanel("Countdown Panel", parent, new Vector2(0.5f, 0.58f), new Vector2(150f, 150f), Vector2.zero, PartyArtPalette.WhiteAccent);
        CanvasGroup group = panel.AddComponent<CanvasGroup>();
        Text text = CreateText("Countdown Text", panel.transform, "3", 78, PartyArtPalette.DarkAccent);
        Countdown = panel.AddComponent<CountdownController>();
        Countdown.Initialize(text, group);
    }

    private void BuildResultPanel(Transform parent)
    {
        GameObject panel = CreatePanel("Result Panel", parent, new Vector2(0.5f, 0.5f), new Vector2(470f, 390f), new Vector2(0f, -40f), PartyArtPalette.UIPanel);
        CanvasGroup group = panel.AddComponent<CanvasGroup>();
        Text title = CreateText("Result Title", panel.transform, "Results", 44, PartyArtPalette.DarkAccent);
        RectTransform titleRect = title.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0f, 0.76f);
        titleRect.anchorMax = new Vector2(1f, 1f);

        List<Text> rows = new List<Text>();
        for (int i = 0; i < 4; i++)
        {
            Text row = CreateText($"Result Row {i + 1}", panel.transform, string.Empty, 32, PartyArtPalette.DarkAccent);
            RectTransform rowRect = row.GetComponent<RectTransform>();
            rowRect.anchorMin = new Vector2(0.08f, 0.56f - i * 0.15f);
            rowRect.anchorMax = new Vector2(0.92f, 0.70f - i * 0.15f);
            rows.Add(row);
        }

        ResultPanel = panel.AddComponent<ResultPanelController>();
        ResultPanel.Initialize(group, title, rows);
    }

    private void BuildMobileZones(Transform parent, IReadOnlyList<PlayerMinigameData> players)
    {
        mobileZones.Clear();
        for (int i = 0; i < 4; i++)
        {
            bool left = i < 2;
            Vector2 anchor = new Vector2(left ? 0f : 1f, 0f);
            Vector2 offset = new Vector2(left ? 132f : -132f, 72f + (i % 2) * 82f);
            Color color = players != null && i < players.Count ? players[i].PlayerColor : PartyArtPalette.PlayerColor(i);
            GameObject button = CreatePanel($"P{i + 1} Action Zone", parent, anchor, new Vector2(210f, 68f), offset, color);
            CreateText("Action Label", button.transform, $"P{i + 1} TAP", 27, PartyArtPalette.DarkAccent);
            MobileInputZone zone = button.AddComponent<MobileInputZone>();
            zone.Configure(i, true);
            mobileZones.Add(zone);
        }
    }

    private GameObject CreatePanel(string name, Transform parent, Vector2 anchor, Vector2 size, Vector2 offset, Color color)
    {
        GameObject panel = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Outline), typeof(Shadow));
        panel.transform.SetParent(parent, false);
        RectTransform rect = panel.GetComponent<RectTransform>();
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.sizeDelta = size;
        rect.anchoredPosition = offset;

        Image image = panel.GetComponent<Image>();
        image.color = color;
        Outline outline = panel.GetComponent<Outline>();
        outline.effectColor = new Color(0.12f, 0.16f, 0.20f, 0.18f);
        outline.effectDistance = new Vector2(3f, -3f);
        Shadow shadow = panel.GetComponent<Shadow>();
        shadow.effectColor = new Color(0f, 0f, 0f, 0.16f);
        shadow.effectDistance = new Vector2(0f, -5f);
        return panel;
    }

    private Text CreateText(string name, Transform parent, string value, int size, Color color)
    {
        GameObject textObject = new GameObject(name, typeof(RectTransform), typeof(Text), typeof(Shadow));
        textObject.transform.SetParent(parent, false);
        RectTransform rect = textObject.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = new Vector2(12f, 8f);
        rect.offsetMax = new Vector2(-12f, -8f);

        Text text = textObject.GetComponent<Text>();
        text.font = font;
        text.text = value;
        text.fontSize = size;
        text.fontStyle = FontStyle.Bold;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = color;
        text.raycastTarget = false;

        Shadow shadow = textObject.GetComponent<Shadow>();
        shadow.effectColor = new Color(1f, 1f, 1f, 0.35f);
        shadow.effectDistance = new Vector2(0f, 2f);
        return text;
    }

    private void ClearChildren()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }

    private static void EnsureEventSystem()
    {
        if (FindAnyObjectByType<EventSystem>() != null)
        {
            return;
        }

        GameObject eventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        DontDestroyOnLoad(eventSystem);
    }
}
