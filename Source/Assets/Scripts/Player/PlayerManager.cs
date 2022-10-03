using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerStates
{
    Move,
    Inspect,
    Wait
}
public class PlayerManager : MonoBehaviour
{
    public static Action<PlayerStates> OnPlayerStateChanged;

    public static PlayerStates _currentState = PlayerStates.Move;


    public static PlayerStates CurrentState {
        get {
            return _currentState;
        }

        set {
            _currentState = value;
            OnPlayerStateChanged?.Invoke(value);
        }
    }
}
