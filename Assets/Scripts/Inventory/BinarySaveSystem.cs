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

    }
    public T Load<T>()
    {
        if (!File.Exists(_filePath))
        {
            return default;
        }

        T saveData;

        using (FileStream file = File.Open(_filePath, FileMode.Open))
        {
            if (file.Length > 0)
            {
                object loadedData = new BinaryFormatter().Deserialize(file);
                saveData = (T)loadedData;
            }
            else
            {
                return default;
            }
        }

        DeleteFile();

        return saveData;
    }
    private void DeleteFile() 
    {
        if (File.Exists(_filePath))
        {
            File.Delete(_filePath);
        }
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
