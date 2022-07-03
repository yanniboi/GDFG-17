using UnityEngine;

public class WinScreen : MonoBehaviour
{
    public void PressButton()
    {
        // LoadingManager.LoadScene("LevelGeneration", this.Seed);
        Application.Quit();
    }
}
