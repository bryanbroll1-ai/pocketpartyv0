using System;
using System.Collections.Generic;
using UnityEngine;

public class ResultsPanel : MonoBehaviour
{
    public void Show(UIManager uiManager, IReadOnlyList<PlayerController> rankedPlayers, Action onMainMenu)
    {
        uiManager.ShowFinalResults(rankedPlayers, onMainMenu);
    }
}
