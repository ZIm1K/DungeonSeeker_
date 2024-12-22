using Inventory;
using ScriptableObjects.Weapons;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Pistol Item", menuName = "Inventory/Items/New Pistol Item")]
public class PistolItemData : ItemScriptableObject
{
    public PistolData data;
}
