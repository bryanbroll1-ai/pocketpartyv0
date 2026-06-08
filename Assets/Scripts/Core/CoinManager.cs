using UnityEngine;

public class CoinManager : MonoBehaviour
{
    public void AddCoins(PlayerController player, int amount)
    {
        player.AddCoins(amount);
    }

    public void AwardPlacementCoins(MinigameResult result)
    {
        for (int i = 0; i < result.Placements.Count; i++)
        {
            int award = i == 0 ? 10 : i == 1 ? 6 : i == 2 ? 3 : 1;
            result.Placements[i].AddCoins(award);
        }
    }
}
