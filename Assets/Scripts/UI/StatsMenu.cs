using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class StatsMenu : MonoBehaviour
{
    GameManager _gm;
    UnitManager _um;
    MapManager _mm;
    CursorManager _cm;
    Camera _camera;
    RectTransform _rect;

    [SerializeField] private GameObject _unitStats;
    [SerializeField] private GameObject _TileStats;

    [SerializeField] private GameObject _defenceValue;
    [SerializeField] private GameObject _healthValue;
    [SerializeField] private GameObject _ammoValue;
    [SerializeField] private GameObject _provisionsValue;

    [SerializeField] private GameObject _unitName;
    [SerializeField] private GameObject _unitSprite;

    [SerializeField] private GameObject _tileName;
    [SerializeField] private GameObject _tileSprite;

    bool _gameLoaded=false;
    // Start is called before the first frame update
    void Awake()
    {
        _gm = FindAnyObjectByType<GameManager>();
        _um = FindAnyObjectByType<UnitManager>();
        _mm = FindAnyObjectByType<MapManager>();
        _cm = FindAnyObjectByType<CursorManager>();
        _camera= Camera.main;
        _rect = GetComponent<RectTransform>();
        CursorManager.OnCursorMove += UpdateInfo;
    }

    private void OnEnable()
    {
       if(_gameLoaded)
       UpdateInfo();
    }
    private void Start()
    {
        UpdateInfo();
        _gameLoaded = true;
    }
    private void UpdateInfo()
    {
        if (_camera.transform.position.x - _cm.transform.position.x >= 0)
        {
            //if the menu is on the left of the screen
            if (_rect.localPosition.x < 0)
            {
                _rect.localPosition = new Vector3(-1 * _rect.localPosition.x + _rect.rect.width, _rect.localPosition.y, _rect.localPosition.z);
            }
        }
        else
        {
            //if the menu is on the right of the screen
            if (_rect.localPosition.x > 0)
            {
                _rect.localPosition = new Vector3(-1 * _rect.localPosition.x + _rect.rect.width, _rect.localPosition.y, _rect.localPosition.z);
            }
        }
        /*
        TerrainDataSO TileData= _mm.GetTileData(_cm.HoveredOverTile);
        //_tileSprite.GetComponent<Image>().sprite = _mm.Map.GetTile<Tile>(_cm.HoveredOverTile).sprite;
        _tileName.GetComponent<TextMeshProUGUI>().text = TileData.TerrainType.ToString();
        _defenceValue.GetComponent<TextMeshProUGUI>().text = TileData.DefenceStars.ToString();
        Unit RefUnit = _um.FindUnit(_cm.HoveredOverTile);
        if (RefUnit != null)
        {
            _unitStats.gameObject.SetActive(true);
            //_unitSprite.GetComponent<Image>().sprite=RefUnit.GetComponent<SpriteRenderer>().sprite;
            _healthValue.GetComponent<TextMeshPro>().text = "2";//RefUnit.Health.ToString();
            //_ammoValue.GetComponent<TextMeshPro>().text = RefUnit.ToString();
            _provisionsValue.GetComponent<TextMeshPro>().text = RefUnit.Provisions.ToString();
        }
        else
        {
            _unitStats.gameObject.SetActive(false);
        }*/
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
