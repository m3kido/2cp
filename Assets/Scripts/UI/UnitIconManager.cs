using UnityEngine;

public class UnitIconManager : MonoBehaviour
{
    #region Variables
    private Unit _unit; // Reference to the _unit this script manages
    [SerializeField] private UnitIconDataSO _iconData;

    private GameObject _H; // Health icon
    private GameObject _P; // Provisions need icon
    private GameObject _E; // Energy need icon
    private GameObject _C; // Capturing icon
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
    }
    #endregion

    #region Methods
    // Display initial icons
    public void InitializeIcons()
    {
        Vector3 basePosition = transform.position + new Vector3(0.14f, 0.15f, 0); 

        _H = PutIcon(true, basePosition, _iconData.H);
        _P = PutIcon(false, basePosition + new Vector3(_iconData.C.transform.localScale.x, 0, 0), _iconData.P);

        // Check if it's an attacking unit
        if (_unit is AttackingUnit)
        {
           _E = PutIcon(false, basePosition + new Vector3(2 * _iconData.C.transform.localScale.x, 0, 0), _iconData.E);
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
            _P.SetActive(true);
        }
    }

    // Set the energy need icon active if too few energy
    public void UpdateEnergyNeedIcon()
    {
        if (_unit is AttackingUnit unit)
        {
            if (unit.Weapons[unit.CurrentWeaponIndex].EnergyOrbs <= Mathf.FloorToInt(_unit.Data.MaxProvisions * 0.25f))
            {
                _E.SetActive(true);
            }
        }
    }
    #endregion
}