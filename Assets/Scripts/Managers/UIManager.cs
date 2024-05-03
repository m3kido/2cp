using UnityEngine;

// Class to manage UI
public class UIManager : MonoBehaviour
{
    // This class should handle :
    // The menu (appears after moving or selecting a tile)
    // Info (appears when hovering on a tile):
    // Display tile info and _unit info if a _unit is on the tile
    // Captain bar
    // Building menu

    #region Variables
    private GameManager _gm;
   
    [SerializeField] private GameObject _actionMenu;

    [SerializeField] private GameObject _settingMenu;

    [SerializeField] private GameObject _statMenu;

    [SerializeField] private GameObject _CaptainsBar;

    [SerializeField] private GameObject _ShopMenu;
    #endregion

    #region UnityMethods
    private void OnEnable()
    {
        GameManager.OnStateChange += ChangeActiveUI;
    }

    private void OnDisable()
    {
        GameManager.OnStateChange -= ChangeActiveUI;
    }

    void Awake()
    {
        _gm = FindAnyObjectByType<GameManager>();
    }

    private void Start()
    {
        _actionMenu.SetActive(false);
        _settingMenu.SetActive(false);
    }
    #endregion

    #region Methods
    private void ChangeActiveUI()
    {
        switch (_gm.LastStateOfPlayer)
        {
            case EPlayerStates.InActionsMenu: { _actionMenu.SetActive(false); break; }
            case EPlayerStates.InSettingsMenu: { _settingMenu.SetActive(false); break; }
            case EPlayerStates.Idle: { _statMenu.SetActive(false); _CaptainsBar.SetActive(false); break; }
            case EPlayerStates.InBuildingMenu: { _ShopMenu.SetActive(false); _CaptainsBar.SetActive(false); break; }
            default: { break; }
        }
        switch (_gm.CurrentStateOfPlayer)
        {
            case EPlayerStates.InActionsMenu: {  _actionMenu.SetActive(true); break; }
            case EPlayerStates.InSettingsMenu: { _settingMenu.SetActive(true); break; }
            case EPlayerStates.Idle: { _statMenu.SetActive(true); _CaptainsBar.SetActive(true); break; }
            case EPlayerStates.InBuildingMenu: { _ShopMenu.SetActive(true); _CaptainsBar.SetActive(true); break; }
            default: { break; }
        }
    }
    #endregion
}
