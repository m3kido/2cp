using UnityEngine;
using UnityEngine.SceneManagement;

// Class to manage the main menu
public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("SceneOne");
    }
}
