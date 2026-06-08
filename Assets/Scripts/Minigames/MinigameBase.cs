using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class MinigameBase : MonoBehaviour, IMinigame
{
    protected readonly List<PlayerController> Players = new List<PlayerController>();
    protected UIManager UI;
    protected Action<MinigameResult> OnComplete;
    protected RectTransform ContentRoot;
    protected bool IsRunning;

    public MinigameState State { get; private set; } = MinigameState.Idle;
    public float RemainingTime { get; private set; }
    protected virtual float CountdownDuration => 3f;
    protected virtual float GameDuration => 30f;

    public virtual void StartMinigame(IReadOnlyList<PlayerController> players, UIManager ui, Action<MinigameResult> onComplete)
    {
        Players.Clear();
        Players.AddRange(players);
        UI = ui;
        OnComplete = onComplete;
        IsRunning = true;
        State = MinigameState.Setup;
        RemainingTime = GameDuration;
    }

    public virtual void CancelMinigame()
    {
        if (State == MinigameState.Complete)
        {
            return;
        }

        IsRunning = false;
        State = MinigameState.Cancelled;
        StopAllCoroutines();
        RuntimeMinigameArena.Clear();
        Destroy(gameObject);
    }

    protected void Finish(MinigameResult result)
    {
        if (!IsRunning)
        {
            return;
        }

        IsRunning = false;
        State = MinigameState.Complete;
        OnComplete?.Invoke(result);
        Destroy(gameObject);
    }

    protected IEnumerator RunCountdownAndTimer(Text countdownText, Text timerText)
    {
        State = MinigameState.Countdown;
        float countdownRemaining = CountdownDuration;
        while (countdownRemaining > 0f)
        {
            int shownSecond = Mathf.CeilToInt(countdownRemaining);
            if (countdownText != null)
            {
                countdownText.text = shownSecond.ToString();
            }

            OnCountdownTick(shownSecond);
            countdownRemaining -= Time.unscaledDeltaTime;
            yield return null;
        }

        if (countdownText != null)
        {
            countdownText.text = "Go";
        }

        State = MinigameState.Running;
        RemainingTime = GameDuration;
        OnMinigameStarted();

        while (RemainingTime > 0f && State == MinigameState.Running)
        {
            RemainingTime -= Time.unscaledDeltaTime;
            float clampedTime = Mathf.Max(0f, RemainingTime);
            if (timerText != null)
            {
                timerText.text = $"{clampedTime:0.0}s";
            }

            OnMinigameTimerTick(clampedTime);
            yield return null;
        }

        if (State == MinigameState.Running)
        {
            OnMinigameTimeExpired();
        }
    }

    protected virtual void OnCountdownTick(int shownSecond)
    {
    }

    protected virtual void OnMinigameStarted()
    {
    }

    protected virtual void OnMinigameTimerTick(float remainingTime)
    {
    }

    protected virtual void OnMinigameTimeExpired()
    {
    }

    protected MinigameInputState GetInput(PlayerController player)
    {
        int playerIndex = player != null ? player.PlayerIndex : 0;
        return MobileMinigameInput.GetState(playerIndex);
    }

    protected bool PointerDownThisFrame()
    {
        if (Input.GetMouseButtonDown(0))
        {
            return true;
        }

        for (int i = 0; i < Input.touchCount; i++)
        {
            if (Input.GetTouch(i).phase == TouchPhase.Began)
            {
                return true;
            }
        }

        return false;
    }

    protected bool PointerHeld()
    {
        return Input.GetMouseButton(0) || Input.touchCount > 0;
    }

    protected bool PointerUpThisFrame()
    {
        if (Input.GetMouseButtonUp(0))
        {
            return true;
        }

        for (int i = 0; i < Input.touchCount; i++)
        {
            if (Input.GetTouch(i).phase == TouchPhase.Ended || Input.GetTouch(i).phase == TouchPhase.Canceled)
            {
                return true;
            }
        }

        return false;
    }

    protected Vector2 PointerPosition()
    {
        if (Input.touchCount > 0)
        {
            return Input.GetTouch(0).position;
        }

        return Input.mousePosition;
    }
}
