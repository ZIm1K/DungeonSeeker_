using UnityEngine;

namespace ScriptableObjects.Weapons
{
    [CreateAssetMenu(fileName = "Pistol", menuName = "Weapon/pistol", order = 1)]
    public class PistolData: ScriptableObject
    {
        public int pistolDamage = 10;
        public GameObject hitObjectPrefab;
        public GameObject decalPrefab;
        public Transform firePoint;
        public string shotSoundPath;
        public float shotTimeout = 0.5f;
        public float reloadTime = 2f;
        public int maxBulletsInWeapon;
    }
}