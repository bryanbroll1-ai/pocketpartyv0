using UnityEngine;

public class PlayerSpawnPoint : MonoBehaviour
{
    [SerializeField] private int playerIndex;

    public int PlayerIndex => playerIndex;

    public void Configure(int index)
    {
        playerIndex = index;
    }
}
