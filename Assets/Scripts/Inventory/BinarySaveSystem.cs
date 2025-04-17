using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class BinarySaveSystem : ISaveManager
{
    private readonly string _directoryPath;
    private readonly string _filePath;
    public BinarySaveSystem()
    {
        string projectRoot = Path.GetFullPath(Path.Combine(Application.persistentDataPath, ".."));
        _directoryPath = Path.Combine(projectRoot, "BinarySaves");

        if (!Directory.Exists(_directoryPath))
            Directory.CreateDirectory(_directoryPath);

        _filePath = Path.Combine(_directoryPath, "BinarySave_" + PhotonNetwork.LocalPlayer.NickName + ".dat");

        ////_filePath = Application.persistentDataPath + "/BinarySave" + PhotonNetwork.LocalPlayer.ActorNumber + ".dat";
    }
    public T Load<T>()
    {
        Debug.LogWarning("Loading From Binary Inv");
        T saveData;

        Debug.LogWarning(_filePath);
        if (!File.Exists(_filePath)) 
        {
            File.Create(_filePath).Close();
        }

        using (FileStream file = File.Open(_filePath, FileMode.Open))
        {
            if (file.Length > 0) 
            {
                object loadedData = new BinaryFormatter().Deserialize(file);
                saveData = (T)loadedData;
                Debug.LogWarning(file.Name);
                Debug.LogWarning("Got data");
                return saveData;
            }        
        }
        return default;

        //if (!File.Exists(_filePath))
        //    return default;

        //try
        //{
        //    using (FileStream file = File.Open(_filePath, FileMode.Open))
        //    {
        //        object loadedData = new BinaryFormatter().Deserialize(file);
        //        return (T)lo
        //        adedData;
        //    }
        //}
        //catch (IOException ex)
        //{
        //    Debug.LogError("File access error: " + ex.Message);
        //    return default;
        //}
    }

    public void Save<T>(T data)
    {
        if (!Directory.Exists(_directoryPath)) 
        {
            Directory.CreateDirectory(_directoryPath);
        }

        using (FileStream file = File.Create(_filePath))
        {
            new BinaryFormatter().Serialize(file, data);
        }
    }
}
