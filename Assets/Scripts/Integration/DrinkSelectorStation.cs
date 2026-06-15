using System.Collections.Generic;
using UnityEngine;
using Indigo.Customers;
using Indigo.UI;

namespace Indigo.Integration
{
    public class DrinkSelectorStation : MonoBehaviour
    {
        [Tooltip("Referenca na Amarov CoffeeMachineSlot.")]
        public CoffeeMachineSlot machine;

        [Tooltip("Tipke 1-4 → Espresso, Cappuccino, Latte, Americano.")]
        public KeyCode[] selectKeys = { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4 };

        public OrderType Selected { get; private set; } = OrderType.Espresso;

        private bool _playerNear;
        private readonly HashSet<CupOrderBridge> _assignedThisBatch = new HashSet<CupOrderBridge>();

        void Update()
        {
            if (UIEvents.IsPaused) return;

            if (_playerNear) HandleSelectKeys();

            AssignDrinkTypeToBrewedCups();
            
            if (machine != null && machine.cupSlots != null)
            {
                bool hasFilledCup = false;
                foreach (var slot in machine.cupSlots)
                {
                    if (slot != null && slot.childCount > 0)
                    {
                        var cup = slot.GetComponentInChildren<CupPickup>();
                        if (cup != null && cup.isFilled) hasFilledCup = true;
                    }
                }
                if (!hasFilledCup) _assignedThisBatch.Clear();
            }
        }

        void HandleSelectKeys()
        {
            for (int i = 0; i < selectKeys.Length && i < 4; i++)
            {
                if (Input.GetKeyDown(selectKeys[i]))
                {
                    Selected = (OrderType)i;
                    
                    // Ostavljamo ispis samo kao brzu potvrdu kad pritisneš tipku!
                    string formattedText = $"   Selected: {Selected.DisplayName()}   ";
                    UIEvents.SetPrompt("OK", formattedText);
                }
            }
        }

        // --- VRAĆENA FUNKCIJA KOJA JE NEDOSTAJALA ---
        void AssignDrinkTypeToBrewedCups()
        {
            if (machine == null || machine.cupSlots == null) return;
            for (int i = 0; i < machine.cupSlots.Length; i++)
            {
                var slot = machine.cupSlots[i];
                if (slot == null || slot.childCount == 0) continue;
                var cup = slot.GetComponentInChildren<CupPickup>();
                if (cup == null || !cup.isFilled) continue;
                var bridge = cup.GetComponent<CupOrderBridge>();
                if (bridge == null) continue;
                if (_assignedThisBatch.Contains(bridge)) continue;
                bridge.AssignDrinkType(Selected);
                _assignedThisBatch.Add(bridge);
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            _playerNear = true;
        }

        void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            _playerNear = false;
        }
    }
}