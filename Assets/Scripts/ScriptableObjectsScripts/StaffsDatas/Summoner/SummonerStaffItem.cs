using Inventory;
using ScriptableObjects.Weapons;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Summoner Staff Item", menuName = "Inventory/Items/New Summoner Staff Item")]
public class SummonerStaffItem : ItemScriptableObject
{
    public SummonerStaffData data;
    public GameObject usePrefab;
}
