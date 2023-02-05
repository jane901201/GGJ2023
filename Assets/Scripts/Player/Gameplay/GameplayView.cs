using System;

using UnityEngine;

public class GameplayView : MonoBehaviour
{
    [SerializeField]
    public GameObject _joystick;
    [SerializeField]
    private MainGameUI _mainGameUI;
    [SerializeField]
    private GameOverUI _gameOverUI;

    private GameplayState _state = GameplayState.None;

    public void SetHumidity(int humidity)
    {
        _mainGameUI.SetHumidityText(humidity);
    }

    public void SetScore(int currentScore, int historyScore)
    {
        _gameOverUI.SetCurrentScore(currentScore);
        _gameOverUI.SetHistoryScore(historyScore);
    }

    public void SetCountdown(float seconds)
    {
        _mainGameUI.SetCountDownText(seconds);
    }

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
                _gameOverUI.gameObject.SetActive(true);
                _mainGameUI.gameObject.SetActive(false);
                break;
            }
            case GameplayState.None:
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }
}
