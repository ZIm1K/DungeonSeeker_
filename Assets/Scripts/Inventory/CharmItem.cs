using Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Charm Item", menuName = "Inventory/Items/New Charm Item")]
public class CharmItem : ItemScriptableObject
{
    public float buffSpeed;
    public float buffJumpForce;
    private void Start()
    {
        itemType = ItemType.Charm;
    }
}
