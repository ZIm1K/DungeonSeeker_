using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObjects.Weapons
{
    [CreateAssetMenu(fileName = "Sword", menuName = "Weapon/sword", order = 1)]
    public class SwordData : ScriptableObject
    {
        public int swordDamage = 30;
        public float shotTimeout = 1.5f;
        public AudioClip attackSound;
    }
}
