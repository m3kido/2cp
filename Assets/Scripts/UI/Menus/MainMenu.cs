using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject[] Screens;
  

    [SerializeField] private GameObject[] _maps;
    

    [SerializeField] private GameObject mapImage;

    [SerializeField] private Sprite[] mapImages;
   


    private int _currentScreen = 0;
    private int _mapIndex = 0;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(_currentScreen == 0)
            {
                PlayGame();
            }
            else if(_currentScreen == 1)
            {
                PlayGame();
            }
            else
            {

            }
          
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
           
            if (_currentScreen == 1)
            {
                HighlightOption((_mapIndex + 1) % 5);
                _mapIndex = (_mapIndex + 1) % 5;
            }
            else
            {

            }

        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            
            if (_currentScreen == 1)
            {
                HighlightOption((_mapIndex +4) % 5);
                _mapIndex = (_mapIndex + 4) % 5;
            }
            else
            {

            }

        }

    }
    private void HighlightOption(int index)
    {
        _maps[_mapIndex].transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().color = Color.white;
        _maps[index].transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().color = new Color(1,1,0);
        mapImage.GetComponent<Image>().sprite= mapImages[index];
    }
    public void PlayGame()
    {
        Screens[_currentScreen].SetActive(false);
        _currentScreen++;
        Screens[_currentScreen].SetActive(true);
    }
}
