using UnityEngine;
using Yanniboi.Library;

public class GuiManager : SingletonBase<GuiManager>
{
    [SerializeField]
    private PassengerGui Passenger;

    private bool _hasPassenger;

    private void Start()
    {
        UpdateGui();
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

    private void OnEnable()
    {
        TaxiController.OnPickup += OnPassengerPickup;
        TaxiController.OnDropoff += OnPassengerDropoff;
    }

    private void OnDisable()
    {
        TaxiController.OnPickup -= OnPassengerPickup;
        TaxiController.OnDropoff -= OnPassengerDropoff;
    }
}
