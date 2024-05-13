using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

// Class to manage the shop menu
public class ShopMenu : MonoBehaviour
{
    #region Variables
    private CursorManager _cm;
    private GameManager _gm;
    private BuildingManager _bm;
    private MapManager _mm;
    private CaptainBar _bar;

    [SerializeField] private Color32 textColor = new(115, 42, 28, 255);

    [SerializeField] private GameObject _unitsList;
    [SerializeField] private GameObject _unitDetails;

    [SerializeField] private List<Unit> _unitsPrefabs;
    [SerializeField] private GameObject ListElement;

    private Dictionary<GameObject, Unit> _unitElements;
    private int _selectedUnit;
    #endregion

    #region UnityMethods
    private void Awake()
    {
        _cm = FindAnyObjectByType<CursorManager>();
        _gm = FindAnyObjectByType<GameManager>();
        _bm = FindAnyObjectByType<BuildingManager>();  
        _mm = FindAnyObjectByType<MapManager>();
    }

    private void Start()
    {
        _bar = FindAnyObjectByType<CaptainBar>();
    }

    private void OnEnable()
    {
        _unitDetails.SetActive(false);
        _unitsList.SetActive(false);

        _unitElements = new();
        var pos = _cm.HoveredOverTile;
        var data = _bm.BuildingDataFromTile[_mm.Map.GetTile<Tile>(pos)] as SpawnerBuildingDataSO;
        foreach (var unit in _unitsPrefabs)
        {
            if (data.DeployableUnits.Contains(unit.Data.UnitType))
            {
                var ListUnit = Instantiate(ListElement, _unitsList.transform);
                _unitElements.Add(ListUnit, unit);
                ListUnit.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = unit.Data.UnitType.ToString();
                ListUnit.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = unit.Data.Cost.ToString();
            }
        }
    }

    private void OnDisable()
    {
        foreach (var listUnit in _unitElements.Keys)
        {
            Destroy(listUnit);
        }
        _unitElements.Clear();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            _gm.CurrentStateOfPlayer = EPlayerStates.Idle;
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            var NewUnit = _unitElements[_unitElements.Keys.ToList()[_selectedUnit]];
            int realPrice = (int)(NewUnit.Data.Cost * _gm.Players[_gm.PlayerTurn].PlayerCaptain.PriceMultiplier); 
            if (_gm.Players[_gm.PlayerTurn].Gold >= realPrice)
            {
                _bm.SpawnUnit(NewUnit.Data.UnitType, _cm.HoveredOverTile, _gm.PlayerTurn);
                _gm.Players[_gm.PlayerTurn].Gold -= realPrice;
                _bar.UpdateGold();
                _gm.CurrentStateOfPlayer = EPlayerStates.Idle;
            }
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            _unitElements.Keys.ToList()[_selectedUnit].transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = textColor;

            _selectedUnit = (_selectedUnit - 1 + _unitElements.Count) % _unitElements.Count;

            _unitElements.Keys.ToList()[_selectedUnit].transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.red;
            UpdateUI();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            _unitElements.Keys.ToList()[_selectedUnit].transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = textColor;

            _selectedUnit = (_selectedUnit + 1 ) % _unitElements.Count;

            _unitElements.Keys.ToList()[_selectedUnit].transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.red;
            UpdateUI();
        }
    }
    #endregion

    #region Methods
    private void ShowUI()
    {
        _unitDetails.SetActive(true);
        _unitsList.SetActive(true);
        _unitElements.Keys.ToList()[_selectedUnit].transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = textColor;
        _selectedUnit = 0;
        _unitElements.Keys.ToList()[_selectedUnit].transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.red;
        UpdateUI();
    }

    private void UpdateUI()
    {
        foreach(var unit in _unitsPrefabs)
        {
            if (_unitElements[_unitElements.Keys.ToList()[_selectedUnit]] == unit)
            {
                _unitDetails.transform.GetChild(1).GetComponent<Image>().sprite = unit.GetComponent<SpriteRenderer>().sprite;
                _unitDetails.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text=unit.Data.MoveRange.ToString();
                // _unitDetails.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text
                // _unitDetails.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text
                return;
            }
        }
    }
    #endregion
}
