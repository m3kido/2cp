using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class CaptainBar : MonoBehaviour
{

    [SerializeField] private GameObject _superMeterSprite;
    [SerializeField] private GameObject _goldValue;
    [SerializeField] private Image _captainSprite;


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
    private void UpdateSuperMeter()
    {
        //idk how the captains will be held
        _superMeterSprite.GetComponent<Image>().fillAmount = _gm.Players[_gm.PlayerTurn].PlayerCaptain.SuperMeter;
    }
    private void UpdateCaptain()
    {
        //_captainSprite.sprite = _gm.Players[_gm.PlayerTurn].PlayerCaptain.FrameSprite;
        if (_goldValue.TryGetComponent<TextMeshProUGUI>(out var textMesh))
        {
            textMesh.text = _gm.Players[_gm.PlayerTurn].Gold.ToString();
        }

        
    }

    
}
