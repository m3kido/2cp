using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CaptainManager : MonoBehaviour
{
    #region Variables
    public static GameManager Gm;
    public static UnitManager Um;
    public static CaptainBar Cb;
    public static Dictionary<ECaptains, CaptainDataSO> CaptainsDict = new();
    public GameObject HealSprite;
    public GameObject EnergySprite;
    [SerializeField] List<CaptainDataSO> _captainSOList = new();

    public static List<Captain> LivingCaptains = new();
    public int CurrentCaptain { get { return Gm.PlayerTurn; } private set { } }

    public static CaptainManager Instance
    {
        get { return FindObjectOfType<CaptainManager>(); }
        private set { }
    }
    #endregion

    #region UnityMethods
    private void Awake()
    {
        Gm = FindObjectOfType<GameManager>();
        Um = FindObjectOfType<UnitManager>();
        Cb = FindObjectOfType<CaptainBar>();
        for (int i = 0; i < _captainSOList.Count; i++)
        {
            CaptainsDict.Add((ECaptains)i, _captainSOList[i]);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S) && Gm.CurrentStateOfPlayer == EPlayerStates.Idle)
        {
            ActivateCeleste();
        }
    }
    #endregion

    #region Methods
    public void ActivateCeleste()
    {
        Captain captain = LivingCaptains[CurrentCaptain];

        captain.EnableCeleste();
        if (captain is Melina)
        {
            Melina melina = captain as Melina;
            StartCoroutine(melina.ReviveSelectionCoroutine(Gm));
        }
        Cb.UpdateSuperMeter();
    }

    public void DeactivateCeleste()
    {
        LivingCaptains[CurrentCaptain].DisableCeleste();
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
    #endregion
}
