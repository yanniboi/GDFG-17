using UnityEngine;

public class StartScreen : MonoBehaviour
{

    [SerializeField] private string seed = "Cheese";
    public string Seed { get { return seed; } }


    public void PressButton()
    {
        LoadingManager.LoadScene("LevelGeneration", this.Seed);
    }
}
