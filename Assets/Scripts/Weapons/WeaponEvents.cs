using UnityEngine;
using UnityEngine.Events;

namespace Objects.Weapon
{
    public class WeaponEvents : MonoBehaviour
    {
        public static UnityEvent<int, int> OnAmmoChanged = new UnityEvent<int, int>();
        public static UnityEvent<string> OnFireballAmmoChanged = new UnityEvent<string>();
        public static UnityEvent<string> OnSwordAmmoChanged = new UnityEvent<string>();
        
        public static UnityEvent OnClearAmmo = new UnityEvent();
    }
}