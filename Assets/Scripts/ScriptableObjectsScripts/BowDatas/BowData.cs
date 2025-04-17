using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObjects.Weapons
{
    [CreateAssetMenu(fileName = "Bow", menuName = "Weapon/bow", order = 1)]
    public class BowData : ScriptableObject
    {
        public int bowDamage = 10;
        public GameObject arrowPrefab;
        public float arrowSpeed;
        public GameObject decalArrow;
        public Transform firePoint;
        public string shotSoundPath;
        public float shotTimeout = 0.5f;
        public float reloadTime = 2f;
        public int maxBulletsInWeapon;
        public string pathOfScObj;
    }
}
