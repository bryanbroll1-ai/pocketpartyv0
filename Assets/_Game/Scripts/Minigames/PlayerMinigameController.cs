using UnityEngine;

public class PlayerMinigameController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2.8f;

    public PlayerMinigameData Data { get; private set; }

    public void Initialize(PlayerMinigameData data)
    {
        Data = data;
    }

    public void ApplyInput(PlayerInputState input, float deltaTime)
    {
        Vector3 move = new Vector3(input.Move.x, 0f, input.Move.y);
        if (move.sqrMagnitude > 0.001f)
        {
            transform.position += Vector3.ClampMagnitude(move, 1f) * moveSpeed * deltaTime;
            transform.rotation = Quaternion.LookRotation(move, Vector3.up);
            Data?.SetDistance(Vector3.Distance(Vector3.zero, transform.position));
        }
    }
}
