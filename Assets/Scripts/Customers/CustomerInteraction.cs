using UnityEngine;
using Indigo.UI;

namespace Indigo.Customers
{
    [RequireComponent(typeof(Customer))]
    public class CustomerInteraction : MonoBehaviour
    {
        [Tooltip("Ako je true, igrač mora držati tačan item (npr. 'Espresso') da bi dostavio. Ako je false, dostava ide samim E pritiskom radi testiranja.")]
        public bool requireHeldDrink = true;

        [Tooltip("KeyCode za interakciju (preuzimanje narudžbe / dostava).")]
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
            if (_customer.TryDeliver(t))
            {
                DestroyHeldCup();
                UIEvents.ClearHeldItem();
            }
        }

        void DestroyHeldCup()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player == null) return;
            var bridges = player.GetComponentsInChildren<Indigo.Integration.CupOrderBridge>(true);
            for (int i = 0; i < bridges.Length; i++)
            {
                if (bridges[i] != null) Destroy(bridges[i].gameObject);
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
