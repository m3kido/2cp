using System;
using System.Collections.Generic;
using UnityEngine;

// This class holds all the map data that will be saved to the file 
[Serializable]
public class SaveMap
{
    // Don't use properties or auto-properties in this class because they're not serializable

    // Map identity
    public int MapID;
    public string MapName;
    public int MaxPlayers;

    // Datas of the map tiles
    public List<TileSaveData> TileSaveDatas;

    public void PrintDebugInfo() // For debug
    {
        Debug.Log("MapID: " + MapID);
        Debug.Log("MapName: " + MapName);
        Debug.Log("MaxPlayers: " + MaxPlayers);
        Debug.Log("TileSaveDatas:");
        foreach (var tileSaveData in TileSaveDatas)
        {
            Debug.Log(" - Terrain type :" + tileSaveData.TerrainType);
            Debug.Log(" - Position : " + tileSaveData.Position);
        }
    }
}
