using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseMinigame : MonoBehaviour
{
    [SerializeField] private float introDuration = 0.45f;
    [SerializeField] private float countdownStepDuration = 0.72f;
    [SerializeField] private float gameDuration = 20f;
    [SerializeField] private float endingDuration = 0.55f;

    protected readonly ScoreSystem Scores = new ScoreSystem();
    protected IReadOnlyList<PlayerMinigameData> Players;
    protected MinigameUIController UI;
    protected MinigameInputRouter InputRouter;

    private Action<MinigameResult> onComplete;
    private Coroutine flowRoutine;

    public MinigameState State { get; private set; } = MinigameState.Idle;
    public float RemainingTime { get; private set; }
    public float GameDuration => gameDuration;

    public void Initialize(IReadOnlyList<PlayerMinigameData> players, MinigameUIController ui, MinigameInputRouter inputRouter, Action<MinigameResult> completeCallback)
    {
        Players = players;
        UI = ui;
        InputRouter = inputRouter;
        onComplete = completeCallback;
        Scores.Initialize(players);
        RemainingTime = gameDuration;
    }

    public void Begin()
    {
        if (flowRoutine != null)
        {
            StopCoroutine(flowRoutine);
        }

        flowRoutine = StartCoroutine(RunFlow());
    }

    public void RequestEnd()
    {
        if (State == MinigameState.Playing)
        {
            RemainingTime = 0f;
        }
    }

    private IEnumerator RunFlow()
    {
        State = MinigameState.Loading;
        OnLoading();
        UI?.Timer?.SetDuration(gameDuration);
        UI?.UpdateScores(Players);
        yield return null;

        State = MinigameState.Intro;
        OnIntro();
        yield return new WaitForSeconds(introDuration);

        State = MinigameState.Countdown;
        yield return UI != null && UI.Countdown != null ? UI.Countdown.PlayCountdown(countdownStepDuration) : FallbackCountdown();

        State = MinigameState.Playing;
        RemainingTime = gameDuration;
        OnGameStarted();
        while (RemainingTime > 0f && State == MinigameState.Playing)
        {
            float deltaTime = Time.deltaTime;
            RemainingTime = Mathf.Max(0f, RemainingTime - deltaTime);
            InputRouter?.Poll();
            TickSurvival(deltaTime);
            OnGameTick(deltaTime);
            UI?.Timer?.SetRemaining(RemainingTime);
            UI?.UpdateScores(Players);
            yield return null;
        }

        State = MinigameState.Ending;
        OnGameEnded();
        yield return new WaitForSeconds(endingDuration);

        State = MinigameState.Results;
        MinigameResult result = BuildResult();
        UI?.ShowResults(result);
        onComplete?.Invoke(result);
    }

    protected virtual void OnLoading()
    {
    }

    protected virtual void OnIntro()
    {
    }

    protected virtual void OnGameStarted()
    {
    }

    protected virtual void OnGameTick(float deltaTime)
    {
    }

    protected virtual void OnGameEnded()
    {
    }

    protected virtual MinigameResult BuildResult()
    {
        List<PlayerMinigameData> ranked = RankingSystem.RankByScore(Players);
        MinigameResult result = new MinigameResult(name);
        result.SetPlayerResults(ranked);
        return result;
    }

    protected PlayerInputState GetInput(PlayerMinigameData player)
    {
        return InputRouter != null && player != null ? InputRouter.GetState(player.PlayerId) : default;
    }

    private IEnumerator FallbackCountdown()
    {
        yield return new WaitForSeconds(countdownStepDuration * 4f);
    }

    private void TickSurvival(float deltaTime)
    {
        if (Players == null)
        {
            return;
        }

        for (int i = 0; i < Players.Count; i++)
        {
            Players[i].AddSurvivalTime(deltaTime);
        }
    }
}
