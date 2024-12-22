using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace Inventory
{
    public class InventoryManager : MonoBehaviourPunCallbacks
    {
        [SerializeField] private GameObject UIPanel;
        [SerializeField] private Transform inventoryPanel;
        [SerializeField] private float reachDistance = 3f;
        [SerializeField] private ItemDatabase itemDatabase;

        private List<InventorySlot> slots = new List<InventorySlot>();
        private Camera mainCamera;
        private bool isOpened;

        private void Awake()
        {
            UIPanel.SetActive(photonView.IsMine);
        }

        void Start()
        {
            if (!photonView.IsMine) return;

            mainCamera = Camera.main;
            for (int i = 0; i < inventoryPanel.childCount; i++)
            {
                if (inventoryPanel.GetChild(i).GetComponent<InventorySlot>() != null)
                {
                    slots.Add(inventoryPanel.GetChild(i).GetComponent<InventorySlot>());
                }
            }
            UIPanel.SetActive(false);
        }

        void Update()
        {
            if (!photonView.IsMine) return;

            if (Input.GetKeyDown(KeyCode.I))
            {
                isOpened = !isOpened;
                UIPanel.SetActive(isOpened);
                Cursor.lockState = isOpened ? CursorLockMode.None : CursorLockMode.Locked;
                Cursor.visible = isOpened;
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                TryPickupItem();
            }
        }

        void TryPickupItem()
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, reachDistance))
            {
                PhotonView targetPhotonView = hit.collider.gameObject.GetComponent<PhotonView>();
                             
                if (targetPhotonView != null && targetPhotonView.GetComponent<Item>() != null)
                {
                    int defenseID = 0; 
                    if (targetPhotonView.gameObject.GetComponent<DefenseItem>() != null)
                    {
                        defenseID = hit.collider.gameObject.GetComponent<DefenseItem>().ID;
                    }
                    string itemID = hit.collider.gameObject.GetComponent<Item>().item.itemID;
                    int amount = hit.collider.gameObject.GetComponent<Item>().amount;

                    photonView.RPC("RPC_AddItemToInventory", RpcTarget.All, itemID, amount,defenseID);
                    
                    if (!targetPhotonView.IsMine)
                    {
                        if (PhotonNetwork.IsMasterClient)
                        {
                            PhotonNetwork.Destroy(hit.collider.gameObject);
                        }
                        else
                        {
                            targetPhotonView.RPC("RPC_RequestDestroy", RpcTarget.MasterClient, targetPhotonView.ViewID);                           
                        }
                    }
                    else
                    {
                        PhotonNetwork.Destroy(hit.collider.gameObject);
                    }
                }
            }
        }

        [PunRPC]
        void RPC_AddItemToInventory(string itemID, int amount, int defenseID)
        {
            AddItem(itemID, amount, defenseID);
        }
        void AddItem(string itemID, int amount, int defenseID)
        {
            ItemScriptableObject item = itemDatabase.GetItemByID(itemID);
            if (item != null)
            {
                foreach (InventorySlot slot in slots)
                {
                    if (slot.item == item)
                    {
                        if (slot.amount + amount <= item.maximumAmount)
                        {
                            slot.amount += amount;
                            slot.itemAmountText.text = slot.amount.ToString();
                            return;
                        }
                        break;
                    }
                }
                foreach (InventorySlot slot in slots)
                {
                    if (slot.isEmpty)
                    {
                        slot.item = item;
                        slot.amount = amount;
                        slot.isEmpty = false;
                        slot.SetIcon(item.icon);
                        slot.itemAmountText.text = amount.ToString();
                        slot.defenseID = defenseID;
                        break;
                    }
                }
            }
        }
    }
}