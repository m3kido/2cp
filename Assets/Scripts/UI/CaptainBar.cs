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
    [SerializeField] private GameObject _superMeterReady;

    [SerializeField] private Sprite AndrewSprite;
    [SerializeField] private Sprite MaximusSprite;
    [SerializeField] private Sprite MelinaSprite;
    [SerializeField] private Sprite GodfretSprite;

    private GameManager _gm;

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
    public void UpdateSuperMeter()
    {
        //idk how the captains will be held
        _superMeterSprite.GetComponent<Image>().fillAmount = _gm.Players[_gm.PlayerTurn].PlayerCaptain.SuperMeter;
        if(_superMeterSprite.GetComponent<Image>().fillAmount == 1)
        {
            _superMeterReady.SetActive(true);
        } 
        else 
        {
            _superMeterReady.SetActive(false);
        }
        
    }
    public void UpdateCaptain()
    {
        var col = _gm.Players[_gm.PlayerTurn].Color;
        if (col == ETeamColors.Azure)
        {
            _captainColor.GetComponent<Image>().color = Color.blue;
        }
        else if (col == ETeamColors.Amber)
        {
            _captainColor.GetComponent<Image>().color = Color.red;
        }
        else if (col == ETeamColors.Gilded)
        {
            _captainColor.GetComponent<Image>().color = Color.yellow;
        }
        else if (col == ETeamColors.Verdant)
        {
            _captainColor.GetComponent<Image>().color = Color.green;
        }
        var cap = _gm.Players[_gm.PlayerTurn].PlayerCaptain;
        if (cap.Data.Name == ECaptains.Andrew)
        {
            _captainSprite.GetComponent<Image>().sprite = AndrewSprite;
        }
        else if (cap.Data.Name == ECaptains.Maximus)
        {
            _captainSprite.GetComponent<Image>().sprite = MaximusSprite;
        }
        else if (cap.Data.Name == ECaptains.Melina)
        {
            _captainSprite.GetComponent<Image>().sprite = MelinaSprite;
        }
        else if (cap.Data.Name == ECaptains.Godfrey)
        {
            _captainSprite.GetComponent<Image>().sprite = GodfretSprite;
        }
        UpdateGold();
        UpdateSuperMeter();
    }
    public void UpdateGold()
    {
        //_captainSprite.sprite = _gm.Players[_gm.PlayerTurn].PlayerCaptain.FrameSprite;
        if (_goldValue.TryGetComponent<TextMeshProUGUI>(out var textMesh))
        {
            textMesh.text = _gm.Players[_gm.PlayerTurn].Gold.ToString();
        }
    }
    #endregion
}