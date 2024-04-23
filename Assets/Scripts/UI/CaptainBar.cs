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
    private void UpdateCaptain()
    {
        //_captainSprite.sprite = _gm.Players[_gm.PlayerTurn].PlayerCaptain.FrameSprite;
        _goldValue.GetComponent<TextMeshProUGUI>().text = _gm.Players[_gm.PlayerTurn].Gold.ToString();
    }
    #endregion
}
