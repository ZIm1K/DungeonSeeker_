using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Objects.Weapon
{
    public class AmmoDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text ammoText;

        private void OnEnable()
        {
            WeaponEvents.OnAmmoChanged.AddListener(UpdateAmmoText);
            WeaponEvents.OnFireballAmmoChanged.AddListener(UpdateFireballAmmoText);
            WeaponEvents.OnSwordAmmoChanged.AddListener(UpdateSwordAmmoText);

            WeaponEvents.OnClearAmmo.AddListener(ClearAmmoText);
        }

        private void OnDisable()
        {
            WeaponEvents.OnAmmoChanged.RemoveListener(UpdateAmmoText);
            WeaponEvents.OnFireballAmmoChanged.RemoveListener(UpdateFireballAmmoText);
            WeaponEvents.OnSwordAmmoChanged.RemoveListener(UpdateSwordAmmoText);

            WeaponEvents.OnClearAmmo.RemoveListener(ClearAmmoText);
        }

        private void UpdateAmmoText(int currentAmmo, int ammoInBackpack)
        {
            ammoText.text = $"{currentAmmo}/{ammoInBackpack}";
        }

        private void UpdateFireballAmmoText(string ammoText)
        {
            this.ammoText.text = $"{ammoText}";
        }
        private void UpdateSwordAmmoText(string ammoText)
        {
            this.ammoText.text = $"{ammoText}";
        }
        private void ClearAmmoText()
        {
            ammoText.text = " ";
        }
    }
}