using ExitGames.Client.Photon.StructWrapping;
using Inventory;
using Objects.PlayerScripts;
using Objects.Weapon;
using Objects.Weapon.Bow;
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

    private DurabilityDefenseDatabase durabilDatabase;

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
                if (eventData.pointerCurrentRaycast.gameObject.transform.parent.parent.GetComponent<InventorySlot>().item != null) 
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

                    durabilDatabase.OnChangeValues -= IfDurabilChanged;
                    curSlot.OnChangeItems += IfSlotItemChanged;

                    HideAllTexts();
                    switch (curSlot.item.itemType)
                    {
                        case ItemType.Food:
                            {
                                //durabilDatabase.OnChangeValues -= IfDurabilChanged;

                                statsPanelValues.fooodItemTextPanel.SetActive(true);

                                statsPanelValues.fooodItemTextPanel.transform.GetChild(0).GetComponent<TMP_Text>().text =
                                    (curSlot.item as FoodItem).healAmount.ToString();  //heal ammount

                                statsPanelValues.useButton.SetActive(true);

                                statsPanelValues.useButton.GetComponent<Button>().onClick.AddListener(HealCharacter);
                                break;
                            }
                        case ItemType.Helmet:
                            {
                                durabilDatabase.OnChangeValues += IfDurabilChanged;

                                statsPanelValues.defenseItemTextPanel.SetActive(true);

                                statsPanelValues.defenseItemTextPanel.transform.GetChild(0).GetComponent<TMP_Text>().text =
                                    (curSlot.item as HelmetItem).defense.ToString();  //defense max ammount
                                GetDurabilityValue(statsPanelValues, curSlot.defenseID);

                                statsPanelValues.defenseItemTextPanel.transform.GetChild(2).gameObject.SetActive(false);
                                statsPanelValues.defenseItemTextPanel.transform.GetChild(3).gameObject.SetActive(false);
                                break;
                            }
                        case ItemType.Armor:
                            {
                                durabilDatabase.OnChangeValues += IfDurabilChanged;

                                statsPanelValues.defenseItemTextPanel.SetActive(true);

                                statsPanelValues.defenseItemTextPanel.transform.GetChild(0).GetComponent<TMP_Text>().text =
                                    (curSlot.item as ArmorItem).defense.ToString();  //defense max ammount
                                GetDurabilityValue(statsPanelValues, curSlot.defenseID);

                                statsPanelValues.defenseItemTextPanel.transform.GetChild(2).gameObject.SetActive(true);
                                statsPanelValues.defenseItemTextPanel.transform.GetChild(2).GetComponent<TMP_Text>().text =
                                    (curSlot.item as ArmorItem).nerfSpeed.ToString();  //nerf speed

                                statsPanelValues.defenseItemTextPanel.transform.GetChild(3).gameObject.SetActive(false);
                                break;
                            }
                        case ItemType.Boots:
                            {
                                durabilDatabase.OnChangeValues += IfDurabilChanged;

                                statsPanelValues.defenseItemTextPanel.SetActive(true);

                                statsPanelValues.defenseItemTextPanel.transform.GetChild(0).GetComponent<TMP_Text>().text =
                                    (curSlot.item as BootsItem).defense.ToString();  //defense max ammount
                                GetDurabilityValue(statsPanelValues, curSlot.defenseID);

                                statsPanelValues.defenseItemTextPanel.transform.GetChild(2).gameObject.SetActive(false);

                                statsPanelValues.defenseItemTextPanel.transform.GetChild(3).gameObject.SetActive(true);
                                statsPanelValues.defenseItemTextPanel.transform.GetChild(3).GetComponent<TMP_Text>().text =
                                    (curSlot.item as BootsItem).buffSpeed.ToString();  //buff speed
                                break;
                            }
                        case ItemType.Charm:
                            {
                                statsPanelValues.charmItemTextPanel.SetActive(true);

                                //statsPanelValues.charmItemTextPanel.transform.GetChild(0).gameObject.SetActive(true);
                                statsPanelValues.charmItemTextPanel.transform.GetChild(0).GetComponent<TMP_Text>().text =
                                    (curSlot.item as CharmItem).buffSpeed.ToString();  //buff speed

                                //statsPanelValues.charmItemTextPanel.transform.GetChild(1).gameObject.SetActive(true);
                                statsPanelValues.charmItemTextPanel.transform.GetChild(1).GetComponent<TMP_Text>().text =
                                    (curSlot.item as CharmItem).buffJumpForce.ToString();  //buff speed 
                                break;
                            }
                        case ItemType.Weapon:
                            {
                                statsPanelValues.weaponItemTexts.SetActive(true);

                                //statsPanelValues.weaponItemTexts.transform.GetChild(0).gameObject.SetActive(true);

                                if (curSlot.item as FireBallItemData)
                                {
                                    statsPanelValues.weaponItemTexts.transform.GetChild(0).GetComponent<TMP_Text>().text =
                                                                    (curSlot.item as FireBallItemData).data.fireballDamage.ToString();  //damage
                                }
                                else if (curSlot.item as BowItemData)
                                {
                                    statsPanelValues.weaponItemTexts.transform.GetChild(0).GetComponent<TMP_Text>().text =
                                                                    (curSlot.item as BowItemData).data.bowDamage.ToString();  //damage
                                }
                                else if (curSlot.item as SwordItemData)
                                {
                                    statsPanelValues.weaponItemTexts.transform.GetChild(0).GetComponent<TMP_Text>().text =
                                                                    (curSlot.item as SwordItemData).data.swordDamage.ToString();  //damage
                                }
                                else if (curSlot.item as SummonerStaffItem)
                                {
                                    statsPanelValues.weaponItemTexts.transform.GetChild(0).GetComponent<TMP_Text>().text =
                                                                    (curSlot.item as SummonerStaffItem).data.allyHealAmmount.ToString();  //damage
                                }
                                else if (curSlot.item as CrucifixItem)
                                {
                                    statsPanelValues.weaponItemTexts.transform.GetChild(0).GetComponent<TMP_Text>().text =
                                                                    (curSlot.item as CrucifixItem).data.damage.ToString();  //damage
                                }
                                else if (curSlot.item as DefenseStaffItem)
                                {
                                    statsPanelValues.weaponItemTexts.transform.GetChild(0).GetComponent<TMP_Text>().text =
                                                                    (curSlot.item as DefenseStaffItem).data.defenseHealAmmount.ToString();  //defense heal
                                }
                                else
                                {
                                    Debug.LogWarning("Sry but this type of weapon not founded");
                                }
                                break;
                            }
                        case ItemType.Bullet:
                            {
                                statsPanelValues.bulletItemTexts.SetActive(true);

                                statsPanelValues.bulletItemTexts.transform.GetChild(0).GetComponent<TMP_Text>().text =
                                                                    (curSlot.item as BulletItem).bulletAmmount.ToString();  //bulletAmmount

                                statsPanelValues.useButton.SetActive(true);

                                statsPanelValues.useButton.GetComponent<Button>().onClick.AddListener(AddBullets);
                                break;
                            }
                        case ItemType.ManaPotion:
                            {
                                statsPanelValues.manaPotionItemTexts.SetActive(true);

                                //statsPanelValues.manaPotionItemTexts.transform.GetChild(0).gameObject.SetActive(true);
                                statsPanelValues.manaPotionItemTexts.transform.GetChild(0).GetComponent<TMP_Text>().text =
                                                                    (curSlot.item as ManaRegenItem).manaRegenInterval.ToString();  //manaRegenChange

                                statsPanelValues.manaPotionItemTexts.transform.GetChild(1).GetComponent<TMP_Text>().text =
                                                                    (curSlot.item as ManaRegenItem).duration.ToString();  //duration

                                statsPanelValues.useButton.SetActive(true);

                                statsPanelValues.useButton.GetComponent<Button>().onClick.AddListener(RegenBuff);
                                break;
                            }
                        default:
                            {
                                Debug.LogWarning("Sry but this type of item not founded or it is not need text");
                                break;
                            }
                    }
                }                
            }
            else
            {
                CloseStatsPanel();
            }
        }
    }
    void RegenBuff()
    {
        curSlot.amount -= 1;
        
        gameObject.GetComponent<PlayerControllerWithCC>().EnableRegen((curSlot.item as ManaRegenItem).manaRegenInterval,
            (curSlot.item as ManaRegenItem).duration);       
        CheckIfSlotNull();
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
        CheckIfSlotNull();
    }
    void AddBullets()
    {
        foreach (Weapon weapon in gameObject.GetComponent<WeaponManager>().weapons) 
        {
            if (weapon as SimpleBow) 
            {
                curSlot.amount -= 1;
                (weapon as SimpleBow).AddBullets((curSlot.item as BulletItem).bulletAmmount);
                if (weapon == gameObject.GetComponent<WeaponManager>().weapons[gameObject.GetComponent<WeaponManager>().currentWeaponIndex]) 
                {
                    (weapon as SimpleBow).UpdateAmmo((weapon as SimpleBow).CountOfBulletsInWeapon, 
                        (weapon as SimpleBow).CountOfBulletsInBackpack);
                }               
                CheckIfSlotNull();
            }
        }      
    }
    void CheckIfSlotNull() 
    {
        if (curSlot.amount < 1)
        {
            curSlot.item = null;
            curSlot.isEmpty = true;
            curSlot.iconGO.GetComponent<Image>().color = new Color(1, 1, 1, 1);
            curSlot.SetBasedIcon();
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
        if (durabilDatabase != null && durabilDatabase.OnChangeValues != null) 
        {
            durabilDatabase.OnChangeValues -= IfDurabilChanged;
            durabilDatabase = null;
        }
        if (curSlot != null) 
        {
            curSlot.OnChangeItems -= IfSlotItemChanged;
        }        
        HideAllTexts();
        curSlot = null;
        statsPanel.GetComponent<StatsList>().useButton.GetComponent<Button>().onClick.RemoveAllListeners();
        statsPanel.GetComponent<StatsList>().useButton.SetActive(false);
        statsPanel.SetActive(false);
    }
    void GetDurabilityValue(StatsList statsPanelValues, int defenseID) 
    {
        statsPanelValues.defenseItemTextPanel.transform.GetChild(1).GetComponent<TMP_Text>().text =
                                    durabilDatabase.GetValueByID(defenseID).ToString(); //defense cur ammount  
    }
    void HideAllTexts() 
    {
        statsPanel.GetComponent<StatsList>().fooodItemTextPanel.SetActive(false);
        statsPanel.GetComponent<StatsList>().charmItemTextPanel.SetActive(false);
        statsPanel.GetComponent<StatsList>().defenseItemTextPanel.SetActive(false);
        statsPanel.GetComponent<StatsList>().weaponItemTexts.SetActive(false);
        statsPanel.GetComponent<StatsList>().bulletItemTexts.SetActive(false);
        statsPanel.GetComponent<StatsList>().manaPotionItemTexts.SetActive(false);
    }
    void IfDurabilChanged() 
    {
        if (statsPanel.activeSelf) 
        {
            StatsList statsPanelValues = statsPanel.GetComponent<StatsList>();
            if (statsPanelValues.defenseItemTextPanel.activeSelf)
            {
                GetDurabilityValue(statsPanelValues, curSlot.defenseID);
            }
        }
    }
    void IfSlotItemChanged() 
    {
        CloseStatsPanel();
    }
}
