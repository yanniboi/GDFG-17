using UnityEngine;

public class StartScreen : MonoBehaviour
{
    public void PressButton()
    {
        LoadingManager.LoadScene("LevelGeneration", "cheese");
    }
}
