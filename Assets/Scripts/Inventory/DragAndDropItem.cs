using Inventory;
using Objects.PlayerScripts;
using Photon.Pun;
using Photon.Pun.Demo.Cockpit;
using System.Net;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEditor.Progress;

namespace Inventory
{
    public class DragAndDropItem : MonoBehaviourPun, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        public InventorySlot oldSlot;
        private Transform player;
        private PhotonView playerView;

        private bool isItemFromChest;

        private bool isItemFromCrafter;

        private void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
            oldSlot = transform.GetComponentInParent<InventorySlot>();
            playerView = player.GetComponent<PhotonView>();
        }
        public void OnDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            if (oldSlot.isEmpty)
                return;
            GetComponent<RectTransform>().position += new Vector3(eventData.delta.x, eventData.delta.y);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            if (oldSlot.isEmpty)
                return;

            player.GetComponent<InventoryManager>().isItemOnDrag = true;
            if (eventData.pointerCurrentRaycast.gameObject.transform.parent.parent.parent.parent.name == "ChestInventory")
            {
                isItemFromChest = true;
                isItemFromCrafter = false;

                GameObject chestSlots = eventData.pointerCurrentRaycast.gameObject.transform.parent.parent.parent.gameObject;

                for (int i = 0; i < chestSlots.transform.childCount; i++)
                {
                    if (chestSlots.transform.GetChild(i).gameObject == oldSlot.gameObject)
                    {
                        player.GetComponent<InventoryManager>().currentChest.RPC("RemoveItemFromChest", RpcTarget.All, i);
                        player.GetComponent<InventoryManager>().UpdateSlotInOnlineLocalySent(i);
                        break;
                    }
                }
            }
            else if (eventData.pointerCurrentRaycast.gameObject.transform.parent.parent.parent.parent.name == "CraftInventory")
            {
                isItemFromChest = false;
                isItemFromCrafter = true;

                GameObject crafterSlots = eventData.pointerCurrentRaycast.gameObject.transform.parent.parent.parent.gameObject;

                for (int i = 0; i < crafterSlots.transform.childCount; i++)
                {
                    if (crafterSlots.transform.GetChild(i).gameObject == oldSlot.gameObject)
                    {
                        player.GetComponent<InventoryManager>().currentCrafter.RPC("RemoveItemFromCrafter", RpcTarget.All, i);
                        player.GetComponent<InventoryManager>().UpdateSlotInOnlineLocalySent(i);
                        break;
                    }
                }

            }
            else 
            {
                isItemFromCrafter = false;
                isItemFromChest = false;
            }
            GetComponentInChildren<Image>().color = new Color(1, 1, 1, 0.5f);
            GetComponentInChildren<Image>().raycastTarget = false;
            transform.SetParent(transform.parent.parent);
        }
        
        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            if (oldSlot.isEmpty)
                return;

            GetComponentInChildren<Image>().color = new Color(1, 1, 1, 1f);
            GetComponentInChildren<Image>().raycastTarget = true;

            transform.SetParent(oldSlot.transform);
            transform.position = oldSlot.transform.position;

            if (oldSlot.gameObject == eventData.pointerCurrentRaycast.gameObject) 
            {
                if (isItemFromChest)
                {
                    GameObject chestSlots = eventData.pointerCurrentRaycast.gameObject.transform.parent.gameObject;

                    for (int i = 0; i < chestSlots.transform.childCount; i++)
                    {
                        if (chestSlots.transform.GetChild(i).gameObject == oldSlot.gameObject)
                        {
                            player.GetComponent<InventoryManager>().currentChest.RPC("AddItemToChest", RpcTarget.All, oldSlot.item.itemID,
                            oldSlot.defenseID, oldSlot.amount, oldSlot.isEmpty, i);
                            player.GetComponent<InventoryManager>().UpdateSlotInOnlineLocalySent(i);
                            break;
                        }
                    }
                }
                else if (isItemFromCrafter) 
                {
                    GameObject craftSlots = eventData.pointerCurrentRaycast.gameObject.transform.parent.gameObject;

                    for (int i = 0; i < craftSlots.transform.childCount; i++)
                    {
                        if (craftSlots.transform.GetChild(i).gameObject == oldSlot.gameObject)
                        {
                            player.GetComponent<InventoryManager>().currentCrafter.RPC("AddItemToCrafter", RpcTarget.All, oldSlot.item.itemID,
                                oldSlot.isEmpty, i);
                            player.GetComponent<InventoryManager>().UpdateSlotInOnlineLocalySent(i);
                            break;
                        }
                    }
                }
            }
            
            player.GetComponent<InventoryManager>().isItemOnDrag = false;
            if (eventData.pointerCurrentRaycast.gameObject == null) return;
            if (eventData.pointerCurrentRaycast.gameObject.name == "UIPanel")
            {
                GameObject itemObject = PhotonNetwork.Instantiate(oldSlot.item.itemPrefab.name,
                    player.position + Vector3.up + player.forward, Quaternion.identity);
                itemObject.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.MasterClient);
                itemObject.GetComponent<PhotonView>().RPC("RPC_Ammount", RpcTarget.All, oldSlot.amount);
                //itemObject.GetComponent<Item>().amount = oldSlot.amount;
                if (itemObject.GetComponent<DefenseItem>() != null)
                {
                    itemObject.GetComponent<PhotonView>().RPC("RPC_DefenseID", RpcTarget.All, oldSlot.defenseID);
                    //itemObject.GetComponent<DefenseItem>().ID = oldSlot.defenseID;                    
                }
                if (oldSlot.itemTypeToGet != ItemType.Default)
                {
                    if (oldSlot.itemTypeToGet == ItemType.Helmet ||
                        oldSlot.itemTypeToGet == ItemType.Armor ||
                        oldSlot.itemTypeToGet == ItemType.Boots ||
                        oldSlot.itemTypeToGet == ItemType.Charm)
                    {
                        OnUnWearItem(oldSlot.item);
                    }
                }
                NullifySlotData();
                oldSlot.OnSlotItemChanged();
            }
            else if (eventData.pointerCurrentRaycast.gameObject.transform.parent.parent.GetComponent<InventorySlot>() != null)
            {
                InventorySlot newSlot = eventData.pointerCurrentRaycast.gameObject.transform.parent.parent.GetComponent<InventorySlot>();
                if (newSlot.itemTypeToGet == oldSlot.itemTypeToGet)
                {
                    if (newSlot.itemTypeToGet == ItemType.Default && oldSlot.itemTypeToGet == ItemType.Default)
                    {
                        ExchangeSlotData(newSlot);
                    }
                    else
                    {
                        OnUnWearItem(newSlot.item);
                        OnUnWearItem(oldSlot.item);
                        ExchangeSlotData(newSlot);
                        OnWearItem(newSlot.item, newSlot.defenseID);
                        OnWearItem(oldSlot.item, oldSlot.defenseID);
                    }
                }
                else
                {
                    if (newSlot.itemTypeToGet == oldSlot.item.itemType && oldSlot.itemTypeToGet == ItemType.Default)
                    {
                        OnUnWearItem(newSlot.item);
                        ExchangeSlotData(newSlot);
                        OnWearItem(newSlot.item, newSlot.defenseID);
                    }
                    else if (newSlot.itemTypeToGet == ItemType.Default)
                    {
                        if (newSlot.item != null)
                        {
                            if (oldSlot.itemTypeToGet == newSlot.item.itemType)
                            {
                                OnUnWearItem(newSlot.item);
                                ExchangeSlotData(newSlot);
                                OnWearItem(newSlot.item, newSlot.defenseID);
                            }
                        }
                        else
                        {
                            OnUnWearItem(oldSlot.item);
                            ExchangeSlotData(newSlot);
                        }
                    }
                }

                if (eventData.pointerCurrentRaycast.gameObject.transform.parent.parent.parent.parent.name == "ChestInventory")
                {
                    int canBreakIndex = 0;

                    GameObject chestSlots = eventData.pointerCurrentRaycast.gameObject.transform.parent.parent.parent.gameObject;

                    for (int i = 0; i < chestSlots.transform.childCount; i++)
                    {
                        if (chestSlots.transform.GetChild(i).gameObject == newSlot.gameObject)
                        {
                            player.GetComponent<InventoryManager>().currentChest.RPC("AddItemToChest", RpcTarget.All, newSlot.item.itemID,
                            newSlot.defenseID, newSlot.amount, newSlot.isEmpty, i);
                            player.GetComponent<InventoryManager>().UpdateSlotInOnlineLocalySent(i);
                            canBreakIndex++;
                        }
                        else if (chestSlots.transform.GetChild(i).gameObject == oldSlot.gameObject)
                        {
                            if (oldSlot.isEmpty == false)
                            {
                                player.GetComponent<InventoryManager>().currentChest.RPC("AddItemToChest", RpcTarget.All, oldSlot.item.itemID,
                                    oldSlot.defenseID, oldSlot.amount, oldSlot.isEmpty, i);
                            }
                            player.GetComponent<InventoryManager>().UpdateSlotInOnlineLocalySent(i);
                            canBreakIndex++;
                        }
                        
                        if (canBreakIndex == 2)
                        {
                            break;
                        }

                    }

                    isItemFromChest = true;

                    return;
                }
                else if (eventData.pointerCurrentRaycast.gameObject.transform.parent.parent.parent.parent.name == "CraftInventory")
                {
                    int canBreakIndex = 0;

                    GameObject craftSlots = eventData.pointerCurrentRaycast.gameObject.transform.parent.parent.parent.gameObject;

                    for (int i = 0; i < craftSlots.transform.childCount; i++)
                    {
                        if (newSlot.item != null)
                        {
                            if (craftSlots.transform.GetChild(i).gameObject == newSlot.gameObject)
                            {
                                player.GetComponent<InventoryManager>().currentCrafter.RPC("AddItemToCrafter", RpcTarget.All, newSlot.item.itemID,
                                    newSlot.isEmpty, i);
                                player.GetComponent<InventoryManager>().UpdateSlotInOnlineLocalySent(i);
                                canBreakIndex++;
                            }
                            else if (craftSlots.transform.GetChild(i).gameObject == oldSlot.gameObject)
                            {
                                if (oldSlot.isEmpty == false)
                                {
                                    player.GetComponent<InventoryManager>().currentCrafter.RPC("AddItemToCrafter", RpcTarget.All, oldSlot.item.itemID,
                                        oldSlot.isEmpty, i);
                                }
                                player.GetComponent<InventoryManager>().UpdateSlotInOnlineLocalySent(i);
                                canBreakIndex++;
                            }                           
                            if (canBreakIndex == 2)
                            {
                                break;
                            }
                        }

                    }

                    isItemFromCrafter = true;

                    return;
                }
            }
            else 
            {
                if (isItemFromChest)
                {
                    int id = CheckForID();
                    player.GetComponent<InventoryManager>().currentChest.RPC("AddItemToChest", RpcTarget.All, oldSlot.item.itemID,oldSlot.defenseID
                        ,oldSlot.amount,oldSlot.isEmpty, id);
                    player.GetComponent<InventoryManager>().UpdateSlotInOnlineLocalySent(id);
                }
                else if(isItemFromCrafter)
                {
                    int id = CheckForID();
                    player.GetComponent<InventoryManager>().currentCrafter.RPC("AddItemToCrafter", RpcTarget.All, oldSlot.item.itemID,
                                    oldSlot.isEmpty, id);
                    player.GetComponent<InventoryManager>().UpdateSlotInOnlineLocalySent(id);
                }
            }            
        }

        int CheckForID() 
        {
            int slotId = 0;

            GameObject slots = transform.parent.parent.gameObject;

            for (int i = 0; i < slots.transform.childCount; i++)
            {
                if (slots.transform.GetChild(i).gameObject == oldSlot.gameObject)
                {
                    slotId = i;
                    return slotId;
                }
            }
            return 0;
        }
        private void OnWearItem(ItemScriptableObject item, int defenseID)
        {
            switch (item)
            {
                case HelmetItem:
                    {
                        HelmetItem itemHelmet = item as HelmetItem;
                        playerView.RPC("WearDefense", playerView.Owner, itemHelmet, defenseID);
                        break;
                    }
                case ArmorItem:
                    {
                        ArmorItem itemArmor = item as ArmorItem;
                        playerView.RPC("WearDefense", playerView.Owner, itemArmor, defenseID);
                        playerView.RPC("NerfSpeed", playerView.Owner, itemArmor.nerfSpeed);
                        break;
                    }
                case BootsItem:
                    {
                        BootsItem itemBoots = item as BootsItem;
                        playerView.RPC("WearDefense", playerView.Owner, itemBoots, defenseID);
                        playerView.RPC("BuffSpeed", playerView.Owner, itemBoots.buffSpeed);
                        break;
                    }
                case CharmItem:
                    {
                        CharmItem itemCharm = item as CharmItem;
                        playerView.RPC("BuffSpeed", playerView.Owner, itemCharm.buffSpeed);
                        playerView.RPC("BuffJumpForce", playerView.Owner, itemCharm.buffJumpForce);
                        break;
                    }
                default:
                    {
                        Debug.Log("This is not buffer");
                        break;
                    }
            }
        }
        private void OnUnWearItem(ItemScriptableObject item)
        {
            switch (item)
            {
                case HelmetItem:
                    {
                        HelmetItem itemHelmet = item as HelmetItem;
                        playerView.RPC("UnWearDefense", playerView.Owner, itemHelmet);
                        break;
                    }
                case ArmorItem:
                    {
                        ArmorItem itemArmor = item as ArmorItem;
                        playerView.RPC("UnWearDefense", playerView.Owner, itemArmor);
                        playerView.RPC("BuffSpeed", playerView.Owner, itemArmor.nerfSpeed);
                        break;
                    }
                case BootsItem:
                    {
                        BootsItem itemBoots = item as BootsItem;
                        playerView.RPC("UnWearDefense", playerView.Owner, itemBoots);
                        playerView.RPC("NerfSpeed", playerView.Owner, itemBoots.buffSpeed);
                        break;
                    }
                case CharmItem:
                    {
                        CharmItem itemCharm = item as CharmItem;
                        playerView.RPC("NerfSpeed", playerView.Owner, itemCharm.buffSpeed);
                        playerView.RPC("NerfJumpForce", playerView.Owner, itemCharm.buffJumpForce);
                        break;
                    }
                default:
                    {
                        Debug.Log("This is not buffer");
                        break;
                    }
            }
        }       
        void NullifySlotData()
        {
            oldSlot.item = null;
            oldSlot.amount = 0;
            oldSlot.isEmpty = true;
            oldSlot.iconGO.GetComponent<Image>().color = new Color(1, 1, 1, 1);
            oldSlot.iconGO.GetComponent<Image>().sprite = null;
            oldSlot.itemAmountText.text = "";
            oldSlot.defenseID = 0;
        }
        void ExchangeSlotData(InventorySlot newSlot)
        {
            ItemScriptableObject item = newSlot.item;
            int amount = newSlot.amount;
            bool isEmpty = newSlot.isEmpty;
            Sprite iconGO = newSlot.iconGO.GetComponent<Image>().sprite;
            TMP_Text itemAmountText = newSlot.itemAmountText;
            int defenseID = newSlot.defenseID;

            newSlot.item = oldSlot.item;
            newSlot.amount = oldSlot.amount;
            if (oldSlot.isEmpty == false)
            {
                newSlot.SetIcon(oldSlot.iconGO.GetComponent<Image>().sprite);
                newSlot.itemAmountText.text = oldSlot.amount.ToString();                
            }
            else
            {
                newSlot.iconGO.GetComponent<Image>().color = new Color(1, 1, 1, 0);
                newSlot.iconGO.GetComponent<Image>().sprite = null;
                newSlot.itemAmountText.text = "";
            }

            newSlot.defenseID = oldSlot.defenseID;
            newSlot.isEmpty = oldSlot.isEmpty;
            oldSlot.item = item;
            oldSlot.amount = amount;
            if (isEmpty == false)
            {
                oldSlot.SetIcon(iconGO);
                oldSlot.itemAmountText.text = amount.ToString();
                
            }
            else
            {
                oldSlot.iconGO.GetComponent<Image>().color = new Color(1, 1, 1, 1);
                oldSlot.iconGO.GetComponent<Image>().sprite = null;
                oldSlot.itemAmountText.text = "";
            }

            oldSlot.defenseID = defenseID;
            oldSlot.isEmpty = isEmpty;

            newSlot.OnSlotItemChanged();
            oldSlot.OnSlotItemChanged();
        }
    }
}