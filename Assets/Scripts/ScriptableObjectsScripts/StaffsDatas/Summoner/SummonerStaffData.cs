using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Summoner Staff", menuName = "Weapon/summon staff", order = 1)]
public class SummonerStaffData : ScriptableObject
{
    public int allyHealAmmount;
    public int allySpeed;
    public int healInterval;
    public int healDistance;
    public int tpDistance;
    public int manaCost;
    public string useSoundPath;
    public GameObject allyPrefab;
    public string pathOfScObj;
}
