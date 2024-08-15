using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


// Class to manage the main menu
public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject[] Screens;


    [SerializeField] private GameObject[] _maps;


    [SerializeField] private GameObject mapImage;

    [SerializeField] private Sprite[] mapImages;

    [SerializeField] private GameObject[] _playerSprites;
    [SerializeField] private GameObject[] _playerSpritesBorder;
    [SerializeField] private GameObject[] _playerColor;
    [SerializeField] private GameObject[] _playerColorBorder;

    [SerializeField] private GameObject playBorder;
    [SerializeField] private Sprite[] _captainSprites;

    [SerializeField] private GameObject _player3;
    [SerializeField] private GameObject _player4;

    private int selectedPlayer = 0;
    private int optionselected = 0;
    private int currentoption = 0;

    public static int _nbplayers = 2;
    public static  ECaptains[] currentCaptains = { ECaptains.Andrew, ECaptains.Godfrey, ECaptains.Maximus, ECaptains.Melina };
    public static ETeamColors[] currentColors = { ETeamColors.Amber, ETeamColors.Azure, ETeamColors.Verdant, ETeamColors.Gilded };

    private int _currentScreen = 0;
    private int _mapIndex = 0;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_currentScreen == 0)
            {
                PlayGame();
            }
            else if (_currentScreen == 1)
            {
                PlayGame();
                switch (_mapIndex)
                {
                    case 2: _nbplayers=3; break;
                    case 3: _nbplayers = 3; break;
                    case 4: _nbplayers = 4; break;
                    default: _nbplayers = 2; break;
                }
                if (_nbplayers <= 3)
                {
                    _player4.gameObject.SetActive(false);
                    
                }
                if(_nbplayers == 2)
                {
                    _player3.gameObject.SetActive(false);
                }
            }
            else
            {
                if (currentoption == 2)
                {
                    SceneManager.LoadScene(_mapIndex + 1);
                }
                else
                {
                    optionselected = (optionselected + 1) % 2;
                    HighlightSelection();
                }

            }

        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {

            if (_currentScreen == 1)
            {
                HighlightOption((_mapIndex + 1) % 5);
                _mapIndex = (_mapIndex + 1) % 5;
            }
            else if (_currentScreen == 2 && optionselected == 0)
            {
                ChangeSelectionOff();
                currentoption = (currentoption + 1) % 3;
                ChangeSelectionOn();
            }


        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {

            if (_currentScreen == 1)
            {
                HighlightOption((_mapIndex + 4) % 5);
                _mapIndex = (_mapIndex + 4) % 5;
            }
            else if (_currentScreen == 2 && optionselected == 0)
            {
                ChangeSelectionOff();
                currentoption = (currentoption + 2) % 3;
                ChangeSelectionOn();
            }

        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {


            if (_currentScreen == 2 && optionselected!=3)
            {
                if (optionselected == 1)
                {
                    if (currentoption == 0)
                    {
                        currentCaptains[selectedPlayer] = (ECaptains)(((int)currentCaptains[selectedPlayer] + 1) % 4);
                        ChangeCaptain();
                    }
                    else if (currentoption == 1)
                    {
                        if (_nbplayers != 4)
                        {
                            bool changed = false;
                            ETeamColors newColor = currentColors[selectedPlayer];
                            while (!changed)
                            {
                                newColor = (ETeamColors)(((int)newColor + 1) % 5);
                                if (newColor == currentColors[selectedPlayer]) { break; }
                                if (newColor == ETeamColors.Neutral) { continue; }
                                bool exists = false;
                                for (int i = 0; i < _nbplayers; i++)
                                {
                                    if (currentColors[i] == newColor) { exists = true; break; }
                                }
                                if (!exists)
                                {
                                    currentColors[selectedPlayer] = newColor;
                                    changed = true;
                                    ChangeColor();
                                }
                            }
                        }

                    }
                }
                else
                {
                    ChangeSelectionOff();
                    selectedPlayer = (selectedPlayer + 1) % _nbplayers;
                    ChangeSelectionOn();
                }

            }

        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {

            if (_currentScreen == 2 && optionselected != 3)
            {
                if (optionselected == 1)
                {
                    if (currentoption == 0)
                    {
                        currentCaptains[selectedPlayer] = (ECaptains)(((int)currentCaptains[selectedPlayer] + 3) % 4);
                        ChangeCaptain();
                    }
                    else if (currentoption == 1)
                    {
                        if (_nbplayers != 4)
                        {
                            bool changed = false;
                            ETeamColors newColor = currentColors[selectedPlayer] ;
                            while (!changed)
                            {
                                newColor = (ETeamColors)(((int)newColor + 4) % 5);
                                if (newColor == currentColors[selectedPlayer] ) { break; }
                                if( newColor == ETeamColors.Neutral) { continue; }
                                bool exists = false;
                                for (int i = 0; i < _nbplayers; i++)
                                {
                                    if (currentColors[i] == newColor) { exists = true; break; }
                                }
                                if (!exists)
                                {
                                    currentColors[selectedPlayer] = newColor;
                                    changed = true;
                                    ChangeColor();
                                }
                            }
                        }
                    }
                }
                else
                {
                    ChangeSelectionOff();
                    selectedPlayer = (selectedPlayer - 1 + _nbplayers) % _nbplayers;
                    ChangeSelectionOn();
                }

            }
        }



    }
    private void HighlightSelection()
    {
        if (currentoption == 0)
        {
            if(optionselected == 0)
            {
                _playerSpritesBorder[selectedPlayer].GetComponent<Image>().color = Color.yellow;
            }
            else
            {
                _playerSpritesBorder[selectedPlayer].GetComponent<Image>().color = Color.red;
            }
         
        }
        else
        {
            if (optionselected == 0)
            {
                _playerColorBorder[selectedPlayer].GetComponent<Image>().color = Color.yellow;
            }
            else
            {
                _playerColorBorder[selectedPlayer].GetComponent<Image>().color = Color.red;
            }
        }
    }
    private void ChangeSelectionOn()
    {
        if (currentoption == 2) { playBorder.gameObject.SetActive(true); }
        if (currentoption == 0)
        {
            _playerSpritesBorder[selectedPlayer].gameObject.SetActive(true);
        }
        else
        {
            _playerColorBorder[selectedPlayer].gameObject.SetActive(true);
        }
    }
    private void ChangeSelectionOff()
    {
        if (currentoption == 2) { playBorder.gameObject.SetActive(false); ; }
        if (currentoption == 0)
        {
            _playerSpritesBorder[selectedPlayer].gameObject.SetActive(false);
        }
        else
        {
            _playerColorBorder[selectedPlayer].gameObject.SetActive(false);
        }
    }
    private void ChangeCaptain()
    {
       

        _playerSprites[selectedPlayer].GetComponent<Image>().sprite = _captainSprites[(int)currentCaptains[selectedPlayer]];
    }
    private void ChangeColor()
    {
        Color jit = Color.red;
        switch (currentColors[selectedPlayer])
        {
            case ETeamColors.Amber :jit=Color.red; break;
            case ETeamColors.Azure: jit = Color.blue; break;
            case ETeamColors.Gilded: jit = Color.green; break;
            case ETeamColors.Verdant: jit = Color.yellow; break;
        }
        _playerColor[selectedPlayer].GetComponent<Image>().color = jit ;
    }
    private void HighlightOption(int index)
    {
        _maps[_mapIndex].transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().color = Color.white;
        _maps[index].transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().color = new Color(1, 1, 0);
        mapImage.GetComponent<Image>().sprite = mapImages[index];
    }
    public void PlayGame()
    {
        Screens[_currentScreen].SetActive(false);
        _currentScreen++;
        Screens[_currentScreen].SetActive(true);
    }
}
