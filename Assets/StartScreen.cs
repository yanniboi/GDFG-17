using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartScreen : MonoBehaviour
{
    public void PressButton()
    {
        LoadingManager.LoadScene("SampleScene", "cheese");
    }
}
