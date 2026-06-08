using UnityEngine;

public struct PlayerInputState
{
    public Vector2 Move;
    public Vector2 Position;
    public Vector2 SwipeDelta;
    public bool TapDown;
    public bool Hold;
    public bool TapUp;
    public bool SwipeUp;
    public bool SwipeDown;
    public bool SwipeLeft;
    public bool SwipeRight;
    public bool ActionDown;
    public bool ActionHeld;
    public bool ActionUp;

    public bool AnyPressed => TapDown || ActionDown || SwipeUp || SwipeDown || SwipeLeft || SwipeRight;
}
