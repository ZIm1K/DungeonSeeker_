using Inventory;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using static UnityEditor.Progress;
using Photon.Realtime;

public class CraftItem : MonoBehaviourPun
{
    [SerializeField] private Button craftButton;

    [SerializeField] private TMP_Text text;

    [SerializeField] List<InventorySlot> craftSlots;

    [SerializeField] private PhotonView playerPhotonView;

    private void Start()
    {
        craftButton.onClick.AddListener(OnCraftButtonClick);

        craftButton.gameObject.SetActive(false);
        text.gameObject.SetActive(true);

        foreach (InventorySlot slot in craftSlots)
        {
            slot.OnChangeItems += CheckForCraftButton;
        }
    }

    private void OnCraftButtonClick()
    {
        ActiveCraftButton(false);

        List<int> indexs = new List<int>();

        foreach (InventorySlot slot in craftSlots)
        {
            if (slot.isEmpty == true)
            {
                Debug.LogWarning("One or more slots is Empty");
                return;
            }
        }

        InventoryManager inventoryManager = gameObject.GetComponent<InventoryManager>();
        for (int i = 0; i < craftSlots.Count; i++)
        {
            indexs.Add((craftSlots[i].item as OreItem).indexOfOre);
            NullifySlotData(craftSlots[i]);
            inventoryManager.currentCrafter.RPC("RemoveItemFromCrafter", RpcTarget.All, i);
            inventoryManager.UpdateSlotInOnlineLocalySent(i);
        }
               
        int index1 = indexs[Random.Range(0, craftSlots.Count)];
        int index2 = indexs[Random.Range(0, craftSlots.Count)];

        if (index1 == index2)
        {
            switch (index1)
            {
                case 1: //Iron
                    {
                        AddItemCheck("1", 1);  
                        Debug.LogWarning("Iron");
                        break;
                    }
                case 2: //Gold
                    {
                        AddItemCheck("1", 1);  
                        Debug.LogWarning("Gold");
                        break;
                    }
                case 3: //Emerald
                    {
                        AddItemCheck("1", 1);  
                        Debug.LogWarning("Emerald");
                        break;
                    }
                default:
                    {
                        Debug.LogWarning("Unknown id");
                        break;
                    }
            }
        }
        else //Combined
        {
            AddItemCheck("1", 1);  
            Debug.LogWarning("Combined");
        }
        
    }

    public void ActiveCraftButton(bool setter) 
    {
        craftButton.gameObject.SetActive(setter);
        text.gameObject.SetActive(!setter);
    }

    void AddItemCheck(string itemID, int ammount) 
    {
        if (playerPhotonView.GetComponent<InventoryManager>().CheckEmptyInInventory() == true)
        {
            playerPhotonView.RPC("RPC_AddItemToInventory", RpcTarget.All, itemID, ammount, 0);
        }
    }

    [PunRPC]
    void CheckForCraftButtonLocaly() 
    {
        CheckForCraftButton();
    }

    void CheckForCraftButton() 
    {
        foreach (InventorySlot slot in craftSlots) 
        {
            if (slot.isEmpty == true) 
            {
                ActiveCraftButton(false);
                Debug.LogWarning("One or more is empty");
                return;
            }
        }
        ActiveCraftButton(true);
        Debug.LogWarning("No one is empty");
    }
    void NullifySlotData(InventorySlot slot)
    {
        slot.item = null;
        slot.amount = 0;
        slot.isEmpty = true;
        slot.iconGO.GetComponent<Image>().color = new Color(1, 1, 1, 1);
        slot.iconGO.GetComponent<Image>().sprite = null;
        slot.itemAmountText.text = "";
        slot.defenseID = 0;
    }        

    private void OnDisable()
    {
        if (craftSlots != null)
        {
            foreach (InventorySlot slot in craftSlots)
            {
                slot.OnChangeItems -= CheckForCraftButton;
            }
        }
    }
}
