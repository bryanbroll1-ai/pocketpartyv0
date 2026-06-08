using UnityEngine;

public static class EditorKeyboardInput
{
    public static PlayerInputState GetState(int playerIndex)
    {
        PlayerInputState state = default;
        KeyCode up;
        KeyCode down;
        KeyCode left;
        KeyCode right;
        KeyCode action;

        switch (playerIndex)
        {
            case 1:
                up = KeyCode.UpArrow;
                down = KeyCode.DownArrow;
                left = KeyCode.LeftArrow;
                right = KeyCode.RightArrow;
                action = KeyCode.RightControl;
                break;
            case 2:
                up = KeyCode.I;
                down = KeyCode.K;
                left = KeyCode.J;
                right = KeyCode.L;
                action = KeyCode.Return;
                break;
            case 3:
                up = KeyCode.T;
                down = KeyCode.G;
                left = KeyCode.F;
                right = KeyCode.H;
                action = KeyCode.LeftShift;
                break;
            default:
                up = KeyCode.W;
                down = KeyCode.S;
                left = KeyCode.A;
                right = KeyCode.D;
                action = KeyCode.Space;
                break;
        }

        Vector2 move = Vector2.zero;
        if (Input.GetKey(left)) move.x -= 1f;
        if (Input.GetKey(right)) move.x += 1f;
        if (Input.GetKey(down)) move.y -= 1f;
        if (Input.GetKey(up)) move.y += 1f;

        state.Move = Vector2.ClampMagnitude(move, 1f);
        state.ActionDown = Input.GetKeyDown(action) || Input.GetKeyDown(KeyCode.Alpha1 + playerIndex);
        state.ActionHeld = Input.GetKey(action) || Input.GetKey(KeyCode.Alpha1 + playerIndex);
        state.ActionUp = Input.GetKeyUp(action) || Input.GetKeyUp(KeyCode.Alpha1 + playerIndex);
        state.TapDown = state.ActionDown;
        state.Hold = state.ActionHeld;
        state.TapUp = state.ActionUp;
        return state;
    }
}
