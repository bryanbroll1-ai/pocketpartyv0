using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static readonly Color ScreenInk = new Color(0.045f, 0.055f, 0.070f, 1f);
    private static readonly Color PanelInk = new Color(0.085f, 0.10f, 0.13f, 0.90f);
    private static readonly Color PanelStroke = new Color(1f, 1f, 1f, 0.18f);
    private static readonly Color ButtonInk = new Color(0.13f, 0.20f, 0.27f, 0.98f);
    private static readonly Color ButtonAccent = new Color(1.00f, 0.70f, 0.22f, 1f);

    public Canvas Canvas { get; private set; }

    private RectTransform root;
    private Font defaultFont;
    private Sprite circleSprite;
    private RectTransform currentScreenRoot;
    private Text boardHudText;
    private Button diceButton;
    private Text statusText;

    public void Initialize()
    {
        defaultFont = LoadBuiltinFont();
        circleSprite = RuntimeVisuals.CreateCircleSprite();
        Canvas = FindAnyObjectByType<Canvas>();
        if (Canvas == null)
        {
            var canvasObject = new GameObject("Canvas");
            Canvas = canvasObject.AddComponent<Canvas>();
            Canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObject.AddComponent<GraphicRaycaster>();
            var scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            scaler.matchWidthOrHeight = 0.5f;
        }

        root = Canvas.GetComponent<RectTransform>();
    }

    private Font LoadBuiltinFont()
    {
        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (font == null)
        {
            Debug.LogError("Could not load Unity built-in font LegacyRuntime.ttf.");
        }

        return font;
    }

    public void ShowMainMenu(Action onPlay, Action onSettings, Action onQuit, Action onTestMinigame = null)
    {
        RuntimeMinigameArena.Clear();
        Clear();
        var panel = CreateScreen("Main Menu", ScreenInk);
        AddScreenDecorations(currentScreenRoot, new Color(0.18f, 0.56f, 0.82f, 0.20f), new Color(0.97f, 0.69f, 0.26f, 0.15f));
        CreateTitle(panel, "Pocket Party");
        CreateLabel(panel, "Local party board prototype", 40, TextAnchor.MiddleCenter);
        CreateButton(panel, "Play", onPlay);
        if (onTestMinigame != null)
        {
            CreateButton(panel, "Test Minigame", onTestMinigame);
        }

        CreateButton(panel, "Settings", onSettings);
        CreateButton(panel, "Quit", onQuit);
    }

    public void ShowSetup(int playerCount, int roundCount, Action<int, int> onStart, Action onBack)
    {
        RuntimeMinigameArena.Clear();
        Clear();
        int selectedPlayers = playerCount;
        int selectedRounds = roundCount;

        var panel = CreateScreen("Setup Screen", new Color(0.050f, 0.064f, 0.078f));
        AddScreenDecorations(currentScreenRoot, new Color(0.34f, 0.78f, 0.48f, 0.18f), new Color(0.96f, 0.45f, 0.62f, 0.16f));
        CreateTitle(panel, "Game Setup");
        var playersLabel = CreateLabel(panel, "", 46, TextAnchor.MiddleCenter);
        var roundsLabel = CreateLabel(panel, "", 46, TextAnchor.MiddleCenter);

        void Refresh()
        {
            playersLabel.text = $"Players: {selectedPlayers}";
            roundsLabel.text = $"Rounds: {selectedRounds}";
        }

        var playerRow = CreateRow(panel);
        CreateButton(playerRow, "-", () =>
        {
            selectedPlayers = Mathf.Max(2, selectedPlayers - 1);
            Refresh();
        });
        CreateButton(playerRow, "+", () =>
        {
            selectedPlayers = Mathf.Min(4, selectedPlayers + 1);
            Refresh();
        });

        var roundRow = CreateRow(panel);
        CreateButton(roundRow, "- Round", () =>
        {
            selectedRounds = Mathf.Max(3, selectedRounds - 1);
            Refresh();
        });
        CreateButton(roundRow, "+ Round", () =>
        {
            selectedRounds = Mathf.Min(20, selectedRounds + 1);
            Refresh();
        });

        CreateButton(panel, "Start", () => onStart?.Invoke(selectedPlayers, selectedRounds));
        CreateButton(panel, "Back", onBack);
        Refresh();
    }

    public void ShowSettings(PerformanceSettings settings, Action onBack)
    {
        RuntimeMinigameArena.Clear();
        Clear();
        var panel = CreateScreen("Settings Screen", new Color(0.047f, 0.064f, 0.067f));
        AddScreenDecorations(currentScreenRoot, new Color(0.40f, 0.82f, 0.88f, 0.18f), new Color(0.98f, 0.75f, 0.28f, 0.16f));
        CreateTitle(panel, "Settings");
        var status = CreateLabel(panel, "", 42, TextAnchor.MiddleCenter);

        void Refresh()
        {
            status.text = $"FPS: {settings.TargetFrameRate}\nBattery Saver: {(settings.BatterySaver ? "On" : "Off")}";
        }

        CreateButton(panel, "30 FPS Saver", () =>
        {
            settings.SetBatterySaver(true);
            Refresh();
        });
        CreateButton(panel, "60 FPS", () =>
        {
            settings.SetTargetFrameRate(60);
            Refresh();
        });
        CreateButton(panel, "Back", onBack);
        Refresh();
    }

    public void ShowBoardHud(Action onRoll)
    {
        RuntimeMinigameArena.Clear();
        Clear();
        var hud = CreateScreen("Board HUD", new Color(0f, 0f, 0f, 0f));
        var hudLayout = hud.GetComponent<VerticalLayoutGroup>();
        hudLayout.childAlignment = TextAnchor.UpperCenter;
        hudLayout.padding = new RectOffset(32, 32, 40, 32);

        var topPanel = CreatePanel(hud, "Board Status Card", new Color(0.035f, 0.045f, 0.060f, 0.76f), 300f);
        boardHudText = CreateLabel(topPanel, "", 34, TextAnchor.MiddleCenter);
        boardHudText.color = Color.white;
        statusText = CreateLabel(topPanel, "", 36, TextAnchor.MiddleCenter);
        statusText.fontStyle = FontStyle.Bold;
        statusText.color = Color.white;

        var spacer = new GameObject("Flexible Space", typeof(RectTransform), typeof(LayoutElement));
        spacer.transform.SetParent(hud, false);
        spacer.GetComponent<LayoutElement>().flexibleHeight = 1f;

        diceButton = CreateButton(hud, "Roll Dice", onRoll);
    }

    public void UpdateBoardHud(IReadOnlyList<PlayerController> players, PlayerController currentPlayer, int round, int totalRounds, string status, bool canRoll)
    {
        if (boardHudText == null)
        {
            return;
        }

        var builder = new StringBuilder();
        builder.AppendLine($"Round {round}/{totalRounds}  |  Turn: {currentPlayer.PlayerName}");
        foreach (var player in players)
        {
            builder.Append($"{player.PlayerName}: {player.Coins}");
            if (player == currentPlayer)
            {
                builder.Append("  active");
            }

            builder.Append("   ");
        }

        boardHudText.text = builder.ToString();
        statusText.text = status;
        diceButton.interactable = canRoll && !currentPlayer.IsBot;
    }

    public RectTransform ShowMinigameScreen(string title, string subtitle)
    {
        return ShowMinigameScreen(title, subtitle, MinigameTheme.Default);
    }

    public RectTransform ShowMinigameScreen(string title, string subtitle, MinigameTheme theme)
    {
        Clear();
        Color background = GetThemeBackground(theme);
        background.a = 0.12f;
        var panel = CreateScreen($"{title} Screen", background);
        AddMinigameDecorations(currentScreenRoot, theme);
        CreateTitle(panel, title);
        if (!string.IsNullOrEmpty(subtitle))
        {
            CreateLabel(panel, subtitle, 38, TextAnchor.MiddleCenter);
        }

        AddFlexibleSpace(panel);
        return panel;
    }

    public void ShowMinigameResults(MinigameResult result, Action onContinue)
    {
        Clear();
        var panel = CreateScreen("Minigame Results", new Color(0.050f, 0.055f, 0.072f));
        AddScreenDecorations(currentScreenRoot, new Color(0.98f, 0.75f, 0.28f, 0.18f), new Color(0.35f, 0.64f, 0.95f, 0.18f));
        CreateTitle(panel, result.Title);
        foreach (var line in result.DisplayLines)
        {
            CreateLabel(panel, line, 36, TextAnchor.MiddleCenter);
        }

        CreateButton(panel, "Continue", onContinue);
    }

    public void ShowFinalResults(IReadOnlyList<PlayerController> players, Action onMainMenu)
    {
        RuntimeMinigameArena.Clear();
        Clear();
        var panel = CreateScreen("Final Results", new Color(0.047f, 0.064f, 0.082f));
        AddScreenDecorations(currentScreenRoot, new Color(0.34f, 0.78f, 0.48f, 0.20f), new Color(0.98f, 0.75f, 0.28f, 0.18f));
        CreateTitle(panel, "Results");
        for (int i = 0; i < players.Count; i++)
        {
            CreateLabel(panel, $"{i + 1}. {players[i].PlayerName} - {players[i].Coins} coins", 44, TextAnchor.MiddleCenter);
        }

        CreateButton(panel, "Main Menu", onMainMenu);
    }

    public void Clear()
    {
        if (root == null)
        {
            return;
        }

        for (int i = root.childCount - 1; i >= 0; i--)
        {
            Destroy(root.GetChild(i).gameObject);
        }
    }

    public void PulseDiceButton()
    {
        if (diceButton != null)
        {
            PartyJuice.PopScale(diceButton.transform, 0.18f, 0.28f);
        }
    }

    public RectTransform CreateRow(RectTransform parent)
    {
        var row = new GameObject("Row", typeof(RectTransform), typeof(HorizontalLayoutGroup), typeof(LayoutElement));
        row.transform.SetParent(parent, false);
        var layout = row.GetComponent<HorizontalLayoutGroup>();
        layout.spacing = 20;
        layout.childControlWidth = true;
        layout.childForceExpandWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandHeight = false;

        var element = row.GetComponent<LayoutElement>();
        element.preferredHeight = 120;
        return row.GetComponent<RectTransform>();
    }

    public void AddFlexibleSpace(RectTransform parent)
    {
        var spacer = new GameObject("Flexible Space", typeof(RectTransform), typeof(LayoutElement));
        spacer.transform.SetParent(parent, false);
        spacer.GetComponent<LayoutElement>().flexibleHeight = 1f;
    }

    public RectTransform CreatePanel(RectTransform parent, string name, Color color, float preferredHeight)
    {
        var panelObject = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Outline), typeof(Shadow), typeof(VerticalLayoutGroup), typeof(LayoutElement));
        panelObject.transform.SetParent(parent, false);

        var image = panelObject.GetComponent<Image>();
        image.color = color.a > 0f ? color : PanelInk;
        image.sprite = RuntimeVisuals.GetRoundedRectSprite();
        image.type = Image.Type.Sliced;

        var outline = panelObject.GetComponent<Outline>();
        outline.effectColor = PanelStroke;
        outline.effectDistance = new Vector2(2f, -2f);

        var shadow = panelObject.GetComponent<Shadow>();
        shadow.effectColor = new Color(0f, 0f, 0f, 0.24f);
        shadow.effectDistance = new Vector2(0f, -8f);

        var layout = panelObject.GetComponent<VerticalLayoutGroup>();
        layout.padding = new RectOffset(30, 30, 24, 24);
        layout.spacing = 16;
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childControlWidth = true;
        layout.childForceExpandWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandHeight = false;

        var element = panelObject.GetComponent<LayoutElement>();
        element.preferredHeight = preferredHeight;
        return panelObject.GetComponent<RectTransform>();
    }

    public Image CreateProgressBar(RectTransform parent, string name, Color backgroundColor, Color fillColor, float preferredHeight)
    {
        var barObject = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(LayoutElement));
        barObject.transform.SetParent(parent, false);
        barObject.GetComponent<Image>().color = backgroundColor;
        var element = barObject.GetComponent<LayoutElement>();
        element.preferredHeight = preferredHeight;

        var fillObject = new GameObject("Fill", typeof(RectTransform), typeof(Image));
        fillObject.transform.SetParent(barObject.transform, false);
        var fillRect = fillObject.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = new Vector2(10f, 10f);
        fillRect.offsetMax = new Vector2(-10f, -10f);

        var fillImage = fillObject.GetComponent<Image>();
        fillImage.color = fillColor;
        fillImage.type = Image.Type.Filled;
        fillImage.fillMethod = Image.FillMethod.Horizontal;
        fillImage.fillAmount = 0f;
        return fillImage;
    }

    public Image CreateDecorativeCircle(RectTransform parent, string name, Color color, Vector2 anchor, Vector2 size, Vector2 offset)
    {
        Image circle = RuntimeVisuals.AddCircle(parent, name, color, anchor, size, offset, circleSprite);
        circle.transform.SetSiblingIndex(0);
        return circle;
    }

    public Button CreateButton(RectTransform parent, string label, Action onClick)
    {
        var buttonObject = new GameObject(label, typeof(RectTransform), typeof(Image), typeof(Outline), typeof(Shadow), typeof(Button), typeof(LayoutElement));
        buttonObject.transform.SetParent(parent, false);
        var image = buttonObject.GetComponent<Image>();
        image.color = ButtonInk;
        image.sprite = RuntimeVisuals.GetRoundedRectSprite();
        image.type = Image.Type.Sliced;

        var element = buttonObject.GetComponent<LayoutElement>();
        element.preferredHeight = 112;
        element.minHeight = 92;

        var outline = buttonObject.GetComponent<Outline>();
        outline.effectColor = new Color(1f, 1f, 1f, 0.13f);
        outline.effectDistance = new Vector2(2f, -2f);

        var shadow = buttonObject.GetComponent<Shadow>();
        shadow.effectColor = new Color(0f, 0f, 0f, 0.28f);
        shadow.effectDistance = new Vector2(0f, -6f);

        var button = buttonObject.GetComponent<Button>();
        button.onClick.AddListener(new UnityAction(() => onClick?.Invoke()));
        var colors = button.colors;
        colors.normalColor = ButtonInk;
        colors.highlightedColor = new Color(0.16f, 0.26f, 0.32f, 1f);
        colors.pressedColor = new Color(0.06f, 0.11f, 0.15f, 1f);
        colors.disabledColor = new Color(0.12f, 0.14f, 0.16f, 0.70f);
        button.colors = colors;

        var accentObject = new GameObject("Accent", typeof(RectTransform), typeof(Image));
        accentObject.transform.SetParent(buttonObject.transform, false);
        var accentRect = accentObject.GetComponent<RectTransform>();
        accentRect.anchorMin = new Vector2(0f, 0f);
        accentRect.anchorMax = new Vector2(0f, 1f);
        accentRect.pivot = new Vector2(0f, 0.5f);
        accentRect.sizeDelta = new Vector2(12f, -34f);
        accentRect.anchoredPosition = new Vector2(14f, 0f);
        var accentImage = accentObject.GetComponent<Image>();
        accentImage.color = ButtonAccent;
        accentImage.sprite = RuntimeVisuals.GetRoundedRectSprite();
        accentImage.type = Image.Type.Sliced;
        accentImage.raycastTarget = false;

        var textObject = new GameObject("Text", typeof(RectTransform), typeof(Text));
        textObject.transform.SetParent(buttonObject.transform, false);
        var textRect = textObject.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(40, 8);
        textRect.offsetMax = new Vector2(-24, -8);

        var text = textObject.GetComponent<Text>();
        text.font = defaultFont;
        text.text = label;
        text.fontSize = 40;
        text.fontStyle = FontStyle.Bold;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;

        return button;
    }

    public Text CreateLabel(RectTransform parent, string label, int fontSize, TextAnchor anchor)
    {
        var textObject = new GameObject("Label", typeof(RectTransform), typeof(Text), typeof(Shadow), typeof(LayoutElement));
        textObject.transform.SetParent(parent, false);
        var text = textObject.GetComponent<Text>();
        text.font = defaultFont;
        text.text = label;
        text.fontSize = fontSize;
        text.alignment = anchor;
        text.color = new Color(0.92f, 0.95f, 0.96f);
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Overflow;
        text.raycastTarget = false;

        var shadow = textObject.GetComponent<Shadow>();
        shadow.effectColor = new Color(0f, 0f, 0f, 0.35f);
        shadow.effectDistance = new Vector2(0f, -3f);

        var element = textObject.GetComponent<LayoutElement>();
        element.minHeight = Mathf.Max(74, fontSize * 2);
        return text;
    }

    private RectTransform CreateScreen(string name, Color color)
    {
        var screenObject = new GameObject(name, typeof(RectTransform), typeof(Image));
        screenObject.transform.SetParent(root, false);

        var rect = screenObject.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        var image = screenObject.GetComponent<Image>();
        image.color = color;
        image.raycastTarget = color.a > 0.05f;

        currentScreenRoot = rect;

        var contentObject = new GameObject("Content", typeof(RectTransform), typeof(VerticalLayoutGroup));
        contentObject.transform.SetParent(screenObject.transform, false);
        var content = contentObject.GetComponent<RectTransform>();
        content.anchorMin = Vector2.zero;
        content.anchorMax = Vector2.one;
        content.offsetMin = Vector2.zero;
        content.offsetMax = Vector2.zero;

        var layout = contentObject.GetComponent<VerticalLayoutGroup>();
        layout.padding = new RectOffset(64, 64, 86, 60);
        layout.spacing = 24;
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childControlWidth = true;
        layout.childForceExpandWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandHeight = false;
        return content;
    }

    private Text CreateTitle(RectTransform parent, string title)
    {
        var text = CreateLabel(parent, title, 72, TextAnchor.MiddleCenter);
        text.color = Color.white;
        text.fontStyle = FontStyle.Bold;
        return text;
    }

    private Color GetThemeBackground(MinigameTheme theme)
    {
        switch (theme)
        {
            case MinigameTheme.Clockwork:
                return new Color(0.09f, 0.10f, 0.15f);
            case MinigameTheme.ColorPop:
                return new Color(0.12f, 0.11f, 0.17f);
            case MinigameTheme.PowerCore:
                return new Color(0.12f, 0.15f, 0.14f);
            case MinigameTheme.DashTrack:
                return new Color(0.08f, 0.13f, 0.17f);
            case MinigameTheme.TargetField:
                return new Color(0.12f, 0.10f, 0.13f);
            case MinigameTheme.MemoryGlow:
                return new Color(0.08f, 0.10f, 0.18f);
            default:
                return new Color(0.08f, 0.10f, 0.14f);
        }
    }

    private void AddScreenDecorations(RectTransform screenRoot, Color primary, Color secondary)
    {
        if (screenRoot == null)
        {
            return;
        }

        CreateAccentBlock(screenRoot, "Accent Slab A", primary, new Vector2(0f, 1f), new Vector2(760f, 120f), new Vector2(90f, -110f), -12f);
        CreateAccentBlock(screenRoot, "Accent Slab B", secondary, new Vector2(1f, 0f), new Vector2(640f, 100f), new Vector2(-70f, 150f), -12f);
        CreateAccentBlock(screenRoot, "Accent Hairline", new Color(1f, 1f, 1f, 0.08f), new Vector2(0.5f, 0f), new Vector2(860f, 5f), new Vector2(0f, 44f), 0f);
    }

    private Image CreateAccentBlock(RectTransform parent, string name, Color color, Vector2 anchor, Vector2 size, Vector2 offset, float rotation)
    {
        var accentObject = new GameObject(name, typeof(RectTransform), typeof(Image));
        accentObject.transform.SetParent(parent, false);
        var rect = accentObject.GetComponent<RectTransform>();
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.sizeDelta = size;
        rect.anchoredPosition = offset;
        rect.localRotation = Quaternion.Euler(0f, 0f, rotation);

        var image = accentObject.GetComponent<Image>();
        image.color = color;
        image.raycastTarget = false;
        image.transform.SetSiblingIndex(0);
        return image;
    }

    private void AddMinigameDecorations(RectTransform screenRoot, MinigameTheme theme)
    {
        switch (theme)
        {
            case MinigameTheme.Clockwork:
                AddScreenDecorations(screenRoot, new Color(0.97f, 0.74f, 0.24f, 0.22f), new Color(0.35f, 0.64f, 0.95f, 0.18f));
                break;
            case MinigameTheme.ColorPop:
                AddScreenDecorations(screenRoot, new Color(0.95f, 0.18f, 0.30f, 0.18f), new Color(0.18f, 0.74f, 0.58f, 0.20f));
                break;
            case MinigameTheme.PowerCore:
                AddScreenDecorations(screenRoot, new Color(0.34f, 0.78f, 0.48f, 0.20f), new Color(0.98f, 0.75f, 0.28f, 0.18f));
                break;
            case MinigameTheme.DashTrack:
                AddScreenDecorations(screenRoot, new Color(0.18f, 0.56f, 0.82f, 0.22f), new Color(0.96f, 0.45f, 0.62f, 0.16f));
                break;
            case MinigameTheme.TargetField:
                AddScreenDecorations(screenRoot, new Color(0.93f, 0.35f, 0.35f, 0.20f), new Color(0.98f, 0.75f, 0.28f, 0.18f));
                break;
            case MinigameTheme.MemoryGlow:
                AddScreenDecorations(screenRoot, new Color(0.40f, 0.82f, 0.88f, 0.18f), new Color(0.48f, 0.42f, 0.92f, 0.20f));
                break;
            default:
                AddScreenDecorations(screenRoot, new Color(0.35f, 0.64f, 0.95f, 0.18f), new Color(0.98f, 0.75f, 0.28f, 0.16f));
                break;
        }
    }
}
