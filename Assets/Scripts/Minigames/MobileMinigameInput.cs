using UnityEngine;

public static class MobileMinigameInput
{
    private const float SwipeThreshold = 72f;
    private static Vector2 pointerStart;
    private static Vector2 previousPosition;
    private static bool pointerWasHeld;

    public static MinigameInputState GetState(int playerIndex)
    {
        if (playerIndex != 0)
        {
            return default;
        }

        bool down = false;
        bool held = false;
        bool up = false;
        Vector2 position = previousPosition;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            position = touch.position;
            down = touch.phase == TouchPhase.Began;
            held = touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary;
            up = touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled;
        }
        else
        {
            position = Input.mousePosition;
            down = Input.GetMouseButtonDown(0);
            held = Input.GetMouseButton(0);
            up = Input.GetMouseButtonUp(0);
        }

        if (down || (held && !pointerWasHeld))
        {
            pointerStart = position;
        }

        Vector2 delta = position - pointerStart;
        Vector2 frameDelta = position - previousPosition;
        bool swipe = up && delta.magnitude >= SwipeThreshold;

        previousPosition = position;
        pointerWasHeld = held && !up;

        return new MinigameInputState
        {
            Move = held ? Vector2.ClampMagnitude(frameDelta / 80f, 1f) : Vector2.zero,
            Position = position,
            SwipeDelta = delta,
            TapDown = down,
            Hold = held,
            TapUp = up,
            Swipe = swipe
        };
    }
}
