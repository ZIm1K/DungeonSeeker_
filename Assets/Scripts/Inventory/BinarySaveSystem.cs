using Photon.Pun;
using System;
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

        string playerName = PhotonNetwork.LocalPlayer != null && !string.IsNullOrWhiteSpace(PhotonNetwork.LocalPlayer.NickName)
            ? PhotonNetwork.LocalPlayer.NickName
            : "OfflinePlayer";
        _filePath = Path.Combine(_directoryPath, "BinarySave_" + SanitizeFileName(playerName) + ".dat");

    }
    public T Load<T>()
    {
        if (!File.Exists(_filePath))
        {
            return default;
        }

        T saveData;

        try
        {
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
        }
        catch (Exception exception)
        {
            Debug.LogWarning($"Failed to load save file: {exception.Message}");
            return default;
        }

        return saveData;
    }

    private string SanitizeFileName(string fileName)
    {
        foreach (char invalidChar in Path.GetInvalidFileNameChars())
        {
            fileName = fileName.Replace(invalidChar, '_');
        }

        return fileName;
    }

    public void Save<T>(T data)
    {
        if (!Directory.Exists(_directoryPath)) 
        {
            Directory.CreateDirectory(_directoryPath);
        }

        try
        {
            using (FileStream file = File.Create(_filePath))
            {
                new BinaryFormatter().Serialize(file, data);
            }
        }
        catch (Exception exception)
        {
            Debug.LogWarning($"Failed to save file: {exception.Message}");
        }
    }
}
