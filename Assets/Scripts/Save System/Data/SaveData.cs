using System;
using System.Collections.Generic;
using UnityEngine;

// This class holds all the data (units, buildings, players, etc.) that will be saved to the file 
[Serializable]
public class SaveData
{
    // Don't use properties or auto-properties in this class because they're not serializable

    // Data realted to game logic (day count, player turn, etc.)
    public GameSaveData GameLogicSaveData;

    // Datas of all the players that were in the game
    public List<PlayerSaveData> PlayerSaveDatas;

    // Datas of all the attacking units that were present before leaving the game
    public List<AttackingUnitSaveData> AttackingUnitSaveDatas;

    // Datas of all the loading units that were present before leaving the game
    public List<LoadingUnitSaveData> LoadingUnitSaveDatas;

    public void PrintDebugInfo() // For debug
    {
        Debug.Log("Day : " + GameLogicSaveData.Day);
        Debug.Log("PlayerTurn : " + GameLogicSaveData.PlayerTurn);

        Debug.Log("PlayerSaveDatas :");
        foreach (var playerData in PlayerSaveDatas)
        {
            Debug.Log(" - PlayerName : " + playerData.Color);
            Debug.Log(" - PlayerScore : " + playerData.CPCharge);
        }

        Debug.Log("AttackingUnitSaveDatas :");
        foreach (var attackingUnitData in AttackingUnitSaveDatas)
        {
            Debug.Log(" - UnitName : " + attackingUnitData.UnitType);
            Debug.Log(" - UnitHealth : " + attackingUnitData.Health);
        }

        Debug.Log("LoadingUnitSaveDatas :");
        foreach (var loadingUnitData in LoadingUnitSaveDatas)
        {
            Debug.Log(" - UnitName : " + loadingUnitData.UnitType);
            Debug.Log(" - UnitCargo : " + loadingUnitData.Health);
        }
    }
}