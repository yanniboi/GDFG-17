using UnityEngine;

public class Pickup : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log("enter collider");
        Debug.Log(col);
        if (col.TryGetComponent(out TaxiController taxi))
        {
            if (!taxi.HasPassenger())
            {
                taxi.Pickup();
            }
        }
    }
}
