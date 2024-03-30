using UnityEngine;

public class UIManager : MonoBehaviour
{
    // This class should handle :
    // The menu (appears after moving or selecting a tile)
    // Info (appears when hovering on a tile):
    // Display tile info and unit info if a unit is on the tile
    // Captain bar
    // Building menu

    private GameManager _gm;
   
    
    [SerializeField] private GameObject _actionMenu;

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
    }

    private void ChangeActiveUI()
    {
        switch (_gm.LastStateOfPlayer)
        {
            case EPlayerStates.InActionsMenu: { _actionMenu.SetActive(false); break; }
            default: { break; }
        }
        switch (_gm.CurrentStateOfPlayer)
        {
            case EPlayerStates.InActionsMenu: {  _actionMenu.SetActive(true); break; }
            default: { break; }
        }
    }
}
