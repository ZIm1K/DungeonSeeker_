using Photon.Pun;
using System.Runtime.Serialization;
using UnityEngine;

namespace Objects.Enemies
{
    public class EnemyModel : MonoBehaviourPun
    {
        private int damage;
        private int health;
        private float attackRange;
        private float attackInterval;

        private EnemyView view;

        [SerializeField] private int level = 1;

        public int Level
        {
            get => level;
            set
            {
                level = value;
                ApplyLevelBonuses();
                UpdateAllStats();
            }
        }

        public float AttackRange { get; set; }
        public float AttackInterval { get; set; }

        public int Damage
        {
            get => damage;
            set
            {
                damage = value;
                view?.UpdateDamageText(damage);
            }
        }

        public int Health
        {
            get => health;
            set
            {
                health = value;
                view?.UpdateHealthText(health);
            }
        }

        public void Initialize(int baseHealth, int baseDamage, float attackRange, float attackInterval, EnemyView view, int level = 1)
        {
            this.view = view;
            this.attackRange = attackRange;
            this.attackInterval = attackInterval;
            this.level = level;

            float multiplier = Mathf.Pow(1.02f, level - 1);
            Health = Mathf.RoundToInt(baseHealth * multiplier);
            Damage = Mathf.RoundToInt(baseDamage * multiplier);
        }

        private void ApplyLevelBonuses()
        {
            float multiplier = Mathf.Pow(1.02f, level - 1);
            Health = Mathf.RoundToInt(health * multiplier);
            Damage = Mathf.RoundToInt(damage * multiplier);
        }

        private void UpdateAllStats()
        {
            view?.UpdateHealthText(health);
        }
        [PunRPC]
        private void TakeDamage(int damage) 
        {
            if (Health - damage > 0)
            {
                Health -= damage;
            }
            else 
            {
                GameObject.FindGameObjectWithTag("EnemyKillCounter").GetComponent<EnemyKillCount>().NewEnemyKilled();
                PhotonNetwork.Destroy(gameObject);               
            }
        }
    }
}
