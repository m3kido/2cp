using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CaptainManager : MonoBehaviour
{
    public static GameManager Gm;
    public static UnitManager Um;
    public static CaptainBar Cp;
    public static Dictionary<ECaptains, CaptainDataSO> CaptainsDict = new();
    public GameObject HealSprite;
    public GameObject EnergySprite;
    [SerializeField] List<CaptainDataSO> _captainSOList = new();

    public static List<Captain> LivingCaptains = new();
    int _currentCaptain = 0;

    public static CaptainManager Instance
    {
        get { return FindObjectOfType<CaptainManager>(); }
        private set { }
    }


    private void Awake()
    {
        Gm = FindObjectOfType<GameManager>();
        Um = FindObjectOfType<UnitManager>();
        Cp = FindObjectOfType<CaptainBar>();
        for (int i = 0; i < _captainSOList.Count; i++)
        {
            CaptainsDict.Add((ECaptains)i, _captainSOList[i]);
        }
        print(CaptainsDict);

    }

    private void Start()
    {

    }

    private void Update()
    {
        _currentCaptain = Gm.PlayerTurn;
        if (Input.GetKeyDown(KeyCode.S) && Gm.CurrentStateOfPlayer == EPlayerStates.Idle)
        {

            ActivateCeleste();

        }
    }

    public void ActivateCeleste()
    {
        Captain captain = LivingCaptains[_currentCaptain];
        captain.EnableCeleste(); Debug.Log("MIMI");
        if (captain is Melina)
        {
            Debug.Log("MELINAA");
            Melina melina = captain as Melina;
            StartCoroutine(melina.ReviveSelectionCoroutine(Gm));
        }
        Cp.UpdateSuperMeter();
    }

    public void DeactivateCeleste()
    {
        LivingCaptains[_currentCaptain].DisableCeleste();
    }

    public static void DeleteCaptain(Captain captain)
    {
        captain.UnsubscribeWhenDestroyed();
        LivingCaptains.Remove(captain);
    }

    public void StartCoroutineWithMethod(IEnumerator method)
    {
        if (method != null)
        {
            StartCoroutine(method);
        }

    }
    public void HealSpr()
    {
        foreach (var unit in Um.Units)
        {
            if (Gm.Players[unit.Owner] == Gm.Players[Gm.PlayerTurn] && unit.Health < 100)
            {
                Instantiate(HealSprite, unit.transform);

            }
        }

    }

    public void HealSpr(Unit unit)
    {
        if (unit.Health < 100)
        {
            Instantiate(HealSprite, unit.transform);
        }
    }

    public void EnergySpr(Unit unit)
    {
        
        Instantiate(EnergySprite, unit.transform);

    }

}
