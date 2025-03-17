using ExitGames.Client.Photon.StructWrapping;
using Inventory;
using Objects.PlayerScripts;
using Objects.Weapon;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShowItemStats : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private GameObject statsPanel;

    [SerializeField] private DurabilityDefenseDatabase durabilDatabase;

    private InventorySlot curSlot;

    private void Start()
    {
        statsPanel.SetActive(false);       
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (eventData.pointerCurrentRaycast.gameObject.name == "Icon")
            {
                durabilDatabase = gameObject.GetComponent<CharacterModel>().durabilDatabase;
                
                statsPanel.SetActive(true);

                curSlot = eventData.pointerCurrentRaycast.gameObject.transform.parent.parent.GetComponent<InventorySlot>();

                if (curSlot.item == null) return;

                StatsList statsPanelValues = statsPanel.GetComponent<StatsList>();
                
                statsPanelValues.nameText.text = curSlot.item.itemName;
                statsPanelValues.typeText.text = "Type:" + curSlot.item.itemType.ToString();
                statsPanelValues.descriptionText.text = curSlot.item.itemDescription;

                statsPanelValues.useButton.GetComponent<Button>().onClick.RemoveAllListeners();
                statsPanelValues.useButton.SetActive(false);

                HideAllTexts();
                switch (curSlot.item.itemType) 
                {
                    case ItemType.Food: 
                        {
                            durabilDatabase.OnChangeValues -= IfDurabilChanged;

                            statsPanelValues.foodItemTexts.SetActive(true);

                            statsPanelValues.foodItemTexts.transform.GetChild(0).GetComponent<TMP_Text>().text = 
                                (curSlot.item as FoodItem).healAmount.ToString();  //heal ammount
                            
                            statsPanelValues.useButton.SetActive(true);
                            
                            statsPanelValues.useButton.GetComponent<Button>().onClick.AddListener(HealCharacter);
                            break;
                        }
                    case ItemType.Helmet: 
                        {
                            durabilDatabase.OnChangeValues += IfDurabilChanged;

                            statsPanelValues.defenseItemTexts.SetActive(true);

                            statsPanelValues.defenseItemTexts.transform.GetChild(0).GetComponent<TMP_Text>().text =
                                (curSlot.item as HelmetItem).defense.ToString();  //defense max ammount
                            GetDurabilityValue(statsPanelValues, curSlot.defenseID);
                           
                            statsPanelValues.defenseItemTexts.transform.GetChild(2).gameObject.SetActive(false);
                            statsPanelValues.defenseItemTexts.transform.GetChild(3).gameObject.SetActive(false);
                            break;
                        }
                    case ItemType.Armor:
                        {
                            durabilDatabase.OnChangeValues += IfDurabilChanged;

                            statsPanelValues.defenseItemTexts.SetActive(true);

                            statsPanelValues.defenseItemTexts.transform.GetChild(0).GetComponent<TMP_Text>().text =
                                (curSlot.item as ArmorItem).defense.ToString();  //defense max ammount
                            GetDurabilityValue(statsPanelValues, curSlot.defenseID);

                            statsPanelValues.defenseItemTexts.transform.GetChild(2).gameObject.SetActive(true);
                            statsPanelValues.defenseItemTexts.transform.GetChild(2).GetComponent<TMP_Text>().text =
                                (curSlot.item as ArmorItem).nerfSpeed.ToString();  //nerf speed

                            statsPanelValues.defenseItemTexts.transform.GetChild(3).gameObject.SetActive(false);
                            break;
                        }
                    case ItemType.Boots:
                        {
                            durabilDatabase.OnChangeValues += IfDurabilChanged;

                            statsPanelValues.defenseItemTexts.SetActive(true);

                            statsPanelValues.defenseItemTexts.transform.GetChild(0).GetComponent<TMP_Text>().text =
                                (curSlot.item as BootsItem).defense.ToString();  //defense max ammount
                            GetDurabilityValue(statsPanelValues, curSlot.defenseID);

                            statsPanelValues.defenseItemTexts.transform.GetChild(2).gameObject.SetActive(false);

                            statsPanelValues.defenseItemTexts.transform.GetChild(3).gameObject.SetActive(true);
                            statsPanelValues.defenseItemTexts.transform.GetChild(3).GetComponent<TMP_Text>().text =
                                (curSlot.item as BootsItem).buffSpeed.ToString();  //buff speed
                            break;
                        }
                    case ItemType.Charm:
                        {
                            durabilDatabase.OnChangeValues -= IfDurabilChanged;

                            statsPanelValues.charmItemTexts.SetActive(true);
                            
                            statsPanelValues.charmItemTexts.transform.GetChild(0).gameObject.SetActive(true);
                            statsPanelValues.charmItemTexts.transform.GetChild(0).GetComponent<TMP_Text>().text =
                                (curSlot.item as CharmItem).buffSpeed.ToString();  //buff speed
                                                                                
                            statsPanelValues.charmItemTexts.transform.GetChild(1).gameObject.SetActive(true);
                            statsPanelValues.charmItemTexts.transform.GetChild(1).GetComponent<TMP_Text>().text =
                                (curSlot.item as CharmItem).buffJumpForce.ToString();  //buff speed 
                            break;
                        }
                    case ItemType.Weapon:
                        {
                            durabilDatabase.OnChangeValues -= IfDurabilChanged;

                            statsPanelValues.weaponItemTexts.SetActive(true);
                            
                            statsPanelValues.weaponItemTexts.transform.GetChild(0).gameObject.SetActive(true);

                            if (curSlot.item as FireBallItemData)
                            {
                                statsPanelValues.weaponItemTexts.transform.GetChild(0).GetComponent<TMP_Text>().text =
                                                                (curSlot.item as FireBallItemData).data.fireballDamage.ToString();  //damage
                            }
                            else if (curSlot.item as PistolItemData)
                            {
                                statsPanelValues.weaponItemTexts.transform.GetChild(0).GetComponent<TMP_Text>().text =
                                                                (curSlot.item as PistolItemData).data.pistolDamage.ToString();  //damage
                            }
                            else if (curSlot.item as SwordItemData)
                            {
                                statsPanelValues.weaponItemTexts.transform.GetChild(0).GetComponent<TMP_Text>().text =
                                                                (curSlot.item as SwordItemData).data.swordDamage.ToString();  //damage
                            }
                            else if (curSlot.item as StaffItem)
                            {
                                statsPanelValues.weaponItemTexts.transform.GetChild(0).GetComponent<TMP_Text>().text =
                                                                (curSlot.item as StaffItem).data.allyHealAmmount.ToString();  //damage
                            }
                            else 
                            {
                                Debug.LogWarning("Sry but this type of weapon not founded");
                            }                                                                                                                                                                 
                            break;
                        }                  
                    default:
                        {
                            Debug.LogWarning("Sry but this type of item not founded or it is not need text");
                            break;
                        }
                }   
            }
            else
            {
                CloseStatsPanel();
            }
        }
    }
    void HealCharacter() 
    {
        if (gameObject.GetComponent<CharacterModel>().Health == 100) return;
        
        curSlot.amount -= 1;        

        if (gameObject.GetComponent<CharacterModel>().MaxHealth - gameObject.GetComponent<CharacterModel>().Health < (curSlot.item as FoodItem).healAmount)
        {
            gameObject.GetComponent<CharacterModel>().Heal(gameObject.GetComponent<CharacterModel>().MaxHealth 
                - gameObject.GetComponent<CharacterModel>().Health);
        }
        else 
        {
            gameObject.GetComponent<CharacterModel>().Heal((curSlot.item as FoodItem).healAmount);
        }

        if(curSlot.amount < 1)
        {
            curSlot.item = null;
            curSlot.isEmpty = true;
            curSlot.iconGO.GetComponent<Image>().color = new Color(1, 1, 1, 1);
            curSlot.iconGO.GetComponent<Image>().sprite = null;
            curSlot.itemAmountText.text = "";
            curSlot.defenseID = 0;

            CloseStatsPanel();
        }
        else
        {
            curSlot.itemAmountText.text = curSlot.amount.ToString();
        }
    }
    void CloseStatsPanel() 
    {
        durabilDatabase.OnChangeValues -= IfDurabilChanged;
        HideAllTexts();
        curSlot = null;
        statsPanel.GetComponent<StatsList>().useButton.GetComponent<Button>().onClick.RemoveAllListeners();
        statsPanel.GetComponent<StatsList>().useButton.SetActive(false);
        statsPanel.SetActive(false);
    }
    void GetDurabilityValue(StatsList statsPanelValues, int defenseID) 
    {
        statsPanelValues.defenseItemTexts.transform.GetChild(1).GetComponent<TMP_Text>().text =
                                    durabilDatabase.GetValueByID(defenseID).ToString(); //defense cur ammount  
    }
    void HideAllTexts() 
    {
        statsPanel.GetComponent<StatsList>().foodItemTexts.SetActive(false);
        statsPanel.GetComponent<StatsList>().defenseItemTexts.SetActive(false);
        statsPanel.GetComponent<StatsList>().charmItemTexts.SetActive(false);
        statsPanel.GetComponent<StatsList>().weaponItemTexts.SetActive(false);
    }
    void IfDurabilChanged() 
    {
        if (statsPanel.activeSelf) 
        {
            StatsList statsPanelValues = statsPanel.GetComponent<StatsList>();
            if (statsPanelValues.defenseItemTexts.activeSelf)
            {
                GetDurabilityValue(statsPanelValues, curSlot.defenseID);
            }
        }
    }
}
