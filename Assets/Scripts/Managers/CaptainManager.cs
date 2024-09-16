using System.Collections;
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

}
