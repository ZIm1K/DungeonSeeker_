using Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ManaRegen Item", menuName = "Inventory/Items/New ManaRegen Item")]
public class ManaRegenItem : ItemScriptableObject
{
    public float manaRegenInterval;
    public float duration;
}
