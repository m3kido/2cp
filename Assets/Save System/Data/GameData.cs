using System;
using System.Collections.Generic;
using UnityEngine;

// This class holds all the data (units, buildings, players, etc.) that will be saved to the file 
[Serializable]
public class GameData
{
    // Don't use properties or auto-properties in this class because they're not serializable

    // Data related to game logic (day count, player turn, etc.)
    public GameSaveData GameLogicSave;

    // All of the players that were in the game
    public List<PlayerSaveData> PlayerSaves;

    // All of the attacking units that were present before leaving the game
    public List<AttackingUnitSaveData> AttackingUnitSaves;

    // All of the loading units that were present before leaving the game
    public List<LoadingUnitSaveData> LoadingUnitSaves;

    // All states of the buildings before leaving the game
    public List<BuildingSaveData> BuildingSaves;

    public void PrintDebugInfo() // For debug
    {
        Debug.Log("Day : " + GameLogicSave.Day);
        Debug.Log("PlayerTurn : " + GameLogicSave.PlayerTurn);

        Debug.Log("PlayerSaveDatas :");
        foreach (var playerData in PlayerSaves)
        {
            Debug.Log(" - PlayerColor : " + playerData.Color);
            Debug.Log(" - PlayerID : " + playerData.ID);
        }

        Debug.Log("AttackingUnitSaveDatas :");
        foreach (var attackingUnitData in AttackingUnitSaves)
        {
            Debug.Log(" - UnitName : " + attackingUnitData.UnitType);
            Debug.Log(" - UnitHealth : " + attackingUnitData.Health);
        }

        Debug.Log("LoadingUnitSaveDatas :");
        foreach (var loadingUnitData in LoadingUnitSaves)
        {
            Debug.Log(" - UnitName : " + loadingUnitData.UnitType);
            Debug.Log(" - UnitCargo : " + loadingUnitData.Health);
        }

        Debug.Log("BuildingSaveDatas :");
        foreach (var BuildingData in BuildingSaves)
        {
            Debug.Log(" - Owner : " + BuildingData.Owner);
            Debug.Log(" - Capture : " + BuildingData.Health);
        }
    }
}