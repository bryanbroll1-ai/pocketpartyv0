using System.Collections.Generic;
using UnityEngine;

public class MinigameInputRouter : MonoBehaviour
{
    [SerializeField] private bool editorKeyboardEnabled = true;

    private readonly PlayerInputState[] states = new PlayerInputState[4];
    private readonly List<MobileInputZone> mobileZones = new List<MobileInputZone>();

    public void RegisterMobileZones(IEnumerable<MobileInputZone> zones)
    {
        mobileZones.Clear();
        if (zones == null)
        {
            return;
        }

        foreach (MobileInputZone zone in zones)
        {
            if (zone != null)
            {
                mobileZones.Add(zone);
            }
        }
    }

    public void Poll()
    {
        for (int i = 0; i < states.Length; i++)
        {
            states[i] = editorKeyboardEnabled ? EditorKeyboardInput.GetState(i) : default;
        }

        foreach (MobileInputZone zone in mobileZones)
        {
            PlayerInputState zoneState = zone.ConsumeState();
            int index = Mathf.Clamp(zone.PlayerIndex, 0, states.Length - 1);
            states[index] = Merge(states[index], zoneState);
        }
    }

    public PlayerInputState GetState(int playerIndex)
    {
        return states[Mathf.Clamp(playerIndex, 0, states.Length - 1)];
    }

    private static PlayerInputState Merge(PlayerInputState a, PlayerInputState b)
    {
        a.Move = b.Move.sqrMagnitude > a.Move.sqrMagnitude ? b.Move : a.Move;
        a.Position = b.Position;
        a.SwipeDelta = b.SwipeDelta.sqrMagnitude > a.SwipeDelta.sqrMagnitude ? b.SwipeDelta : a.SwipeDelta;
        a.TapDown |= b.TapDown;
        a.Hold |= b.Hold;
        a.TapUp |= b.TapUp;
        a.SwipeUp |= b.SwipeUp;
        a.SwipeDown |= b.SwipeDown;
        a.SwipeLeft |= b.SwipeLeft;
        a.SwipeRight |= b.SwipeRight;
        a.ActionDown |= b.ActionDown;
        a.ActionHeld |= b.ActionHeld;
        a.ActionUp |= b.ActionUp;
        return a;
    }
}
