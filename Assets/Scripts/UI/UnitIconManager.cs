using UnityEngine;

// Script that display icons near the units
public class UnitIconManager : MonoBehaviour
{
    #region Variables
    private Unit _unit; // Reference to the _unit this script manages
    [SerializeField] private UnitIconDataSO _iconData;

    private GameObject _H; // Health icon
    private GameObject _P; // Provisions need icon
    private GameObject _E; // Energy need icon
    private GameObject _C; // Capturing icon
    private GameObject _L; // Loading icon

    private float _blinkTimer;
    private bool _isEActive = false;
    private bool _isPActive = false;
    #endregion

    #region UnityMethods
    private void Start()
    {
        _unit = GetComponent<Unit>();

        InitializeIcons();
    }

    private void Update()
    {
        UpdateHealthIcon();
        UpdateProvisionsNeedIcon();
        UpdateEnergyNeedIcon();
        UpdateLoadingIcon();
        
        if (_isPActive) // Make warning icons blink
        {
            MakeBlink(_P);
        }

        if (_isEActive)
        {
            MakeBlink(_E);
        }
    }
    #endregion

    #region Methods
    // Make icons blink
    public void MakeBlink(GameObject icon)
    {
        _blinkTimer += Time.deltaTime;
        if (_blinkTimer >= 0.5f)
        {
            // Toggle visibility of the icon
            icon.SetActive(!icon.activeSelf);

            // Reset the timer
            _blinkTimer = 0f;
        }
    }

    // Display initial icons
    public void InitializeIcons()
    {
        Vector3 basePosition = transform.position + new Vector3(0.18f, 0.18f, 0); 
        
        _H = PutIcon(true, basePosition, _iconData.H); // H icon
        _P = PutIcon(false, basePosition + new Vector3(0.33f, -0.05f, 0), _iconData.P); // P icon

        // Check if it's an attacking unit
        if (_unit is AttackingUnit)
        {
            // E icon
           _E = PutIcon(false, basePosition + new Vector3(-0.05f, 0.33f, -0), _iconData.E);
        }

        // Check if it's a loading unit
        if (_unit is LoadingUnit)
        {
            // L icon
            _L = PutIcon(false, basePosition + new Vector3(-0.05f, 0.33f, -0), _iconData.L);
        }
    }

    // create and position an icon 
    public GameObject PutIcon(bool isActive, Vector3 position, GameObject iconPrefab)
    {
        GameObject iconInstance;
        if (transform.Find(iconPrefab.name) != null)
        {
            iconInstance = transform.Find(iconPrefab.name).gameObject;
        }
        else
        {
            iconInstance = Instantiate(iconPrefab, position, Quaternion.identity, transform);
        }

        iconInstance.transform.position = position;
        iconInstance.SetActive(isActive);

        return iconInstance;
    }

    // Change health icon sprite based on unit's health
    public void UpdateHealthIcon()
    {
        _H.GetComponent<SpriteRenderer>().sprite = _iconData.HealthIcons[Mathf.FloorToInt(_unit.Health / 10)];
    }

    // Set the provisions need icon active if too few provisions
    public void UpdateProvisionsNeedIcon()
    {
        if (_unit.Provisions <= Mathf.FloorToInt(_unit.Data.MaxProvisions * 0.25f))
        {
            if (!_isPActive)
            {
                _P.SetActive(true);
                _isPActive = true;
            }
        }
        else
        {
            _P.SetActive(false);
            _isPActive = false;
        }
    }

    // Set the energy need icon active if too few energy
    public void UpdateEnergyNeedIcon()
    {
        if (_unit is AttackingUnit unit)
        {
            if (unit.Weapons[unit.CurrentWeaponIndex].EnergyOrbs <= Mathf.FloorToInt(_unit.Data.MaxProvisions * 0.25f))
            {
                if (!_isEActive)
                {
                    _E.SetActive(true);
                    _isEActive = true;
                }
            }
            else
            {
                _E.SetActive(false);
                _isEActive = false;
            }
        }
    }
    
    // Set the loading icon active if a unit is being loaded
    public void UpdateLoadingIcon()
    {
        if (_unit is LoadingUnit unit)
        {
            if (unit.LoadedUnit != null)
            {
                _L.SetActive(true);
            }
            else
            {
                _L.SetActive(false);
            }
        } 
    }
    #endregion
}