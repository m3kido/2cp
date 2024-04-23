using System.Collections.Generic;
using UnityEngine;


public class CaptainManager : MonoBehaviour
{
    public static GameManager Gm;
    public static UnitManager Um;
    public static Dictionary<ECaptains, CaptainDataSO> CaptainsDict = new();
    [SerializeField] List<CaptainDataSO> _captainSOList = new();
    public static List<Captain> LivingCaptains = new();
    int _currentCaptain = 0;




    private void Awake()
    {
        Gm = FindObjectOfType<GameManager>();
        Um = FindObjectOfType<UnitManager>();
        for (int i = 0; i < _captainSOList.Count; i++)
        {
            CaptainsDict.Add((ECaptains)i, _captainSOList[i]);
        }

    }

    private void Start()
    {
        
    }

    private void Update()
    {
        _currentCaptain = Gm.PlayerTurn;
    }

    public void ActivateCeleste()
    {
        LivingCaptains[_currentCaptain].EnableCeleste();
    }

    public void DeactivateCeleste()
    {
        LivingCaptains[_currentCaptain].DisableCeleste();
    }

    public static void DeleteCaptain(Captain captain)
    {
        LivingCaptains.Remove(captain);
    }
}
