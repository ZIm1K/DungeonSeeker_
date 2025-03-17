using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Staff", menuName = "Weapon/staff", order = 1)]
public class StaffData : ScriptableObject
{
    public int allyHealAmmount;
    public int allySpeed;
    public int healInterval;
    public int healDistance;
    public int manaCost;
    public string useSoundPath;
    public GameObject allyPrefab;
}
