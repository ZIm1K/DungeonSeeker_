using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelHandler : MonoBehaviour
{
    public static int Level
    {
        get => PlayerPrefs.GetInt("PlayerLevel", 1);
        set => PlayerPrefs.SetInt("PlayerLevel", value);        
    }

    public static void IncreaseLevel()
    {
        Level += 1;
    }

    public static void ResetLevel()
    {
        PlayerPrefs.SetInt("PlayerLevel", 1);
    }
}
