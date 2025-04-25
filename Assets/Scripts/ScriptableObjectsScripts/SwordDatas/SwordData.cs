using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObjects.Weapons
{
    [CreateAssetMenu(fileName = "Sword", menuName = "Weapon/sword", order = 1)]
    public class SwordData : ScriptableObject
    {
        public int swordDamage = 15;
        public float shotTimeout = 0.3f;
        public string attackSoundPath;
        public float rangeOfAttack = 10;
        public string pathOfScObj;
        public AnimationClip animationClip;
    }
}
