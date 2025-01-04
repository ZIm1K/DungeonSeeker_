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

        private void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
            oldSlot = transform.GetComponentInParent<InventorySlot>();
            playerView = player.GetComponent<PhotonView>();
        }
        public void OnDrag(PointerEventData eventData)
        {
            if (oldSlot.isEmpty)
                return;
            GetComponent<RectTransform>().position += new Vector3(eventData.delta.x, eventData.delta.y);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (oldSlot.isEmpty)
                return;           

            if (eventData.pointerCurrentRaycast.gameObject.transform.parent.parent.parent.parent.name == "ChestInventory")
            {
                isItemFromChest = true;

                GameObject chestSlots = eventData.pointerCurrentRaycast.gameObject.transform.parent.parent.parent.gameObject;

                for (int i = 0; i < chestSlots.transform.childCount; i++)
                {
                    bool isEnd = false;
                    if (!isEnd)
                    {
                        if (chestSlots.transform.GetChild(i).gameObject == oldSlot.gameObject)
                        {
                            player.GetComponent<InventoryManager>().currentChest.RPC("RemoveItemFromChest", RpcTarget.All, i);
                            player.GetComponent<InventoryManager>().UpdateSlotInOnlineLocalySent(i);
                            isEnd = true;
                        }
                    }
                }
            }
            else
            {
                isItemFromChest = false;
            }
            GetComponentInChildren<Image>().color = new Color(1, 1, 1, 0.5f);
            GetComponentInChildren<Image>().raycastTarget = false;
            transform.SetParent(transform.parent.parent);
        }
        
        public void OnPointerUp(PointerEventData eventData)
        {
            if (oldSlot.isEmpty)
                return;

            GetComponentInChildren<Image>().color = new Color(1, 1, 1, 1f);
            GetComponentInChildren<Image>().raycastTarget = true;

            transform.SetParent(oldSlot.transform);
            transform.position = oldSlot.transform.position;

            if (oldSlot.gameObject == eventData.pointerCurrentRaycast.gameObject && isItemFromChest) 
            {
                GameObject chestSlots = eventData.pointerCurrentRaycast.gameObject.transform.parent.gameObject;

                for (int i = 0; i < chestSlots.transform.childCount; i++)
                {
                    bool isEnd = false;
                    if (!isEnd)
                    {
                        if (chestSlots.transform.GetChild(i).gameObject == oldSlot.gameObject)
                        {
                            player.GetComponent<InventoryManager>().currentChest.RPC("AddItemToChest", RpcTarget.All, oldSlot.item.itemID,
                            oldSlot.defenseID, oldSlot.amount, oldSlot.isEmpty, i);
                            player.GetComponent<InventoryManager>().UpdateSlotInOnlineLocalySent(i);
                            isEnd = true;
                        }
                    }
                }
            }

            if (eventData.pointerCurrentRaycast.gameObject == null) return;
            if (eventData.pointerCurrentRaycast.gameObject.name == "UIPanel")
            {
                GameObject itemObject = PhotonNetwork.Instantiate(oldSlot.item.itemPrefab.name,
                    player.position + Vector3.up + player.forward, Quaternion.identity);
                itemObject.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.MasterClient);
                itemObject.GetComponent<Item>().amount = oldSlot.amount;
                if (itemObject.GetComponent<DefenseItem>() != null)
                {
                    OnUnWearItem(oldSlot.item);
                    itemObject.GetComponent<DefenseItem>().ID = oldSlot.defenseID;
                }
                else if (itemObject.GetComponent<Item>().item.itemType == ItemType.Charm)
                {
                    OnUnWearItem(oldSlot.item);
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
                            else 
                            {
                                //player.GetComponent<InventoryManager>().currentChest.RPC("RemoveItemFromChest", RpcTarget.All, i);
                            }
                            player.GetComponent<InventoryManager>().UpdateSlotInOnlineLocalySent(i);
                            canBreakIndex++;
                        }
                        if (isItemFromChest == false)
                        {
                            if (canBreakIndex == 1) 
                            {
                                break;
                            }
                        }
                        else if(canBreakIndex == 2)
                        {
                            break;
                        }
                        
                    }  
                    
                    isItemFromChest = true;

                    return;
                }
                //else if (isItemFromChest == true) 
                //{
                //    isItemFromChest = false;

                //    GameObject chestsSlots = eventData.pointerCurrentRaycast.gameObject.transform.
                //        parent.parent.parent.parent.GetChild(0).GetChild(0).gameObject;

                //    for (int i = 0; i < chestsSlots.transform.childCount; i++)
                //    {
                //        if (chestsSlots.transform.GetChild(i).gameObject == oldSlot.gameObject)
                //        {
                //            //player.GetComponent<InventoryManager>().currentChest.RPC("RemoveItemFromChest", RpcTarget.All, i);
                //            //player.GetComponent<InventoryManager>().UpdateSlotInOnlineLocalySent(i);
                //            return;
                //        }
                //    }                   
                //}                
            }
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