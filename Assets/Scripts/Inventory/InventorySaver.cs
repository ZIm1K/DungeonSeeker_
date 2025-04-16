using Inventory;
using Objects.Weapon;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class SavedSlotData
{
    public string itemID;
    public int amount;
    public string slotType;
    public int defenseID;
    public int slotIndex; 

    public SavedSlotData(string itemID, int amount, string slotType, int defenseID, int slotIndex)
    {
        this.itemID = itemID;
        this.amount = amount;
        this.slotType = slotType;
        this.defenseID = defenseID;
        this.slotIndex = slotIndex;
    }
}

public class InventorySaver : MonoBehaviour
{
    [SerializeField] private List<InventorySlot> defenseSlots;
    [SerializeField] private List<InventorySlot> charmSlots;
    [SerializeField] private List<InventorySlot> weaponSlots;
    [SerializeField] private List<InventorySlot> defaultSlots;

    [SerializeField] private List<SavedSlotData> savedSlotsData;

    private ISaveManager _saveSystem;

    private void Start()
    {
        //_saveSystem = new BinarySaveSystem();
        //_saveSystem.Save(savedSlotsData);
        //LoadInventory();
    }

    public void SaveInventory()
    {
        savedSlotsData = new List<SavedSlotData>();

        for (int i = 0; i < defenseSlots.Count; i++)
        {
            var slot = defenseSlots[i];
            savedSlotsData.Add(slot.item != null
                ? new SavedSlotData(slot.item.itemID, slot.amount, "Defense", slot.defenseID, i)
                : new SavedSlotData("0", 0, "Defense", 0, i));
        }

        for (int i = 0; i < charmSlots.Count; i++)
        {
            var slot = charmSlots[i];
            savedSlotsData.Add(slot.item != null
                ? new SavedSlotData(slot.item.itemID, slot.amount, "Charm", slot.defenseID, i)
                : new SavedSlotData("0", 0, "Charm", 0, i));
        }

        for (int i = 0; i < weaponSlots.Count; i++)
        {
            var slot = weaponSlots[i];
            savedSlotsData.Add(slot.item != null
                ? new SavedSlotData(slot.item.itemID, slot.amount, "Weapon", slot.defenseID, i)
                : new SavedSlotData("0", 0, "Weapon", 0, i));
        }

        for (int i = 0; i < defaultSlots.Count; i++)
        {
            var slot = defaultSlots[i];
            savedSlotsData.Add(slot.item != null
                ? new SavedSlotData(slot.item.itemID, slot.amount, "Default", slot.defenseID, i)
                : new SavedSlotData("0", 0, "Default", 0, i));
        }

        _saveSystem.Save(savedSlotsData);
    }

    public void LoadInventory()
    {
        _saveSystem = new BinarySaveSystem();
        //_saveSystem.Save(savedSlotsData);
        savedSlotsData = _saveSystem.Load<List<SavedSlotData>>();
        if (savedSlotsData == null) return;
        if (defenseSlots == null || charmSlots == null || weaponSlots == null || defaultSlots == null)
        {
            Debug.LogError("One or more slot lists are null.");
            return;
        }

        ClearSlots(defenseSlots);
        ClearSlots(charmSlots);
        ClearSlots(weaponSlots);
        ClearSlots(defaultSlots);

        foreach (var savedSlot in savedSlotsData)
        {
            if (savedSlot.itemID != "0")
            {
                var item = gameObject.GetComponent<ItemDatabase>().allItems.Find(x => x.itemID == savedSlot.itemID);
                if (item != null)
                {
                    switch (savedSlot.slotType)
                    {
                        case "Defense":
                            AssignItemToSlotAtIndex(defenseSlots, savedSlot.slotIndex, item, savedSlot.amount, savedSlot.defenseID);
                            break;
                        case "Charm":
                            AssignItemToSlotAtIndex(charmSlots, savedSlot.slotIndex, item, savedSlot.amount, savedSlot.defenseID);
                            break;
                        case "Weapon":
                            AssignItemToSlotAtIndex(weaponSlots, savedSlot.slotIndex, item, savedSlot.amount, savedSlot.defenseID);
                            break;
                        case "Default":
                            AssignItemToSlotAtIndex(defaultSlots, savedSlot.slotIndex, item, savedSlot.amount, savedSlot.defenseID);
                            break;
                    }
                }
            }
        }
        savedSlotsData.Clear();
        _saveSystem.Save(savedSlotsData);
    }

    private void AssignItemToSlotAtIndex(List<InventorySlot> slots, int index, ItemScriptableObject item, int amount, int defenseID)
    {
        if (slots == null || index < 0 || index >= slots.Count)
        {
            Debug.LogError($"Slots is null or index {index} is out of range");
            return;
        }

        var slot = slots[index];
        if (slot == null)
        {
            Debug.LogError($"Slot at index {index} is null");
            return;
        }

        if (slot.iconGO == null || slot.itemAmountText == null)
        {
            Debug.LogError("InventorySlot components are not assigned");
            return;
        }

        slot.item = item;
        slot.SetIcon(item.icon);
        slot.amount = amount;
        slot.isEmpty = item == null;
        slot.itemAmountText.text = item != null ? amount.ToString() : "";
        slot.defenseID = defenseID;
        slot.OnSlotItemChanged();
    }

    private void ClearSlots(List<InventorySlot> slots)
    {
        foreach (var slot in slots)
        {
            if (slot == null) continue;

            slot.item = null;
            slot.isEmpty = true;
            if (slot.iconGO != null)
            {
                var image = slot.iconGO.GetComponent<Image>();
                if (image != null)
                {
                    slot.SetBasedIcon();
                    image.color = new Color(1, 1, 1, 1);
                }
            }
            if (slot.itemAmountText != null)
            {
                slot.itemAmountText.text = "";
            }
            slot.defenseID = 0;
        }
    }
}
