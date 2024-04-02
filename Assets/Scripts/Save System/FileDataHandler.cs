using UnityEngine;
using System;
using System.IO;

// This class is responsible of writing the SaveData class into a file
public class FileDataHandler
{
    private string dataDirPath = "";
    private string dataFileName = "";
    private bool useEncryption = false; // For security. Can be disabled
    private readonly string encryptionCodeWord = "zqI8UqfDdR"; // I chose a random key, even more secure

    // Constructor
    public FileDataHandler(string dataDirPath, string dataFileName, bool useEncryption)
    {
        this.dataDirPath = dataDirPath;
        this.dataFileName = dataFileName;
        this.useEncryption = useEncryption;
    }

    // Load SaveData from the file
    public SaveData Load()
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        SaveData loadData = null;
        if (File.Exists(fullPath))
        {
            try
            {
                // load the serialized data from the file
                string dataToLoad = "";
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                // decrypt the data (optional)
                if (useEncryption)
                {
                    dataToLoad = EncryptDecrypt(dataToLoad);
                }

                // deserialize the data from Json back into a GameData object
                loadData = JsonUtility.FromJson<SaveData>(dataToLoad);
            }
            catch (Exception e)
            {
                Debug.LogError("Error when trying to load data from file: " + fullPath + "\n" + e);
            }
        }
        return loadData;
    }

    // Write SaveData in the file
    public void Save(SaveData data)
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        try
        {
            // Create the directory if it doesn't exist
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            data.PrintDebugInfo();

            // Serialize the C# GameSaveData object into Json
            string dataToStore = JsonUtility.ToJson(data, true);

            Debug.Log("Serialized data: " + dataToStore);

            // Encrypt the data
            if (useEncryption)
            {
                dataToStore = EncryptDecrypt(dataToStore);
            }

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
            Debug.LogError("Error when trying to save data to the file: " + fullPath + "\n" + e);
        }
    }

    // Encryption and decryption function (XOR Method)
    private string EncryptDecrypt(string data)
    {
        string modifiedData = "";
        for (int i = 0; i < data.Length; i++)
        {
            modifiedData += (char)(data[i] ^ encryptionCodeWord[i % encryptionCodeWord.Length]);
        }
        return modifiedData;
    }
}
