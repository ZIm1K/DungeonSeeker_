using Inventory;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Reflection;
using Random = UnityEngine.Random;
using Objects.PlayerScripts;

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
                        int index = Random.Range(1, 5);
                        switch (index) 
                        {
                            case 1:  //Iron Sword
                                {
                                    AddItemCheck("11", 1, 0);
                                    break;
                                }
                            case 2:  //Iron Helmet
                                {
                                    string ID = "2";
                                    AddDefenseItem(ID);                                  
                                    break;
                                }
                            case 3:    //Iron Armor
                                {
                                    string ID = "3";
                                    AddDefenseItem(ID);
                                    break;
                                }
                            case 4:   //Iron Boots
                                {
                                    string ID = "4";
                                    AddDefenseItem(ID);
                                    break;
                                }
                            default:    
                                {
                                    Debug.LogWarning("Unknown id for craft");
                                    break;
                                }
                        }
                        break;
                    }
                case 2: //Copper
                    {
                        int index = Random.Range(1, 4);
                        switch (index)
                        {
                            case 1:  //Copper Helmet
                                {
                                    string ID = "17";
                                    AddDefenseItem(ID);
                                    break;
                                }
                            case 2:  //Copper Armor
                                {
                                    string ID = "18";
                                    AddDefenseItem(ID);
                                    break;
                                }
                            case 3:    //Copper Boots
                                {
                                    string ID = "19";
                                    AddDefenseItem(ID);
                                    break;
                                }                           
                            default:
                                {
                                    Debug.LogWarning("Unknown id for craft");
                                    break;
                                }
                        }
                        break;
                    }
                case 3: //Leather
                    {
                        int index = Random.Range(1, 4);
                        switch (index)
                        {
                            case 1:  //Leather Helmet
                                {
                                    string ID = "20";
                                    AddDefenseItem(ID);
                                    break;
                                }
                            case 2:  //Leather Armor
                                {
                                    string ID = "21";
                                    AddDefenseItem(ID);
                                    break;
                                }
                            case 3:    //Leather Boots
                                {
                                    string ID = "22";
                                    AddDefenseItem(ID);
                                    break;
                                }
                            default:
                                {
                                    Debug.LogWarning("Unknown id for craft");
                                    break;
                                }
                        }
                        break;
                    }
                case 4: //Super Material
                    {
                        int index = Random.Range(1, 4);
                        switch (index)
                        {
                            case 1:  //Magic Helmet
                                {
                                    string ID = "23";
                                    AddDefenseItem(ID);
                                    break;
                                }
                            case 2:  //Magic Armor
                                {
                                    string ID = "24";
                                    AddDefenseItem(ID);
                                    break;
                                }
                            case 3:    //Magic Boots
                                {
                                    string ID = "25";
                                    AddDefenseItem(ID);
                                    break;
                                }
                            default:
                                {
                                    Debug.LogWarning("Unknown id for craft");
                                    break;
                                }
                        }
                        break; ;
                    }
                default:
                    {
                        Debug.LogWarning("Unknown id for craft");
                        break;
                    }
            }
        }
        else 
        {
            AddItemCheck("16", 1, 0);
            Debug.LogWarning("Super Material");
        }
    }

    public void ActiveCraftButton(bool setter) 
    {
        craftButton.gameObject.SetActive(setter);
        text.gameObject.SetActive(!setter);
    }
    void AddDefenseItem(string ID) 
    {
        int createdDefenseID = playerPhotonView.GetComponent<CharacterModel>().durabilDatabase.OnNewDefenseItemAdded(ID);
        AddItemCheck(ID, 1, createdDefenseID);
    }

    void AddItemCheck(string itemID, int ammount, int createdDefenseID) 
    {
        if (playerPhotonView.GetComponent<InventoryManager>().CheckEmptyInInventory() == true)
        {
            playerPhotonView.RPC("RPC_AddItemToInventory", RpcTarget.All, itemID, ammount, createdDefenseID);
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
        slot.SetBasedIcon();
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
