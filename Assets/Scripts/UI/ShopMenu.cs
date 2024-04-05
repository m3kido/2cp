using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopMenu : MonoBehaviour
{
    private CursorManager _cm;
    private GameManager _gm;
    private BuildingManager _bm;

    [SerializeField] private Color32 textColor = new Color32(115, 42, 28, 255);

    [SerializeField] private GameObject _unitsList;
    [SerializeField] private GameObject _unitDetails;

    [SerializeField] private List<Unit> _unitsPrefabs;
    [SerializeField] private GameObject ListElement;

    private Dictionary<GameObject,Unit> _unitElements;
    private int _selectedUnit;

    private void Awake()
    {
        _cm = FindAnyObjectByType<CursorManager>();
        _gm = FindAnyObjectByType<GameManager>();
        _bm = FindAnyObjectByType<BuildingManager>();  
        _unitElements = new();
        foreach (var unit in _unitsPrefabs)
        {
            
            var ListUnit =  Instantiate(ListElement, _unitsList.transform);
            _unitElements.Add(ListUnit,unit);
            ListUnit.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text= unit.Data.UnitType.ToString();
            ListUnit.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = unit.Data.Cost.ToString();
            var ListUnit2 = Instantiate(ListElement, _unitsList.transform);
            _unitElements.Add(ListUnit2, unit);
            ListUnit2.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = unit.Data.UnitType.ToString();
            ListUnit2.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = unit.Data.Cost.ToString();
        }
        
    }
    private void OnEnable()
    {
        _unitDetails.SetActive(false);
        _unitsList.SetActive(false);
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
            if (_gm.Players[_gm.PlayerTurn].Gold >= NewUnit.Data.Cost)
            {
                _bm.SpawnUnit(NewUnit.Data.UnitType, _cm.HoveredOverTile, _gm.PlayerTurn);
                _gm.Players[_gm.PlayerTurn].Gold -= NewUnit.Data.Cost;
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
    private void ShowUI()
    {
        _unitDetails.SetActive(true);
        _unitsList.SetActive(true);
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
                //_unitDetails.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text
                //_unitDetails.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text
                return;
            }
        }
        

    }
}
