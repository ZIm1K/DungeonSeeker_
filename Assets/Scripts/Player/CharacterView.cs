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
        public TMP_Text ManaBuffText;
        private float time;

        public void UpdateHealthText(int health)
        {
            HealthText.text = $"Health: {health}";
        }
        public void UpdateManaText(int mana)
        {
            ManaText.text = $"Mana: {mana}";
        }
        public void UpdateDefenseText(int defense) 
        {            
            DefenseText.text = $"Defense: {defense}";
        }
        public void UpdateSpeedText(float speed)
        {
            SpeedText.text = $"Speed: {speed}";
        }
        public void UpdateJumpForceText(float force)
        {
            JumpForceText.text = $"JumpForce: {force}";
        }
        public void UpdateTimerText(float time)
        {
            if (time > 0)
            {
                ManaBuffText.gameObject.SetActive(true);                              
            }
            else 
            {
                ManaBuffText.gameObject.SetActive(false);
                return;
            }
            ManaBuffText.text = $"ManaBuff: {time}";
        }
    }
}