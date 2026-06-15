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
        private bool _promptShown = false;

        void Update()
        {
            if (!_playerNear) return;

            if (!_promptShown)
            {
                UIEvents.SetPrompt("E", "ACTIVATE MAX LEVEL & FUNDS");
                _promptShown = true;
            }

            if (Input.GetKeyDown(interactKey))
            {
                // 1. Dodavanje novca
                MoneyManager.AddTip(cheatAmount, transform.position + Vector3.up * 1.5f, Satisfaction.Happy);
                
                // 2. Dodavanje ogromne količine XP-a kroz tvoju AddXPP funkciju
                PlayerProgression.AddXp(cheatXPAmount);
                
                Debug.Log($"[DEV CHEAT]: Max nivo aktiviran! Dodijeljeno ${cheatAmount} i {cheatXPAmount} XP.");
                
                UIEvents.SetPrompt("E", "MAX LEVEL ACTIVATED!");
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _playerNear = true;
                _promptShown = false;
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _playerNear = false;
                if (_promptShown)
                {
                    UIEvents.ClearPrompt();
                    _promptShown = false;
                }
            }
        }

        void OnDisable()
        {
            if (_promptShown)
            {
                UIEvents.ClearPrompt();
                _promptShown = false;
            }
        }
    }
}