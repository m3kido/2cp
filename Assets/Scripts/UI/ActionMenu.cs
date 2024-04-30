using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Class to manage action selection menu
public class ActionMenu : MonoBehaviour
{
    #region Variables
    // Managers will be needed
    private CursorManager _cm;
    private GameManager _gm;
    private UnitManager _um;
    private BuildingManager _bm;
    private AttackManager _am;
    private Camera _camera;
    private RectTransform _rect;

    [SerializeField] private Sprite _cursor;
    [SerializeField] private GameObject _options;
    private List<GameObject> _optionsList;
    private int _selectedOption;

    [SerializeField] private GameObject _waitOption;
    [SerializeField] private GameObject _captureOption;
    [SerializeField] private GameObject _attackOption;
    [SerializeField] private GameObject _loadOption;
    [SerializeField] private GameObject _dropOption;
    [SerializeField] private GameObject _refillOption;
    // public GameObject FireOption;

    private GameObject _waitOptionInstance;
    private GameObject _captureOptionInstance;
    private GameObject _attackOptionInstance;
    private GameObject _loadOptionInstance;
    private GameObject _dropOptionInstance;
    private GameObject _refillOptionInstance;
    #endregion

    #region UnityMethods
    private void Awake()
    {
        _cm = FindAnyObjectByType<CursorManager>();
        _um = FindAnyObjectByType<UnitManager>();
        _gm = FindAnyObjectByType<GameManager>();
        _bm = FindAnyObjectByType<BuildingManager>();
        _am = FindAnyObjectByType<AttackManager>();
        _camera = Camera.main;
        _rect = GetComponent<RectTransform>();

        _optionsList = new List<GameObject>();

        _waitOptionInstance = Instantiate(_waitOption, _options.transform);
        _waitOptionInstance.SetActive(false);

        _captureOptionInstance = Instantiate(_captureOption, _options.transform);
        _captureOptionInstance.SetActive(false);

        _attackOptionInstance = Instantiate(_attackOption, _options.transform);
        _attackOptionInstance.SetActive(false);

        _loadOptionInstance = Instantiate(_loadOption, _options.transform);
        _loadOptionInstance.SetActive(false);

        _dropOptionInstance = Instantiate(_dropOption, _options.transform);
        _dropOptionInstance.SetActive(false);

        _refillOptionInstance = Instantiate(_refillOption, _options.transform);
        _refillOptionInstance.SetActive(false);
    }

    private void OnEnable()
    {
        if (_gm.CurrentStateOfPlayer != EPlayerStates.InActionsMenu) { return; }
        //this will be reusable
        //this changes the location of the menu based on the cursor position
        //local position returns the position considiring its parent(canvas) as the reference
        //im adding the width because the pivot of action menu is on its top right
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
        CalculateOptions();
    }

    private void OnDisable()
    {
        if (_optionsList.Count == 0) { return; }
        _optionsList[_selectedOption].transform.GetChild(0).gameObject.SetActive(false);
        foreach (GameObject option in _optionsList) { option.SetActive(false); }
        _optionsList.Clear();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            _gm.CurrentStateOfPlayer = EPlayerStates.Selecting;
            _um.SelectedUnit.transform.position = _cm.SaveTile;
            
            if (_um.Path.Count != 0)
            {
                _cm.HoveredOverTile = _um.Path.Last();
            }
            _um.SelectUnit(_um.SelectedUnit);
        }

        else if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_optionsList[_selectedOption] == _waitOptionInstance)
            {
                _um.EndMove();
                _gm.CurrentStateOfPlayer = EPlayerStates.Idle;
            }
            else if (_optionsList[_selectedOption] == _attackOptionInstance)
            {

                if (_um.SelectedUnit is AttackingUnit)
                {
                    _am.Attacker = _um.SelectedUnit as AttackingUnit;

                    Debug.Log("We're attacking");
                    _am.InitiateAttack();
                    Debug.Log("Done attacking");
                }
            }
            else if (_optionsList[_selectedOption] == _captureOptionInstance)
            {
                _bm.CaptureBuilding(_cm.HoveredOverTile);
                _um.EndMove();
                
            }
            else if (_optionsList[_selectedOption] == _captureOptionInstance)
            {
                _bm.CaptureBuilding(_cm.HoveredOverTile);
                _um.EndMove();

            }
            else if (_optionsList[_selectedOption] == _loadOptionInstance)
            {
                (_um.FindUnit(_um.SelectedUnit.GetGridPosition()) as LoadingUnit).LoadUnit(_um.SelectedUnit);
                _um.EndMove();

            }
            else if (_optionsList[_selectedOption] == _dropOptionInstance)
            {
                (_um.SelectedUnit as LoadingUnit).InisitateDropUnit();
                

            }
        }
        //change selected option
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            //getting the cursor image and  hiding it
            _optionsList[_selectedOption].transform.GetChild(0).gameObject.SetActive(false);

            _selectedOption = (_selectedOption - 1 + _optionsList.Count) % _optionsList.Count;

            //getting the cursor image and  showing it
            _optionsList[_selectedOption].transform.GetChild(0).gameObject.SetActive(true);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            _optionsList[_selectedOption].transform.GetChild(0).gameObject.SetActive(false);

            _selectedOption = (_selectedOption + 1) % _optionsList.Count;

            _optionsList[_selectedOption].transform.GetChild(0).gameObject.SetActive(true);
        }
    }
    #endregion

    #region Methods
    private void CalculateOptions()
    {
        if (_um.FindUnit(_um.SelectedUnit.GetGridPosition()))
        {
            _loadOptionInstance.SetActive(true);
            _optionsList.Add(_loadOptionInstance);

        }
        else
        {
            CheckFire();// if is an attacking unit

            CheckDrop();
            CheckAbility();
            
        }
        
       
        _selectedOption = 0;
        _optionsList[_selectedOption].transform.GetChild(0).gameObject.SetActive(true);
    }

     private void CheckFire()
      {
          if (_um.SelectedUnit is AttackingUnit && _am.CheckAttack(_um.SelectedUnit as AttackingUnit))
          {
            if((_um.SelectedUnit as AttackingUnit).IndirectUnit && _um.Path.Count == 0)
            {
                _attackOptionInstance.SetActive(true);
                _optionsList.Add(_attackOptionInstance);
                return;
            }
           
        }
      } 
    private void CheckDrop()
    {
        if (_um.SelectedUnit is LoadingUnit && (_um.SelectedUnit as LoadingUnit).LoadedUnit != null&& (_um.SelectedUnit as LoadingUnit).GetDropTiles() )
        {
            _dropOptionInstance.SetActive(true);
            _optionsList.Add(_dropOptionInstance);
            return;
        }
    }
    private void CheckAbility()
    {
        var building = _bm.BuildingFromPosition.ContainsKey(_cm.HoveredOverTile) ? _bm.BuildingFromPosition[_cm.HoveredOverTile] : null;
        if (building != null)
        {
            if ( building.Owner != _gm.PlayerTurn)
            {
                if (_um.SelectedUnit.Data.UnitType == EUnits.Infantry || _um.SelectedUnit.Data.UnitType == EUnits.Lancers)
                {
                    _captureOptionInstance.SetActive(true);
                    _optionsList.Add(_captureOptionInstance);
                    return;
                }
            }
            else
            {
                //heal
            }
        }
       
        _waitOptionInstance.SetActive(true);
        _optionsList.Add(_waitOptionInstance);
    }
    #endregion
}
