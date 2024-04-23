using System;
using System.Collections.Generic;
using UnityEngine;

// This class holds map data 
[Serializable]
public class Map
{
    // Don't use properties or auto-properties in this class because they're not serializable

    // Map properties
    public MapSaveData MapProperties;

    // Datas of the map tiles
    public List<TileSaveData> TileSaveDatas;

    public void PrintDebugInfo() // For debug
    {
        Debug.Log("MapID: " + MapProperties.MapID);
        Debug.Log("MapName: " + MapProperties.MapName);
        Debug.Log("MaxPlayers: " + MapProperties.MaxPlayers);
        Debug.Log("TileSaveDatas:");
        foreach (var tileSaveData in TileSaveDatas)
        {
            Debug.Log(" - Terrain type :" + tileSaveData.TerrainType);
            Debug.Log(" - Position : " + tileSaveData.Position);
        }
    }
}
