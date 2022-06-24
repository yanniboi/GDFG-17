using UnityEngine;
using UnityEngine.UI;

public class PassengerGui : MonoBehaviour
{
    [SerializeField] private Image Passenger, NoPassenger;

    public void SetPassenger(bool hasPassenger)
    {
        this.Passenger.gameObject.SetActive(hasPassenger);
        this.NoPassenger.gameObject.SetActive(!hasPassenger);
    }
}
