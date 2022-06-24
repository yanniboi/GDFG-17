using UnityEngine;

public class CapsuleController : MonoBehaviour
{

    public void DropoffComplete()
    {
        GameManager.Instance.WinGame();
    }
}
