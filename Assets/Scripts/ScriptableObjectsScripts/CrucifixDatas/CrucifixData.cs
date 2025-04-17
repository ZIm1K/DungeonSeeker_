using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Crucifix", menuName = "Weapon/crucifix", order = 2)]
public class CrucifixData : ScriptableObject
{
    public float rangeOfAttack = 15;
    public int damage;
    public string attackSoundPath;
    public string pathOfScObj;
}
