using UnityEngine;
using UnityEngine.UI;

public static class PartyUIShowcaseFactory
{
    public static void BuildShowcaseUi()
    {
        GameObject canvasObject = new GameObject("Party UI Showcase");
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObject.AddComponent<GraphicRaycaster>();
        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        scaler.matchWidthOrHeight = 0.5f;

        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        CreatePanel(canvas.transform, "00:30", new Vector2(0.5f, 1f), new Vector2(250f, 82f), new Vector2(0f, -58f), PartyArtPalette.PlayerYellow, font, 44);
        CreateScoreColumns(canvas.transform, font);
        CreatePanel(canvas.transform, "3", new Vector2(0.5f, 0.58f), new Vector2(132f, 132f), Vector2.zero, PartyArtPalette.WhiteAccent, font, 70);
        CreatePanel(canvas.transform, "RESULT\nP1", new Vector2(1f, 0f), new Vector2(270f, 126f), new Vector2(-160f, 104f), PartyArtPalette.UIPanel, font, 34);
        CreatePanel(canvas.transform, "CAM", new Vector2(1f, 1f), new Vector2(132f, 76f), new Vector2(-92f, -58f), PartyArtPalette.PlayerBlue, font, 32);
    }

    private static void CreateScoreColumns(Transform parent, Font font)
    {
        for (int i = 0; i < 4; i++)
        {
            bool leftSide = i < 2;
            Vector2 anchor = leftSide ? new Vector2(0f, 1f) : new Vector2(1f, 1f);
            float x = leftSide ? 124f : -124f;
            float y = -138f - (i % 2) * 86f;
            CreatePanel(parent, $"P{i + 1}  0", anchor, new Vector2(190f, 70f), new Vector2(x, y), PartyArtPalette.PlayerColor(i), font, 30);
        }
    }

    private static void CreatePanel(Transform parent, string text, Vector2 anchor, Vector2 size, Vector2 offset, Color color, Font font, int fontSize)
    {
        GameObject panel = new GameObject(text, typeof(RectTransform), typeof(Image), typeof(Outline), typeof(Shadow));
        panel.transform.SetParent(parent, false);
        RectTransform rect = panel.GetComponent<RectTransform>();
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.sizeDelta = size;
        rect.anchoredPosition = offset;
        Image image = panel.GetComponent<Image>();
        image.color = color;
        image.raycastTarget = false;
        Outline outline = panel.GetComponent<Outline>();
        outline.effectColor = new Color(0.12f, 0.16f, 0.20f, 0.18f);
        outline.effectDistance = new Vector2(3f, -3f);
        Shadow shadow = panel.GetComponent<Shadow>();
        shadow.effectColor = new Color(0f, 0f, 0f, 0.14f);
        shadow.effectDistance = new Vector2(0f, -5f);

        GameObject label = new GameObject("Label", typeof(RectTransform), typeof(Text));
        label.transform.SetParent(panel.transform, false);
        RectTransform labelRect = label.GetComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = new Vector2(12f, 8f);
        labelRect.offsetMax = new Vector2(-12f, -8f);
        Text labelText = label.GetComponent<Text>();
        labelText.font = font;
        labelText.text = text;
        labelText.fontSize = fontSize;
        labelText.fontStyle = FontStyle.Bold;
        labelText.alignment = TextAnchor.MiddleCenter;
        labelText.color = PartyArtPalette.DarkAccent;
        labelText.raycastTarget = false;
    }
}
