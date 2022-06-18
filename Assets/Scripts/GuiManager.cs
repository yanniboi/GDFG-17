using System;
using UnityEngine;
using UnityEngine.UI;
using Yanniboi.Library;

public class GuiManager : SingletonBase<GuiManager>
{
    [SerializeField]
    private PassengerGui Passenger;
    
    [SerializeField]
    private Image Shade;

    [SerializeField]
    private GameObject WinScreen;

    
    private bool _hasPassenger;

    private bool _menuIsShowing = false;
    
    private void Start()
    {
        UpdateGui();
    }

    public void ToggleMenu()
    {
        _menuIsShowing = !_menuIsShowing;
        UpdateMenu();
    }
    
    private void UpdateGui()
    {
        Passenger.SetPassenger(_hasPassenger);
    }

    private void OnPassengerPickup()
    {
        _hasPassenger = true;
        UpdateGui();
    }

    private void OnPassengerDropoff()
    {
        _hasPassenger = false;
        UpdateGui();
    }

    private void ShowWinScreen()
    {
        ShowShade();
        WinScreen.SetActive(true);
    }
    
    private void UpdateMenu()
    {
        if (_menuIsShowing)
        {
            ShowMenu();
        }
        else
        {
            HideMenu();
        }
    }

    private void ShowShade()
    {
        Color color = Shade.color;
        color.a = 0.9f;
        Shade.color = color;
    }
    
    private void HideShade()
    {
        Color color = Shade.color;
        color.a = 0f;
        Shade.color = color;
    }
    
    private void ShowMenu()
    {
        ShowShade();
    }

    private void HideMenu()
    {
        HideShade();
    }

    private void OnEnable()
    {
        TaxiController.OnPickup += OnPassengerPickup;
        TaxiController.OnDropoff += OnPassengerDropoff;
        GameManager.OnWin += ShowWinScreen;
    }

    private void OnDisable()
    {
        TaxiController.OnPickup -= OnPassengerPickup;
        TaxiController.OnDropoff -= OnPassengerDropoff;
        GameManager.OnWin -= ShowWinScreen;
    }
}
