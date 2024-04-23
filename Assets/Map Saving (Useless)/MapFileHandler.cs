using UnityEngine;
using System;
using System.IO;

// This class is responsible of writing Maps into a file
public class MapFileHandler
{
    private string _mapDataDirPath = "";
    private string _mapDataFileName = "";
  
    // Constructor
    public MapFileHandler(string dataDirPath, string dataFileName)
    {
        _mapDataDirPath = dataDirPath;
        _mapDataFileName = dataFileName;
    }

    // Load map from the file
    public Map Load()
    {
        string fullPath = Path.Combine(_mapDataDirPath, _mapDataFileName);
        Map loadMapData = null;
        if (File.Exists(fullPath))
        {
            try
            {
                // Load the serialized data from the file
                string dataToLoad = "";
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                Debug.Log("Serialized data: " + dataToLoad);

                // Deserialize the data from Json back into a map object
                loadMapData = JsonUtility.FromJson<Map>(dataToLoad);

                loadMapData.PrintDebugInfo();
            }
            catch (Exception e)
            {
                Debug.LogError("Error when trying to load data from file: " + fullPath + "\n" + e);
            }
        }
        return loadMapData;
    }

    // Write a map in the file
    public void Save(Map map)
    {
        string fullPath = Path.Combine(_mapDataDirPath, _mapDataFileName);
        try
        {
            // Create the directory if it doesn't exist
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            Debug.Log("K");
            map.PrintDebugInfo();

            // Serialize the Map object into Json
            string dataToStore = JsonUtility.ToJson(map, true);

            Debug.Log("Serialized data: " + dataToStore);

            // Write the serialized data to the file
            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error when trying to save the map to the file: " + fullPath + "\n" + e);
        }
    }
}
