using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

// Class to manage the stats menu
public class StatsMenu : MonoBehaviour
{
    #region Variables
    GameManager _gm;
    UnitManager _um;
    MapManager _mm;
    BuildingManager _bm;
    CursorManager _cm;
    Camera _camera;
    RectTransform _rect;
    RectTransform _unitRect;
    RectTransform _tileRect;

    private Animator _anim;

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

    bool _gameLoaded = false;
    #endregion

    #region UnityMethods
    // Start is called before the first frame update
    void Awake()
    {
        _gm = FindAnyObjectByType<GameManager>();
        _um = FindAnyObjectByType<UnitManager>();
        _mm = FindAnyObjectByType<MapManager>();
        _bm = FindAnyObjectByType<BuildingManager>();
        _cm = FindAnyObjectByType<CursorManager>();
        _camera = Camera.main;
        _rect = GetComponent<RectTransform>();
        _unitRect = _unitStats.GetComponent<RectTransform>();
        _tileRect = _TileStats.GetComponent<RectTransform>();

        _anim = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        CursorManager.OnCursorMove += UpdateInfo;
        if (_gameLoaded)
        {
             UpdateInfo();
            
        }

    }
    private void OnDisable()
    {
        CursorManager.OnCursorMove -= UpdateInfo;
    }
    private void Start()
    {
        UpdateInfo();
        _anim.SetTrigger("Replay");
        _gameLoaded = true;
    }
    #endregion

    #region Methods
    private void UpdateInfo()
    {
        if (_camera.transform.position.x - _cm.transform.position.x >= 0)
        {
            // If the menu is on the left of the screen
            if (_rect.localPosition.x < 0)
            {
                _rect.localPosition = new Vector3(-1 * _rect.localPosition.x + _rect.rect.width, _rect.localPosition.y, _rect.localPosition.z);
                var save = _unitRect.localPosition;
                _unitRect.localPosition = _tileRect.localPosition;
                _tileRect.localPosition = save;
               
            }
        }
        else
        {
            // If the menu is on the right of the screen
            if (_rect.localPosition.x > 0)
            {
                _rect.localPosition = new Vector3(-1 * _rect.localPosition.x + _rect.rect.width, _rect.localPosition.y, _rect.localPosition.z);
                var save = _unitRect.localPosition;
                _unitRect.localPosition = _tileRect.localPosition;
                _tileRect.localPosition = save;
                _anim.SetTrigger("Replay");
            }
        }

        TerrainDataSO TileData = _mm.GetTileData(_cm.HoveredOverTile);
        _tileSprite.GetComponent<Image>().sprite = _mm.Map.GetTile<Tile>(_cm.HoveredOverTile).sprite;
        if (TileData.TerrainType == ETerrains.Building)
        {
            _tileName.GetComponent<TextMeshProUGUI>().text = _bm.BuildingFromPosition[_cm.HoveredOverTile].BuildingType.ToString();
        }
        else
        {
            _tileName.GetComponent<TextMeshProUGUI>().text = TileData.TerrainType.ToString();
        }

        _defenceValue.GetComponent<TextMeshProUGUI>().text = TileData.DefenceStars.ToString();
        Unit RefUnit = _um.FindUnit(_cm.HoveredOverTile);

        if (RefUnit != null)
        {
            _unitStats.gameObject.SetActive(true);
            _unitName.GetComponent<TextMeshProUGUI>().text = RefUnit.Data.UnitType.ToString();
            _unitSprite.GetComponent<Image>().sprite = RefUnit.GetComponent<SpriteRenderer>().sprite;
            _healthValue.GetComponent<TextMeshProUGUI>().text = RefUnit.Health.ToString();
            // _ammoValue.GetComponent<TextMeshPro>().text = RefUnit.ToString();
            _provisionsValue.GetComponent<TextMeshProUGUI>().text = RefUnit.Provisions.ToString();
        }
        else
        {
            _unitStats.gameObject.SetActive(false);
        }
    }
    #endregion
}

