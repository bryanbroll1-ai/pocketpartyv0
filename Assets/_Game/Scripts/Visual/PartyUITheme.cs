using UnityEngine;

[CreateAssetMenu(fileName = "PartyUITheme", menuName = "Pocket Party/UI Theme")]
public class PartyUITheme : ScriptableObject
{
    public Color PanelColor = PartyArtPalette.UIPanel;
    public Color TextColor = PartyArtPalette.DarkAccent;
    public Color TimerColor = PartyArtPalette.PlayerYellow;
    public Color ButtonColor = PartyArtPalette.PlayerBlue;
    public Color ButtonTextColor = Color.white;
    public Color[] PlayerColors =
    {
        PartyArtPalette.PlayerBlue,
        PartyArtPalette.PlayerRed,
        PartyArtPalette.PlayerGreen,
        PartyArtPalette.PlayerYellow
    };
}
