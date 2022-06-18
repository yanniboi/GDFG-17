using System;
using UnityEngine;
using Yanniboi.Library;

public class GameManager : SingletonBase<GameManager>
{
    public static event Action OnWin;

    private bool _isPaused = false;
    private bool _gameOver = false;

    
    private void Update()
    {
        if (_gameOver)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (IsPaused())
            {
                UnPause();
            }
            else
            {
                Pause();
            }
            GuiManager.Instance.ToggleMenu();
        }
    }

    public bool IsPaused()
    {
        return _isPaused;
    }

    public void Pause()
    {
        _isPaused = true;
        Time.timeScale = 0;
    }

    public void UnPause()
    {
        _isPaused = false;
        Time.timeScale = 1;
    }
    
    public void WinGame()
    {
        Pause();
        _gameOver = true;
        OnWin?.Invoke();
    }
}
