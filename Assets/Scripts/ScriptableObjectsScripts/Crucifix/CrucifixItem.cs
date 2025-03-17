using Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Crucifix Item", menuName = "Inventory/Items/New Crucifix Item")]
public class CrucifixItem : ItemScriptableObject
{
    public CrucifixData data;
    public GameObject usePrefab;
}
