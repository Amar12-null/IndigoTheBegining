using UnityEngine;
using Indigo.Customers;
using Indigo.UI;

namespace Indigo.Integration
{
    /// <summary>
    /// Bridge između Amarovog CupPickup-a i mog NPC sistema.
    /// Kači se na cup prefab pored CupPickup-a. Ne mijenja njegovu logiku.
    /// </summary>
    [RequireComponent(typeof(CupPickup))]
    public class CupOrderBridge : MonoBehaviour
    {
        [Tooltip("Tip pića koji ova šolja predstavlja. Postavlja se ručno na prefabu ili dinamički od DrinkSelectorStation-a kad se skuha.")]
        public OrderType drinkType = OrderType.Espresso;

        [Tooltip("Da li se drinkType zaključava nakon što ga DrinkSelector postavi.")]
        public bool autoLockOnAssigned = true;

        public bool IsDrinkTypeAssigned { get; private set; }

        private CupPickup _cup;
        private bool _lastIsPickedUp;
        private bool _wasHeldByUs;

        void Awake()
        {
            _cup = GetComponent<CupPickup>();
        }

        public void AssignDrinkType(OrderType t)
        {
            if (IsDrinkTypeAssigned && autoLockOnAssigned) return;
            drinkType = t;
            IsDrinkTypeAssigned = true;
        }

        void Update()
        {
            if (_cup == null) return;

            bool nowPicked = _cup.isPickedUp;
            
            if (nowPicked && !_lastIsPickedUp)
            {
                string state = _cup.isFilled ? "Ready" : "Empty";
                UIEvents.SetHeldItem(drinkType.DisplayName(), state);
                _wasHeldByUs = true;
            }
            // Ako je šolja spuštena na aparat, odmah očisti HUD!
            else if (!nowPicked && _lastIsPickedUp)
            {
                UIEvents.ClearHeldItem();
                _wasHeldByUs = false;
            }
            else if (nowPicked && _wasHeldByUs && _cup.isFilled
                     && UIEvents.CurrentHeldState != "Ready")
            {
                UIEvents.SetHeldItem(drinkType.DisplayName(), "Ready");
            }

            _lastIsPickedUp = nowPicked;
        }
        void OnDestroy()
        {
            if (_wasHeldByUs && UIEvents.CurrentHeldName == drinkType.DisplayName())
                UIEvents.ClearHeldItem();
        }
    }
}
