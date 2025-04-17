using Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Defense Staff Item", menuName = "Inventory/Items/New Defense Staff Item")]
public class DefenseStaffItem : ItemScriptableObject
{
    public DefenseStaffData data;
    public GameObject usePrefab;
}
