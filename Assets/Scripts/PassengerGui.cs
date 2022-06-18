using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PassengerGui : MonoBehaviour
{
    [SerializeField] private Image Passenger, NoPassenger;

    public void SetPassenger(bool hasPassenger)
    {
        Passenger.gameObject.SetActive(hasPassenger);
        NoPassenger.gameObject.SetActive(!hasPassenger);
    }
}
