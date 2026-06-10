using System.Collections.Generic;
using UnityEngine;
using Indigo.Customers;
using Indigo.UI;

namespace Indigo.Integration
{
    /// <summary>
    /// Stoji pored Amarove CoffeeMachineSlot. Igrač pritiskom 1-4 bira tip pića
    /// dok je u trigger zoni, a kad mašina napuni šolje, ova skripta im dodjeljuje izabrani drinkType.
    /// </summary>
    public class DrinkSelectorStation : MonoBehaviour
    {
        [Tooltip("Referenca na Amarov CoffeeMachineSlot.")]
        public CoffeeMachineSlot machine;

        [Tooltip("Tipke 1-4 → Espresso, Cappuccino, Latte, Americano (poredak prema OrderType enum-u).")]
        public KeyCode[] selectKeys = { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4 };

        public OrderType Selected { get; private set; } = OrderType.Espresso;

        private bool _playerNear;
        private readonly HashSet<CupOrderBridge> _assignedThisBatch = new HashSet<CupOrderBridge>();

        void Update()
        {
            if (UIEvents.IsPaused) return;

            if (_playerNear) HandleSelectKeys();

            AssignDrinkTypeToBrewedCups();
        }

        void HandleSelectKeys()
        {
            for (int i = 0; i < selectKeys.Length && i < 4; i++)
            {
                if (Input.GetKeyDown(selectKeys[i]))
                {
                    Selected = (OrderType)i;
                    UIEvents.SetPrompt("", "Selected: " + Selected.DisplayName());
                }
            }
        }

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
            UIEvents.SetPrompt("1-4", "Select drink (" + Selected.DisplayName() + ")");
        }

        void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            _playerNear = false;
            UIEvents.ClearPrompt();
        }
    }
}
