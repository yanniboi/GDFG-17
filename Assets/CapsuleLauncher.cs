using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapsuleLauncher : MonoBehaviour
{

    [SerializeField] private GameObject _prefab;

    public void Launch()
    {
        GameObject capsule = Instantiate(_prefab, transform.position, Quaternion.identity);
    }
}
