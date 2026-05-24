using Photon.Pun;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class JsonSaveSystem : ISaveManager
{
    private readonly string _directoryPath;
    private readonly string _filePath;

    public JsonSaveSystem()
    {
        string projectRoot = Path.GetFullPath(Path.Combine(Application.persistentDataPath, ".."));
        _directoryPath = Path.Combine(projectRoot, "JsonSaves");

        if (!Directory.Exists(_directoryPath))
            Directory.CreateDirectory(_directoryPath);

        string playerId = "OfflinePlayer";
        if (PhotonNetwork.LocalPlayer != null)
        {
            // Better to use UserId. If it's not set, fallback to NickName, or a generated Guid.
            if (!string.IsNullOrWhiteSpace(PhotonNetwork.LocalPlayer.UserId))
            {
                playerId = PhotonNetwork.LocalPlayer.UserId;
            }
            else if (!string.IsNullOrWhiteSpace(PhotonNetwork.LocalPlayer.NickName))
            {
                playerId = PhotonNetwork.LocalPlayer.NickName;
            }
        }

        // Just in case UserId is empty string (sometimes happens in offline mode)
        if (string.IsNullOrWhiteSpace(playerId))
        {
            if (!PlayerPrefs.HasKey("LocalPlayerID"))
            {
                PlayerPrefs.SetString("LocalPlayerID", Guid.NewGuid().ToString());
                PlayerPrefs.Save();
            }
            playerId = PlayerPrefs.GetString("LocalPlayerID");
        }

        _filePath = Path.Combine(_directoryPath, "Save_" + SanitizeFileName(playerId) + ".json");
    }

    public T Load<T>()
    {
        if (!File.Exists(_filePath))
        {
            return default;
        }

        try
        {
            string json = File.ReadAllText(_filePath);
            if (!string.IsNullOrWhiteSpace(json))
            {
                SerializationWrapper<T> wrapper = JsonUtility.FromJson<SerializationWrapper<T>>(json);
                return wrapper.data;
            }
        }
        catch (Exception exception)
        {
            Debug.LogWarning($"Failed to load save file: {exception.Message}");
        }

        return default;
    }

    public void Save<T>(T data)
    {
        if (!Directory.Exists(_directoryPath))
        {
            Directory.CreateDirectory(_directoryPath);
        }

        try
        {
            SerializationWrapper<T> wrapper = new SerializationWrapper<T> { data = data };
            string json = JsonUtility.ToJson(wrapper, true);
            File.WriteAllText(_filePath, json);
        }
        catch (Exception exception)
        {
            Debug.LogWarning($"Failed to save file: {exception.Message}");
        }
    }

    private string SanitizeFileName(string fileName)
    {
        foreach (char invalidChar in Path.GetInvalidFileNameChars())
        {
            fileName = fileName.Replace(invalidChar, '_');
        }
        return fileName;
    }

    [Serializable]
    private class SerializationWrapper<TData>
    {
        public TData data;
    }
}
