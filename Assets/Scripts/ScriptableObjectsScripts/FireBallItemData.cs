using Inventory;
using ScriptableObjects.Weapons;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FireBall Item", menuName = "Inventory/Items/New FireBall Item")]
public class FireBallItemData : ItemScriptableObject
{
    public FireballData data;
}
