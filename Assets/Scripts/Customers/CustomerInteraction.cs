using UnityEngine;
using Indigo.UI;
using Indigo.Integration; // NOVO: Dodajemo namespace da skripta vidi CupOrderBridge!

namespace Indigo.Customers
{
    [RequireComponent(typeof(Customer))]
    public class CustomerInteraction : MonoBehaviour
    {
        [Tooltip("Ako je true, igrač mora držati tačan item da bi dostavio.")]
        public bool requireHeldDrink = true;

        [Tooltip("KeyCode za interakciju.")]
        public KeyCode interactKey = KeyCode.E;

        private Customer _customer;
        private bool _playerNear;
        private bool _promptShown;

        void Awake()
        {
            _customer = GetComponent<Customer>();
        }

        void Update()
        {
            if (!_playerNear) return;

            if (_customer.CanTakeOrder())
            {
                ShowPrompt("E", "Take order");
                if (Input.GetKeyDown(interactKey))
                {
                    _customer.TryTakeOrder();
                    RefreshPrompt();
                }
            }
            else if (_customer.CanDeliver())
            {
                if (requireHeldDrink)
                {
                    var held = UIEvents.CurrentHeldName;
                    if (!string.IsNullOrEmpty(held))
                    {
                        ShowPrompt("E", "Deliver " + _customer.Order.DisplayName());
                        if (Input.GetKeyDown(interactKey))
                            TryDeliverHeld(held);
                    }
                    else
                    {
                        ShowPrompt("", "Bring " + _customer.Order.DisplayName());
                    }
                }
                else
                {
                    ShowPrompt("E", "Deliver " + _customer.Order.DisplayName());
                    if (Input.GetKeyDown(interactKey))
                        _customer.TryDeliver(_customer.Order);
                }
            }
            else
            {
                ClearPromptIfMine();
            }
        }

        void TryDeliverHeld(string heldName)
        {
            if (!TryParseOrder(heldName, out var t)) return;
            
            // Prosljeđujemo stvarni tip kafe gostu
            if (_customer.TryDeliver(t))
            {
                DestroyHeldCup();
                UIEvents.ClearHeldItem();
            }
        }

        // --- POPRAVLJENO SIGURNO UNIŠTAVANJE ŠOLJICE IZ RUKE ---
        void DestroyHeldCup()
        {
            // Pronalazimo aparat za kafu na sceni kako bismo preko njega uzeli tačnu referencu na playerHand
            var machine = GameObject.FindAnyObjectByType<CoffeeMachineSlot>();
            if (machine != null && machine.playerHand != null)
            {
                // Tražimo šoljicu koja se nalazi direktno unutar igračevih ruku
                CupPickup heldCup = machine.playerHand.GetComponentInChildren<CupPickup>();
                if (heldCup != null)
                {
                    Destroy(heldCup.gameObject); // Uništavamo šoljicu iz ruke!
                    
                    // Oslobađamo desnu ruku u ArmOverride sistemu animacija
                    ArmOverride arm = machine.playerHand.GetComponentInParent<ArmOverride>();
                    if (arm != null) arm.isHoldingRight = false;
                    
                    Debug.Log("CustomerInteraction: Šoljica uništena i ruka je spuštena.");
                }
            }
        }

        static bool TryParseOrder(string heldName, out OrderType t)
        {
            t = default;
            if (string.IsNullOrEmpty(heldName)) return false;
            return System.Enum.TryParse(heldName, true, out t);
        }

        void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            _playerNear = true;
            RefreshPrompt();
        }

        void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            _playerNear = false;
            ClearPromptIfMine();
        }

        void RefreshPrompt() { _promptShown = false; }

        void ShowPrompt(string key, string text)
        {
            if (_promptShown && UIEvents.CurrentPromptText == text) return;
            UIEvents.SetPrompt(key, text);
            _promptShown = true;
        }

        void ClearPromptIfMine()
        {
            if (_promptShown)
            {
                UIEvents.ClearPrompt();
                _promptShown = false;
            }
        }

        void OnDisable()
        {
            ClearPromptIfMine();
        }
    }
}