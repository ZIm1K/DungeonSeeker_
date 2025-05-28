using Inventory;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Objects.PlayerScripts
{
    public class CharacterModel : MonoBehaviourPunCallbacks
    {
        [SerializeField] private int health;
        [SerializeField] private int maxHealth;

        [SerializeField] private float healthRegenInterval = 2f;
        [SerializeField] private int healthaRegenAmount = 1;

        private int defense;

        [SerializeField] int helmetDefense;
        [SerializeField] private int armorDefense;
        [SerializeField] private int bootsDefense;

        [SerializeField] private int helmetID;
        [SerializeField] private int armorID;
        [SerializeField] private int bootsID;

        public DurabilityDefenseDatabase durabilDatabase;

        [SerializeField] private int mana;
        [SerializeField] private int maxMana;

        [SerializeField] private int manaRegenAmount = 1;
        [SerializeField] private float manaRegenInterval = 0.5f;
        private float curManaRegenInterval;
        private float timer;

        private float speed;
        private float jumpForce;
        private CharacterView view;
        private PlayerControllerWithCC playerController;

        private float currentMultiplier;

        private bool isAlive = true;

        public int Health
        {
            get { return health; }
            set
            {
                health = value;
                view.UpdateHealthText(health);
            }
        }
        public int MaxHealth
        {
            get { return maxHealth; }
            set
            {
                maxHealth = value;
            }
        }
        public int Defense
        {
            get { return defense; }
            set
            {
                defense = value;
                view.UpdateDefenseText(defense);
            }
        }
        public int HelmetDefense
        {
            get { return helmetDefense; }
            set
            {
                if (value < 0)
                {
                    helmetDefense = 0;
                }
                else
                {
                    helmetDefense = value;
                }
            }
        }
        public int ArmorDefense
        {
            get { return armorDefense; }
            set
            {
                if (value < 0)
                {
                    armorDefense = 0;
                }
                else
                {
                    armorDefense = value;
                }
            }
        }
        public int BootsDefense
        {
            get { return bootsDefense; }
            set
            {
                if (value < 0)
                {
                    bootsDefense = 0;
                }
                else
                {
                    bootsDefense = value;
                }
            }
        }
        public int Mana
        {
            get { return mana; }
            set
            {
                mana = value;
                view.UpdateManaText(mana);
            }
        }
        public int MaxMana
        {
            get { return maxMana; }
            set
            {
                maxMana = value;
            }
        }
        public float Speed
        {
            get { return speed; }
            set
            {
                speed = value;
                view.UpdateSpeedText(speed);
            }
        }
        public float JumpForce
        {
            get { return jumpForce; }
            set
            {
                jumpForce = value;
                view.UpdateJumpForceText(jumpForce);
            }
        }
        public void Initialize(int health, int mana, CharacterView view, float speed, PlayerControllerWithCC playerController,
     float jumpForce, DurabilityDefenseDatabase durabilDatabase)
        {
            //int level = LevelHandler.Level;
            //currentMultiplier = Mathf.Pow(1.03f, level - 1);

            //maxHealth = Mathf.RoundToInt(health * currentMultiplier); //don`t need to buff speed because of overpower
            //maxMana = Mathf.RoundToInt(mana * currentMultiplier); //don`t need to buff speed because of overpower
            maxHealth = health;
            maxMana = mana;
            this.health = maxHealth;
            this.mana = maxMana;
            this.speed = speed;
            this.jumpForce = jumpForce;

            //this.speed = speed * (1 + 0.05f * (level - 1));   //don`t need to buff speed because of overpower
            //this.jumpForce = jumpForce * (1 + 0.03f * (level - 1));   //don`t need to buff jump force because of overpower

            this.view = view;
            this.playerController = playerController;
            this.durabilDatabase = durabilDatabase;
            view.HealthBar.maxValue = maxHealth;
            view.ManaBar.maxValue = mana;
            curManaRegenInterval = manaRegenInterval;
            UpdateAllStats();

            StartCoroutine(RegenerateMana());
            StartCoroutine(RegenerateHealth());
        }


        [PunRPC]
        public void WearDefense(ItemScriptableObject item, int ID)
        {
            switch (item)
            {
                case HelmetItem:
                    {
                        helmetID = ID;
                        HelmetDefense += durabilDatabase.GetValueByID(helmetID);
                        break;
                    }
                case ArmorItem:
                    {
                        armorID = ID;
                        ArmorDefense += durabilDatabase.GetValueByID(armorID);
                        break;
                    }
                case BootsItem:
                    {
                        bootsID = ID;
                        BootsDefense += durabilDatabase.GetValueByID(bootsID);
                        break;
                    }
            }
            Defense = HelmetDefense + ArmorDefense + BootsDefense;
        }
        [PunRPC]
        public int UnWearDefense(ItemScriptableObject item)
        {
            int returnID = 0;
            switch (item)
            {
                case HelmetItem:
                    {
                        returnID = helmetID;
                        HelmetDefense = 0;
                        helmetID = 0;
                        Defense = ArmorDefense + BootsDefense;
                        break;
                    }
                case ArmorItem:
                    {
                        returnID = armorID;
                        ArmorDefense = 0;
                        armorID = 0;
                        Defense = HelmetDefense + BootsDefense;
                        break;
                    }
                case BootsItem:
                    {
                        returnID = bootsID;
                        BootsDefense = 0;
                        bootsID = 0;
                        Defense = ArmorDefense + BootsDefense;
                        break;
                    }
            }
            return returnID;
        }
        [PunRPC]
        public void BuffSpeed(float ammount)
        {
            Speed += ammount;
            playerController.UpdateSpeed(Speed);
        }
        [PunRPC]
        public void NerfSpeed(float ammount)
        {
            Speed -= ammount;
            playerController.UpdateSpeed(Speed);
        }

        [PunRPC]
        public void BuffJumpForce(float ammount)
        {
            JumpForce += ammount;
            playerController.UpdateJumpForce(JumpForce);
        }
        [PunRPC]
        public void NerfJumpForce(float ammount)
        {
            JumpForce -= ammount;
            playerController.UpdateJumpForce(JumpForce);
        }

        [PunRPC]
        public void TakeDamage(int damage)
        {
            if (!photonView.IsMine) return;

            if (Defense > 0)
            {
                if (HelmetDefense > 0)
                {
                    HelmetDefense = SubtractTypeDefense(HelmetDefense, damage);
                    durabilDatabase.SubDurabilAmmount(helmetID, damage);
                }
                else if (ArmorDefense > 0)
                {
                    ArmorDefense = SubtractTypeDefense(ArmorDefense, damage);
                    durabilDatabase.SubDurabilAmmount(armorID, damage);
                }
                else if (BootsDefense > 0)
                {
                    BootsDefense = SubtractTypeDefense(BootsDefense, damage);
                    durabilDatabase.SubDurabilAmmount(bootsID, damage);
                }

                Defense = HelmetDefense + ArmorDefense + BootsDefense;
            }
            else
            {
                if (Health > 0) 
                {
                    Health = Mathf.Max(Health - damage, 0);
                    if (Health == 0)
                    {
                        DisconectManager.disconectInstance.ChangingScenes(3);
                    }
                }               
            }
        }

        private void Die() 
        {
            DisconectManager.disconectInstance.ChangingScenes(3);
        }       

        public int SubtractTypeDefense(int defense, int damage) 
        {
            if (defense > damage)
            {
                defense -= damage;
            }
            else
            {
                defense = 0;
            }
            return defense;
        }

        public void UpdateAllStats()
        {
            view.UpdateHealthText(Health);
            view.UpdateManaText(Mana);
            view.UpdateDefenseText(Defense);
            view.UpdateSpeedText(Speed);
            view.UpdateJumpForceText(JumpForce);
            view.UpdateLevelText(LevelHandler.Level);
        }


        [PunRPC]
        public void Heal(int heal)
        {
            Health += heal;
            Debug.Log("Player healed " + heal + " health. Health is now " + health);
        }

        public void SpendMana(int count)
        {
            Mana -= count;
            Debug.Log($"Player spend {count} of mana");
        }

        public void AddMana(int count)
        {
            Mana += count;
            Debug.Log($"Player added {count} of mana");
        }
        public void AddHealth(int count)
        {
            Health += count;
            Debug.Log($"Player added {count} of health");
        }
        private IEnumerator RegenerateMana()
        {
            while (true)
            {
                yield return new WaitForSeconds(curManaRegenInterval);
                if (Mana < MaxMana)
                {
                    int newMana = Mathf.Min(Mana + manaRegenAmount, maxMana);
                    AddMana(newMana - Mana);
                }
            }
        }
        private IEnumerator RegenerateHealth()
        {
            while (true)
            {
                yield return new WaitForSeconds(healthRegenInterval);
                if (Health < maxHealth)
                {
                    int newHealth = Mathf.Min(Health + healthaRegenAmount, maxHealth);
                    AddHealth(newHealth - Health);
                }
            }
        }
        public void EnableRegen(float manaRegenInterval, float duration)
        {
            if (curManaRegenInterval == manaRegenInterval) //if poition had the same buff
            {
                if (timer < 1)
                {
                    curManaRegenInterval = manaRegenInterval;
                    StartCoroutine(WaitForDuration(duration));
                }
                else
                {
                    timer += duration;
                }
            }
            else
            {
                timer = duration;
                curManaRegenInterval = manaRegenInterval;
                StartCoroutine(WaitForDuration(duration));
            }
        }
        private IEnumerator WaitForDuration(float duration)
        {
            timer = duration;
            while (timer >= 0)
            {
                view.UpdateTimerText(timer);
                yield return new WaitForSeconds(1f);
                timer -= 1;
            }
            curManaRegenInterval = manaRegenInterval;
        }
        private void OnDisable()
        {
            List<Item> listOfDefense = new List<Item>(GameObject.FindObjectsOfType<Item>());
            foreach (Item item in listOfDefense) 
            {
                ItemType typeOfitem = item.item.itemType;
                if (typeOfitem == ItemType.Helmet || typeOfitem == ItemType.Armor || typeOfitem == ItemType.Boots) 
                {
                    DurabilityDefenseDatabase.instance.RemoveItemFromList((item as DefenseItem).ID - 1);
                }
            }
            DurabilityDefenseDatabase.instance.ClearNotNeededItems();
        }
    }    
}