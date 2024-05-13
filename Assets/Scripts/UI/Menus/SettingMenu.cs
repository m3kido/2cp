using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Class to manage the settings menu
public class SettingMenu : MonoBehaviour
{
    #region Variables
    private GameManager _gm;
    private GameDataSaveManager _sm;

    private List<GameObject> _settingsList;

    private int _selectedSetting;

    [SerializeField] GameObject SaveSetting;
    [SerializeField] GameObject OptionsSetting;
    [SerializeField] GameObject EndSetting;

    [SerializeField] private Color32 textColor = new(115, 42, 28,255);
    #endregion

    #region UnityMethods
    // Start is called before the first frame update
    private void Awake()
    {
        _gm = FindAnyObjectByType<GameManager>();
        _sm = FindAnyObjectByType<GameDataSaveManager>();

        _settingsList = new List<GameObject>
        {
            SaveSetting,
            OptionsSetting,
            EndSetting
        };
        _selectedSetting = 0;
        ShowSelection(_settingsList[_selectedSetting]);
    }

    private void OnEnable()
    {
        _selectedSetting = 0;
        ShowSelection(_settingsList[_selectedSetting]);
    }

    private void OnDisable()
    {
        HideSelection(_settingsList[_selectedSetting]);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            _gm.CurrentStateOfPlayer = EPlayerStates.Idle;
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_settingsList[_selectedSetting] == SaveSetting)
            {
                _sm.SaveGame();
            }
            else if (_settingsList[_selectedSetting] == OptionsSetting)
            {
               
            }
            else if (_settingsList[_selectedSetting] == EndSetting)
            {
                _gm.EndTurn();
                
            }
            _gm.CurrentStateOfPlayer = EPlayerStates.Idle;
        }
        //change selected option
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
           
            HideSelection(_settingsList[_selectedSetting]);

            _selectedSetting = (_selectedSetting - 1 + _settingsList.Count) % _settingsList.Count;

            ShowSelection(_settingsList[_selectedSetting]);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            HideSelection(_settingsList[_selectedSetting]);

            _selectedSetting = (_selectedSetting + 1) % _settingsList.Count;

            ShowSelection(_settingsList[_selectedSetting]);
        }
    }
    #endregion

    #region Methods
    private void ShowSelection(GameObject Setting)
    {
        _settingsList[_selectedSetting].transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().color = Color.white;
        _settingsList[_selectedSetting].transform.GetChild(1).gameObject.SetActive(true);
        _settingsList[_selectedSetting].transform.GetChild(2).gameObject.SetActive(true);
        _settingsList[_selectedSetting].transform.GetChild(3).gameObject.SetActive(false);
        _settingsList[_selectedSetting].transform.GetChild(4).gameObject.SetActive(false);
    }

    private void HideSelection(GameObject Setting)
    {
        _settingsList[_selectedSetting].transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().color = textColor;
        _settingsList[_selectedSetting].transform.GetChild(1).gameObject.SetActive(false);
        _settingsList[_selectedSetting].transform.GetChild(2).gameObject.SetActive(false);
        _settingsList[_selectedSetting].transform.GetChild(3).gameObject.SetActive(true);
        _settingsList[_selectedSetting].transform.GetChild(4).gameObject.SetActive(true);
    }
    #endregion
}
