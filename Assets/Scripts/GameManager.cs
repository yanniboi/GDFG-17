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
        if (this._gameOver)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (this.IsPaused())
            {
                this.UnPause();
            }
            else
            {
                this.Pause();
            }
            GuiManager.Instance.ToggleMenu();
        }
    }

    public bool IsPaused()
    {
        return this._isPaused;
    }

    public void Pause()
    {
        this._isPaused = true;
        Time.timeScale = 0;
    }

    public void UnPause()
    {
        this._isPaused = false;
        Time.timeScale = 1;
    }

    public void WinGame()
    {
        this.Pause();
        this._gameOver = true;
        OnWin?.Invoke();
    }
}
