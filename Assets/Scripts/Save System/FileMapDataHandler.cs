using UnityEngine;
using System;
using System.IO;

// This class is responsible of writing the SaveMap class into a file
public class FileMapDataHandler
{
    private string _mapDataDirPath = "";
    private string _mapDataFileName = "";
  
    // Constructor
    public FileMapDataHandler(string dataDirPath, string dataFileName)
    {
        _mapDataDirPath = dataDirPath;
        _mapDataFileName = dataFileName;
    }

    // Load SaveMap from the file
    public SaveMap Load()
    {
        string fullPath = Path.Combine(_mapDataDirPath, _mapDataFileName);
        SaveMap loadMapData = null;
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

                // Deserialize the data from Json back into a SaveMap object
                loadMapData = JsonUtility.FromJson<SaveMap>(dataToLoad);

                loadMapData.PrintDebugInfo();
            }
            catch (Exception e)
            {
                Debug.LogError("Error when trying to load data from file: " + fullPath + "\n" + e);
            }
        }
        return loadMapData;
    }

    // Write SaveMap in the file
    public void Save(SaveMap mapData)
    {
        string fullPath = Path.Combine(_mapDataDirPath, _mapDataFileName);
        try
        {
            // Create the directory if it doesn't exist
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            mapData.PrintDebugInfo();

            // Serialize the SaveMap object into Json
            string dataToStore = JsonUtility.ToJson(mapData, true);

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
