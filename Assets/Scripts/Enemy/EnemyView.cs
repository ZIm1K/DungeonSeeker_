using TMPro;
using UnityEngine;

namespace Objects.Enemies
{
    public class EnemyView : MonoBehaviour
    {
        public TMP_Text HealthText;
        public TMP_Text DamageText;

        public void UpdateHealthText(int health)
        {
            HealthText.text = $"Health: {health}";
        }
        
        public void UpdateDamageText(int damage)
        {
            DamageText.text = $"Damage: {damage}";
        }
    }
}
