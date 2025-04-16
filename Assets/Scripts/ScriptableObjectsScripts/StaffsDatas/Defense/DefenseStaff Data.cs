using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Defense Staff", menuName = "Weapon/defense staff", order = 1)]
public class DefenseStaffData : ScriptableObject
{
    public int defenseHealAmmount;
    public int manaCost;
    public string useSoundPath;
    public string pathOfScObj;
    public int useTimeOut;
}
