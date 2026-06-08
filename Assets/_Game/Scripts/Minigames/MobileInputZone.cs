using UnityEngine;
using UnityEngine.EventSystems;

public class MobileInputZone : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField] private int playerIndex;
    [SerializeField] private bool actionZone = true;

    private const float SwipeThreshold = 70f;
    private Vector2 startPosition;
    private Vector2 currentPosition;
    private bool downThisFrame;
    private bool upThisFrame;
    private bool held;
    private Vector2 swipeDelta;

    public int PlayerIndex => playerIndex;

    public void Configure(int index, bool isActionZone)
    {
        playerIndex = index;
        actionZone = isActionZone;
    }

    public PlayerInputState ConsumeState()
    {
        PlayerInputState state = default;
        state.Position = currentPosition;
        state.SwipeDelta = swipeDelta;
        state.TapDown = downThisFrame;
        state.Hold = held;
        state.TapUp = upThisFrame;
        state.ActionDown = actionZone && downThisFrame;
        state.ActionHeld = actionZone && held;
        state.ActionUp = actionZone && upThisFrame;

        if (upThisFrame && swipeDelta.magnitude >= SwipeThreshold)
        {
            if (Mathf.Abs(swipeDelta.x) > Mathf.Abs(swipeDelta.y))
            {
                state.SwipeRight = swipeDelta.x > 0f;
                state.SwipeLeft = swipeDelta.x < 0f;
            }
            else
            {
                state.SwipeUp = swipeDelta.y > 0f;
                state.SwipeDown = swipeDelta.y < 0f;
            }
        }

        downThisFrame = false;
        upThisFrame = false;
        swipeDelta = Vector2.zero;
        return state;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        held = true;
        downThisFrame = true;
        startPosition = eventData.position;
        currentPosition = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        currentPosition = eventData.position;
        swipeDelta = currentPosition - startPosition;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        held = false;
        upThisFrame = true;
        currentPosition = eventData.position;
        swipeDelta = currentPosition - startPosition;
    }
}
