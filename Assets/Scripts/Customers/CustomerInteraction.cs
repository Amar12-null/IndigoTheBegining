using UnityEngine;
using Indigo.UI;
using Indigo.Integration; 


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
                    
                    CupPickup rightHandCup = null;
                    CupPickup trayCup = null;

                    var machine = FindAnyObjectByType<CoffeeMachineSlot>();
                    if (machine != null && machine.playerHand != null)
                    {
                        rightHandCup = machine.playerHand.GetComponentInChildren<CupPickup>();
                    }

                    var tray = FindAnyObjectByType<ServingTray>();
                    if (tray != null && tray.isPickedUp)
                    {
                        // Skeniramo tacnu da vidimo ima li kafa koja odgovara gostu!
                        foreach (Transform slot in tray.traySlots)
                        {
                            if (slot.childCount > 0)
                            {
                                CupPickup cup = slot.GetComponentInChildren<CupPickup>();
                                CupOrderBridge bridge = cup?.GetComponent<CupOrderBridge>();
                                
                                if (bridge != null && bridge.drinkType == _customer.Order && cup.isFilled)
                                {
                                    trayCup = cup;
                                    break; 
                                }
                            }
                        }
                    }

                    // Provjeravamo da li desna ruka drži pravu kafu
                    bool rightHandCorrect = false;
                    if (rightHandCup != null && rightHandCup.isFilled)
                    {
                        var bridge = rightHandCup.GetComponent<CupOrderBridge>();
                        if (bridge != null && bridge.drinkType == _customer.Order) rightHandCorrect = true;
                    }

                    // --- LOGIKA ZA PRIKAZ I DOSTAVU ---
                    if (rightHandCorrect)
                    {
                        ShowPrompt("E", "Deliver " + _customer.Order.DisplayName());
                        if (Input.GetKeyDown(interactKey)) TryDeliverSpecificCup(rightHandCup, true);
                    }
                    else if (trayCup != null)
                    {
                        // NOVO: Prepoznaje kafu na tacni i nudi direktnu dostavu!
                        ShowPrompt("E", "Deliver " + _customer.Order.DisplayName() );
                        if (Input.GetKeyDown(interactKey)) TryDeliverSpecificCup(trayCup, false);
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

        
        void TryDeliverSpecificCup(CupPickup cup, bool wasInRightHand)
        {
            if (_customer.TryDeliver(_customer.Order))
            {
                Destroy(cup.gameObject); // Brišemo specifičnu šolju (bila ona u ruci ili na tacni)

                if (wasInRightHand)
                {
                    var machine = FindAnyObjectByType<CoffeeMachineSlot>();
                    if (machine != null && machine.playerHand != null)
                    {
                        ArmOverride arm = machine.playerHand.GetComponentInParent<ArmOverride>();
                        if (arm != null) arm.isHoldingRight = false;
                    }
                    UIEvents.ClearHeldItem();
                }
                
                // Opcionalno zvučni efekat predaje kafe (koristimo "Place" zvuk)
                if (Indigo.Audio.SoundManager.Instance != null)
                    Indigo.Audio.SoundManager.Instance.Play(Indigo.Audio.SfxId.Place);
            }
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