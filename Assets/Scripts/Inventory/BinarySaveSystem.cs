using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class BinarySaveSystem : ISaveManager
{
    private readonly string _filePath;
    public BinarySaveSystem()
    {
        _filePath = Application.persistentDataPath + "/BinarySave.dat";
    }
    public T Load<T>()
    {
        T saveData;

        using (FileStream file = File.Open(_filePath, FileMode.Open))
        {
            object loadedData = new BinaryFormatter().Deserialize(file);
            saveData = (T)loadedData;
        }
        return saveData;
    }

    public void Save<T>(T data)
    {
        using (FileStream file = File.Create(_filePath))
        {
            new BinaryFormatter().Serialize(file, data);
        }
    }
}
