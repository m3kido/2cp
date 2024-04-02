using UnityEngine;

// Class to represent the main menu
public class MainMenu : MonoBehaviour
{
    // Just random functions i wrote
    public void OnNewGameClicked()
    {
        DataPersistenceManager.Instance.NewGame();
    }

    public void OnLoadGameClicked()
    {
        DataPersistenceManager.Instance.LoadGame();
    }
}
