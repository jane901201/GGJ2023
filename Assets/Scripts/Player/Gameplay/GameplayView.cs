using System;

using UnityEngine;

public enum GameplayState
{
    Start,
    PlayerSession,
    Reborn,
    Death,
    None
}

public class GameplayView : MonoBehaviour
{
    [SerializeField]
    public GameObject _joystick;

    private GameplayState _state = GameplayState.None;

    public void Render(GameplayState state)
    {
        if(state == _state)
            return;

        _state = state;
        
        switch (state)
        {
            case GameplayState.Start:
            {
                _joystick.SetActive(false);
                break;
            }
            case GameplayState.PlayerSession:
            {
                _joystick.SetActive(true);
                break;
            }
            case GameplayState.Reborn:
            {
                _joystick.SetActive(false);
                break;
            }
            case GameplayState.Death:
            {
                _joystick.SetActive(false);
                break;
            }
            case GameplayState.None:
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }
}
