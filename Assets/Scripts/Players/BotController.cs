using UnityEngine;

public class BotController : MonoBehaviour
{
    public float GetReactionDelay(float minDelay, float maxDelay)
    {
        return Random.Range(minDelay, maxDelay);
    }

    public float GetTimeStopResult()
    {
        return Mathf.Clamp(Random.Range(0.78f, 1.22f), 0.1f, 2f);
    }

    public int GetTapRateScore(int minScore, int maxScore)
    {
        return Random.Range(minScore, maxScore + 1);
    }
}
