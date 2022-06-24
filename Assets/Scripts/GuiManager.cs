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
        this.UpdateGui();
    }

    public void ToggleMenu()
    {
        this._menuIsShowing = !this._menuIsShowing;
        this.UpdateMenu();
    }

    private void UpdateGui()
    {
        this.Passenger.SetPassenger(this._hasPassenger);
    }

    private void OnPassengerPickup()
    {
        this._hasPassenger = true;
        this.UpdateGui();
    }

    private void OnPassengerDropoff()
    {
        this._hasPassenger = false;
        this.UpdateGui();
    }

    private void ShowWinScreen()
    {
        this.ShowShade();
        this.WinScreen.SetActive(true);
    }

    private void UpdateMenu()
    {
        if (this._menuIsShowing)
        {
            this.ShowMenu();
        }
        else
        {
            this.HideMenu();
        }
    }

    private void ShowShade()
    {
        Color color = this.Shade.color;
        color.a = 0.9f;
        this.Shade.color = color;
    }

    private void HideShade()
    {
        Color color = this.Shade.color;
        color.a = 0f;
        this.Shade.color = color;
    }

    private void ShowMenu()
    {
        this.ShowShade();
    }

    private void HideMenu()
    {
        this.HideShade();
    }

    private void OnEnable()
    {
        TaxiController.OnPickup += this.OnPassengerPickup;
        TaxiController.OnDropoff += this.OnPassengerDropoff;
        GameManager.OnWin += this.ShowWinScreen;
    }

    private void OnDisable()
    {
        TaxiController.OnPickup -= this.OnPassengerPickup;
        TaxiController.OnDropoff -= this.OnPassengerDropoff;
        GameManager.OnWin -= this.ShowWinScreen;
    }
}
