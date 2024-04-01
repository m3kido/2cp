using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void OnNewGameClicked()
    {
        DataPersistenceManager.Instance.NewGame();
    }

    public void OnLoadGameClicked()
    {
        DataPersistenceManager.Instance.LoadGame();
    }
}
