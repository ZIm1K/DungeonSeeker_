using Inventory;
using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Objects.PlayerScripts
{
    public class CharacterView : MonoBehaviour
    {
        public TMP_Text HealthText;
        public TMP_Text ManaText;
        public TMP_Text DefenseText;
        public TMP_Text SpeedText;
        public TMP_Text JumpForceText;
        public GameObject ManaBuffEmpty;
        public TMP_Text ManaBuffText;
        public Slider HealthBar;
        public Slider ManaBar;
        private float time;      
        public TMP_Text Level;       

        public void UpdateLevelText(int level)
        {
            Level.text = $"{level}";
        }
        public void UpdateHealthText(int health)
        {
            HealthText.text = $"Health: {health}";
            HealthBar.value = health;
        }
        public void UpdateManaText(int mana)
        {
            ManaText.text = $"Mana: {mana}";
            ManaBar.value = mana;
        }
        public void UpdateDefenseText(int defense) 
        {            
            DefenseText.text = $"{defense}";
        }
        public void UpdateSpeedText(float speed)
        {
            SpeedText.text = $"{speed}";
        }
        public void UpdateJumpForceText(float force)
        {
            JumpForceText.text = $"{force}";
        }
        public void UpdateTimerText(float time)
        {
            if (time > 0)
            {
                ManaBuffEmpty.gameObject.SetActive(true);                              
            }
            else 
            {
                ManaBuffEmpty.gameObject.SetActive(false);
                return;
            }
            ManaBuffText.text = $"{time}";
        }
    }
}