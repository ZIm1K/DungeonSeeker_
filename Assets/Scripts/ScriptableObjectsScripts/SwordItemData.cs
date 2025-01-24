using Inventory;
using ScriptableObjects.Weapons;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Sword Item", menuName = "Inventory/Items/New Sword Item")]
public class SwordItemData : ItemScriptableObject
{
    public SwordData data;
}
