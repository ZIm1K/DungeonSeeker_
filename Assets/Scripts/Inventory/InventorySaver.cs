using Inventory;
using Objects.Weapon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
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

    [SerializeField] private GameObject loadingPanel;

    private ISaveManager _saveSystem;

    private async void Start()
    {
        loadingPanel.transform.GetChild(0).gameObject.SetActive(true);
        await LoadInventoryOnLoad();
        //if (!PlayerPrefs.HasKey("Id"))
        //{
        //    PlayerPrefs.SetString("Id", PhotonNetwork.LocalPlayer.UserId);
        //    PlayerPrefs.Save();
        //}

        //userId = PlayerPrefs.GetString("Id"); 
        LoadInventory();
    }
    async Task LoadInventoryOnLoad() 
    {
        while (!PhotonNetwork.InRoom && PhotonNetwork.LocalPlayer?.ActorNumber != 0)  //Load in on full load
        {
            await Task.Yield();
        }
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

    public async void LoadInventory()
    {
        _saveSystem = new BinarySaveSystem();
        savedSlotsData = _saveSystem.Load<List<SavedSlotData>>();
        if (savedSlotsData == default) 
        {
            loadingPanel.transform.GetChild(0).gameObject.SetActive(false);
            gameObject.GetComponent<InventoryManager>().InvLoaded();
            return;
        }
        if (defenseSlots == null || charmSlots == null || weaponSlots == null || defaultSlots == null)
        {
            Debug.LogError("One or more slot lists are null.");
            return;
        }
        Debug.LogWarning("Not returned");
        ClearSlots(defenseSlots);
        ClearSlots(charmSlots);
        ClearSlots(weaponSlots);
        ClearSlots(defaultSlots);
        Debug.LogWarning("Start loading");
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
                            await defenseSlots[savedSlot.slotIndex].gameObject.transform.GetChild(0)
                                .GetComponent<DragAndDropItem>().LoadWearItem(item, savedSlot.defenseID);
                            //AssignItemToSlotAtIndex(defenseSlots, savedSlot.slotIndex, item, savedSlot.amount, savedSlot.defenseID);
                            break;
                        case "Charm":
                            AssignItemToSlotAtIndex(charmSlots, savedSlot.slotIndex, item, savedSlot.amount, savedSlot.defenseID);
                            await charmSlots[savedSlot.slotIndex].gameObject.transform.GetChild(0)
                                .GetComponent<DragAndDropItem>().LoadWearItem(item, savedSlot.defenseID);
                            break;
                        case "Weapon":
                            AssignItemToSlotAtIndex(weaponSlots, savedSlot.slotIndex, item, savedSlot.amount, savedSlot.defenseID);
                            if (savedSlot.slotIndex == 0)
                            {
                                gameObject.GetComponent<WeaponManager>().OnKeyDown1();
                            }
                            else if (savedSlot.slotIndex == 1)
                            {
                                gameObject.GetComponent<WeaponManager>().OnKeyDown2();
                            }
                            break;
                        case "Default":
                            AssignItemToSlotAtIndex(defaultSlots, savedSlot.slotIndex, item, savedSlot.amount, savedSlot.defenseID);
                            break;
                    }
                    Debug.LogWarning("you loaded something");
                }
            }
        }
        //savedSlotsData.Clear();
        //_saveSystem.Save(savedSlotsData);
        //_saveSystem.DeleteFile();
        //Debug.LogWarning("Cleared");
        loadingPanel.transform.GetChild(0).gameObject.SetActive(false);
        gameObject.GetComponent<InventoryManager>().InvLoaded();
        //Debug.LogWarning("End load");
    }

    private void AssignItemToSlotAtIndex(List<InventorySlot> slots, int index, ItemScriptableObject item, int amount, int defenseID)
    {
        if (slots == null || index < 0 || index >= slots.Count)
        {
            Debug.LogWarning($"Slots is null or index {index} is out of range");
            return;
        }

        var slot = slots[index];
        if (slot == null)
        {
            Debug.LogWarning($"Slot at index {index} is null");
            return;
        }

        if (slot.iconGO == null || slot.itemAmountText == null)
        {
            Debug.LogWarning("InventorySlot components are not assigned");
            Debug.LogWarning(slot);
            Debug.LogWarning(defenseID);
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
