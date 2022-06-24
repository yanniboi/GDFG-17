using System;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    private float lifeTime = 0;

    private void Update()
    {
        lifeTime += Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (lifeTime <= 2)
        {
            return;
        }
        
        if (col.TryGetComponent(out TaxiController taxi))
        {
            if (!taxi.HasPassenger())
            {
                taxi.Pickup();
                Destroy(gameObject);
            }
        }
    }
}
