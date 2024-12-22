using Photon.Pun;
using System.Data.Common;
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
        
        public float AttackRange { get; set; }
        public float AttackInterval { get; set; }
        public int Damage
        {
            get { return damage; }
            set
            {
                damage = value;
                view.UpdateDamageText(damage);
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
        
        public void Initialize(int health, int damage, float attackRange, float attackInterval, EnemyView view)
        {
            this.health = health;
            this.damage = damage;
            this.attackRange = attackRange;
            this.attackInterval = attackInterval;
            this.view = view;
        }

        [PunRPC]
        public void TakeDamage(int damage)
        {
            if (Health > damage)
            {
                Health -= damage;
            }
            else 
            { 
                Health = 0;
            }
            Debug.Log("Enemy took " + damage + " damage. Health is now " + health);
        }

        [PunRPC]
        public void Heal(int heal)
        {
            Health += heal;
            Debug.Log("Enemy healed " + heal + " health. Health is now " + health);
        }
    }
}
