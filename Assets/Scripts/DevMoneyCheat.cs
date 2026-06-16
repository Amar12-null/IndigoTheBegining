using UnityEngine;
using Indigo.UI;
using Indigo.Customers;   
using Indigo.Progression; 

namespace Indigo.DevTools
{
    public class DevMoneyCheat : MonoBehaviour
    {
       
        
        public int cheatAmount = 50000; // Povećano na 50 hiljada da imaš za sve odjednom!

       
        public int cheatXPAmount = 50000; // Povećano sa 5000 na 50000 za instant Max Level!

        
        public KeyCode interactKey = KeyCode.E;

        private bool _playerNear = false;
       

        void Update()
        {
            if (!_playerNear) return;

            

            if (Input.GetKeyDown(interactKey))
            {
                // 1. Dodavanje novca
                MoneyManager.AddTip(cheatAmount, transform.position + Vector3.up * 1.5f, Satisfaction.Happy);
                
                // 2. Dodavanje ogromne količine XP-a kroz tvoju AddXPP funkciju
                PlayerProgression.AddXp(cheatXPAmount);
                
                Debug.Log($"[DEV CHEAT]: Max nivo aktiviran! Dodijeljeno ${cheatAmount} i {cheatXPAmount} XP.");
                
                
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _playerNear = true;
                
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _playerNear = false;
                
            }
        }

        
    }
}