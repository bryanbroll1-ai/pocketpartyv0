using System;
using System.Collections.Generic;

public interface IMinigame
{
    MinigameState State { get; }
    void StartMinigame(IReadOnlyList<PlayerController> players, UIManager ui, Action<MinigameResult> onComplete);
    void CancelMinigame();
}
