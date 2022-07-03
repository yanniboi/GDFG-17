using TMPro;
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

    [SerializeField]
    private TMP_Text Instructions;

    private bool _hasPassenger;
    private string _instructions = "Find a passenger";

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
        this.Instructions.text = _instructions;
    }

    private void OnPassengerPickup()
    {
        this._hasPassenger = true;
        this._instructions = "Take them to the dropoff";
        this.UpdateGui();
    }

    private void OnPassengerDropoff()
    {
        this._hasPassenger = false;
        this._instructions = "...";
        Invoke(nameof(DroppedInstructions), 3f);
        this.UpdateGui();
    }

    private void DroppedInstructions()
    {
        _instructions = "That's not right... try again!";
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
