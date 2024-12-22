using Inventory;
using Photon.Pun;
using System.Net;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Objects.PlayerScripts
{
    public class CharacterModel : MonoBehaviourPun
    {
        public static CharacterModel Instance;
        [SerializeField] private int health;
        private int defense;
        
        [SerializeField] int helmetDefense;
        [SerializeField] private int armorDefense;
        [SerializeField] private int bootsDefense;

        [SerializeField] private int helmetID;
        [SerializeField] private int armorID;
        [SerializeField] private int bootsID;

        [SerializeField] private DurabilityDefenseDatabase durabilDatabase;

        [SerializeField] private int mana;//
        [SerializeField] private float speed;//
        [SerializeField] private float jumpForce;//
        [SerializeField] private CharacterView view;//
        [SerializeField] private PlayerControllerWithCC playerController;//

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(Instance);
            }
            else
            {
                Instance = this;
            }
        }        
        public int Health
        {
            get { return health; }
            set
            {               
                health = value;                         
                view.UpdateHealthText(health);
            }
        }
        public int Defense
        {
            get { return defense; }
            set
            {
                defense = HelmetDefense + ArmorDefense + BootsDefense;
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
            this.health = health;
            this.mana = mana;
            this.view = view;
            this.speed = speed;
            this.playerController = playerController;
            this.jumpForce = jumpForce;
            this.durabilDatabase = durabilDatabase;
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
            Debug.LogWarning("I can take damage");
            if (!photonView.IsMine) return;
            Debug.LogWarning("Me is me");
            if (Defense > 0)
            {
                if (HelmetDefense > 0)
                {                    
                    HelmetDefense = SubtractTypeDefense(HelmetDefense, damage);
                    durabilDatabase.SubDurabilAmmount(helmetID,damage);
                }                
                else if (ArmorDefense > 0)
                {                   
                    ArmorDefense = SubtractTypeDefense(ArmorDefense, damage);
                    durabilDatabase.SubDurabilAmmount(armorID, damage);
                }
                else if (BootsDefense > 0) 
                {                   
                    BootsDefense = SubtractTypeDefense(BootsDefense,damage);
                    durabilDatabase.SubDurabilAmmount(bootsID, damage);
                }   
                Defense = HelmetDefense + ArmorDefense + BootsDefense;
            }
            else 
            {
                if (Health > damage)
                {
                    Health -= damage;
                }
                else 
                {
                    Health = 0;
                }
            }
            Debug.LogWarning("Damaged");
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

        [PunRPC]
        public void Heal(int heal)
        {
            Health += heal;
            Debug.Log("Player healed " + heal + " health. Health is now " + health);
        }

        [PunRPC]
        public void SpendMana(int count)
        {
            Mana -= count;
            Debug.Log($"Player spend {count} of mana");
        }

        [PunRPC]
        public void AddMana(int count)
        {
            Mana += count;
            Debug.Log($"Player added {count} of mana");
        }
    }
}