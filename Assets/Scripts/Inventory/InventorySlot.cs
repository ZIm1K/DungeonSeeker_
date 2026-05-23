using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Inventory
{
    public class InventorySlot : MonoBehaviour
    {
        public Action OnChangeItems;

        public ItemScriptableObject item;       
        public int amount;
        public bool isEmpty = true;
        public GameObject iconGO;
        public TMP_Text itemAmountText;
        public ItemType itemTypeToGet;
        public int defenseID;
        public Sprite basedIcon;
        private void Awake()
        {
            if (transform.childCount == 0 || transform.GetChild(0).childCount < 2)
            {
                Debug.LogWarning($"Inventory slot {name} has an unexpected UI hierarchy.");
                enabled = false;
                return;
            }

            iconGO = transform.GetChild(0).GetChild(0).gameObject;
            itemAmountText = transform.GetChild(0).GetChild(1).GetComponent<TMP_Text>();
            SetBasedIcon();
        }
        
        public void SetIcon(Sprite icon)
        {
            if (iconGO == null) return;

            iconGO.GetComponent<Image>().color = new Color(1, 1, 1, 1);
            iconGO.GetComponent<Image>().sprite = icon;
        }
        public void SetBasedIcon() 
        {
            if (iconGO == null) return;

            iconGO.GetComponent<Image>().sprite = basedIcon;
        }
        public void OnSlotItemChanged() 
        {
            OnChangeItems?.Invoke();           
        }       
    }
}
