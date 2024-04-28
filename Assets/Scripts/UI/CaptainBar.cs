using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Class to manage the Captain bar
public class CaptainBar : MonoBehaviour
{
    #region Variables
    [SerializeField] private GameObject _superMeterSprite;
    [SerializeField] private GameObject _goldValue;
    [SerializeField] private Image _captainSprite;
    [SerializeField] private Image _captainColor;

    private GameManager _gm;
    #endregion

    #region UnityMethods
    private void Awake()
    {
        _gm = FindAnyObjectByType<GameManager>();
    }
 
    void Start()
    {
        UpdateCaptain();
        GameManager.OnTurnEnd += UpdateCaptain;
        UnitManager.OnMoveEnd += UpdateSuperMeter;
    }
    #endregion

    #region Methods
    private void UpdateSuperMeter()
    {
        //idk how the captains will be held
        _superMeterSprite.GetComponent<Image>().fillAmount = _gm.Players[_gm.PlayerTurn].SuperMeter;
    }
    public void UpdateCaptain()
    {
        var col = _gm.Players[_gm.PlayerTurn].Color;
        if(col==ETeamColors.Azure)
        {
            _captainColor.GetComponent<Image>().color=Color.blue;
        }
        else if(col==ETeamColors.Amber)
        {
            _captainColor.GetComponent<Image>().color = Color.red;
        }
        else if ( col==ETeamColors.Gilded)
        {
            _captainColor.GetComponent<Image>().color = Color.yellow;
        }
        else if (col == ETeamColors.Verdant)
        {
            _captainColor.GetComponent<Image>().color = Color.green;
        }

        _goldValue.GetComponent<TextMeshProUGUI>().text = _gm.Players[_gm.PlayerTurn].Gold.ToString();
    }
    #endregion
}
