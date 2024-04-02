using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
    public int Day;
    public int PlayerTurn;
    public List<PlayerSaveData> PlayerSaveDatas;
    public List<AttackingUnitSaveData> AttackingUnitSaveDatas;
    public List<LoadingUnitSaveData> LoadingUnitSaveDatas;

    public void PrintDebugInfo()
    {
        Debug.Log("Day: " + Day);
        Debug.Log("PlayerTurn: " + PlayerTurn);

        Debug.Log("PlayerSaveDatas:");
        foreach (var playerData in PlayerSaveDatas)
        {
            Debug.Log("  - PlayerName: " + playerData.Color);
            Debug.Log("    PlayerScore: " + playerData.CPCharge);
            // Add more fields if needed
        }

        Debug.Log("AttackingUnitSaveDatas:");
        foreach (var attackingUnitData in AttackingUnitSaveDatas)
        {
            Debug.Log("  - UnitName: " + attackingUnitData.UnitType);
            Debug.Log("    UnitHealth: " + attackingUnitData.Health);
            // Add more fields if needed
        }

        Debug.Log("LoadingUnitSaveDatas:");
        foreach (var loadingUnitData in LoadingUnitSaveDatas)
        {
            Debug.Log("  - UnitName: " + loadingUnitData.UnitType);
            Debug.Log("    UnitCargo: " + loadingUnitData.Health);
            // Add more fields if needed
        }
    }
}