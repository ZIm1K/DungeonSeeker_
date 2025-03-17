using Inventory;
using ScriptableObjects.Weapons;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Staff Item", menuName = "Inventory/Items/New Staff Item")]
public class StaffItem : ItemScriptableObject
{
    public StaffData data;
    public GameObject usePrefab;
}
