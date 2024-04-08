using UnityEngine;
using System;
using System.IO;

// This class is responsible of writing the SaveData class into a file
public class FileDataHandler
{
    private string _dataDirPath = "";
    private string _dataFileName = "";
    private bool _useEncryption = false; // For security. Can be disabled
    private readonly string _encryptionCodeWord = "zqI8UqfDdR"; // I chose a random key, even more secure

    // Constructor
    public FileDataHandler(string dataDirPath, string dataFileName, bool useEncryption)
    {
        _dataDirPath = dataDirPath;
        _dataFileName = dataFileName;
        _useEncryption = useEncryption;
    }

    // Load SaveData from the file
    public SaveData Load()
    {
        string fullPath = Path.Combine(_dataDirPath, _dataFileName);
        SaveData loadData = null;
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

                // Decrypt the data (if encrypted)
                if (_useEncryption)
                {
                    dataToLoad = EncryptDecrypt(dataToLoad);
                }

                Debug.Log("Serialized data: " + dataToLoad);

                // Deserialize the data from Json back into a SaveData object
                loadData = JsonUtility.FromJson<SaveData>(dataToLoad);

                loadData.PrintDebugInfo();
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
        string fullPath = Path.Combine(_dataDirPath, _dataFileName);
        try
        {
            // Create the directory if it doesn't exist
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            data.PrintDebugInfo();

            // Serialize the SaveData object into Json
            string dataToStore = JsonUtility.ToJson(data, true);

            // Encrypt the data
            if (_useEncryption)
            {
                dataToStore = EncryptDecrypt(dataToStore);
            }

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
            Debug.LogError("Error when trying to save data to the file: " + fullPath + "\n" + e);
        }
    }

    // Encryption and decryption function (XOR Method)
    private string EncryptDecrypt(string data)
    {
        string modifiedData = "";
        for (int i = 0; i < data.Length; i++)
        {
            modifiedData += (char)(data[i] ^ _encryptionCodeWord[i % _encryptionCodeWord.Length]);
        }
        return modifiedData;
    }
}
