using Inventory;
using ScriptableObjects.Weapons;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Bow Item", menuName = "Inventory/Items/New Bow Item")]
public class BowItemData : ItemScriptableObject
{
    public BowData data;
    public GameObject usePrefab;
}
