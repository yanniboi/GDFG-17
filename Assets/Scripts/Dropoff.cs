using UnityEngine;

public class Dropoff : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log("enter collider");
        Debug.Log(col);

        if (col.TryGetComponent(out CapsuleController capsule))
        {
            capsule.DropoffComplete();
        }
    }
}
