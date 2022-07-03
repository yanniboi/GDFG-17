using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class TaxiIndicator : MonoBehaviour
{
    private Light2D _light;

    private void Start()
    {
        _light = GetComponent<Light2D>();
    }

    private void TurnOn()
    {
        _light.color = Color.green;
        _light.intensity = 3;
    }

    
    private void TurnOff()
    {
        _light.color = Color.red;
        _light.intensity = 1;
    }
    
    private void OnEnable()
    {
        TaxiController.OnPickup += this.TurnOn;
        TaxiController.OnDropoff += this.TurnOff;
    }

    private void OnDisable()
    {
        TaxiController.OnPickup -= this.TurnOn;
        TaxiController.OnDropoff -= this.TurnOff;
    }

}
