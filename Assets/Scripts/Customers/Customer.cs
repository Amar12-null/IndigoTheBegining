using UnityEngine;
using UnityEngine.AI;

namespace Indigo.Customers
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class Customer : MonoBehaviour
    {
        public enum State
        {
            Entering,
            Seeking,
            Sitting,
            WaitingForOrderTaken,
            WaitingForDrink,
            Drinking,
            Leaving,
            Done,
        }

        [Header("Refs")]
        public Animator animator;
        public Transform headAnchor;

        [Tooltip("Drag-dropuj 9 stickman prefabova ovdje. Customer će nasumično odabrati jedan kao vizual. Ako je prazno, koristi se postojeći child model.")]
        public GameObject[] visualVariants;

        [Header("Movement")]
        public float arrivalDistance = 0.4f;

        [Header("Behaviour")]
        public float drinkingDuration = 6f;
        public float leaveAfterNoTableDelay = 3f;

        [Header("Patience (seconds)")]
        [Tooltip("Koliko sekundi NPC čeka da igrač uzme narudžbu prije nego što ode ljut.")]
        public float maxOrderWait = 30f;

        [Tooltip("Koliko sekundi NPC čeka da igrač donese piće nakon uzimanja narudžbe.")]
        public float maxDeliveryWait = 45f;

        [Tooltip("Iznad ovog % preostalog strpljenja, NPC je Happy (puna napojnica).")]
        [Range(0f, 1f)] public float happyThreshold = 0.65f;

        [Tooltip("Iznad ovog % preostalog strpljenja (i ispod happy), NPC je Neutral (normalna napojnica).")]
        [Range(0f, 1f)] public float neutralThreshold = 0.30f;

        [Header("Order")]
        public OrderType[] possibleOrders = { OrderType.Espresso, OrderType.Cappuccino, OrderType.Latte, OrderType.Americano };

        public State CurrentState { get; private set; } = State.Entering;
        public OrderType Order { get; private set; }
        public Table CurrentTable { get; private set; }
        public bool OrderTaken { get; private set; }

        public float PatienceRemaining { get; private set; }
        public float PatienceMax { get; private set; }
        public float PatienceFraction => PatienceMax > 0f ? Mathf.Clamp01(PatienceRemaining / PatienceMax) : 0f;
        public Satisfaction CurrentSatisfaction => SatisfactionForFraction(PatienceFraction);

        private NavMeshAgent _agent;
        private Transform _exitPoint;
        private OrderBubble _bubble;
        private float _stateTimer;

        public void Init(Transform exitPoint)
        {
            _exitPoint = exitPoint;
            if (possibleOrders == null || possibleOrders.Length == 0)
                Order = OrderType.Espresso;
            else
                Order = possibleOrders[Random.Range(0, possibleOrders.Length)];
        }

        void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            SpawnRandomVisual();
            if (animator == null) animator = GetComponentInChildren<Animator>();
        }

        void SpawnRandomVisual()
        {
            if (visualVariants == null || visualVariants.Length == 0) return;
            var prefab = visualVariants[Random.Range(0, visualVariants.Length)];
            if (prefab == null) return;
            var visual = Instantiate(prefab, transform);
            visual.transform.localPosition = Vector3.zero;
            visual.transform.localRotation = Quaternion.identity;
        }

        void Start()
        {
            EnterShop();
        }

        void Update()
        {
            if (Indigo.UI.UIEvents.IsPaused) return;

            UpdateAnimator();

            switch (CurrentState)
            {
                case State.Entering: TickEntering(); break;
                case State.Seeking: TickSeeking(); break;
                case State.Sitting: TickSitting(); break;
                case State.WaitingForOrderTaken: TickWaitingForOrder(); break;
                case State.WaitingForDrink: TickWaitingForDrink(); break;
                case State.Drinking: TickDrinking(); break;
                case State.Leaving: TickLeaving(); break;
            }
        }

        void TickEntering()
        {
            _stateTimer -= Time.deltaTime;
            if (_stateTimer <= 0f) GoLeave(angry: false);
        }

        void EnterShop()
        {
            var table = TableManager.Instance != null ? TableManager.Instance.FindFreeTable() : null;
            if (table == null)
            {
                _stateTimer = leaveAfterNoTableDelay;
                CurrentState = State.Entering;
                return;
            }

            if (!table.TryReserve(this))
            {
                _stateTimer = leaveAfterNoTableDelay;
                CurrentState = State.Entering;
                return;
            }

            CurrentTable = table;
            _agent.SetDestination(table.SeatPosition);
            CurrentState = State.Seeking;
        }

        void TickSeeking()
        {
            if (CurrentTable == null) { GoLeave(angry: false); return; }
            if (!_agent.pathPending && _agent.remainingDistance <= arrivalDistance)
            {
                SitDown();
            }
        }

       void SitDown()
        {
            _agent.ResetPath();
            _agent.enabled = false; 

            transform.position = CurrentTable.SeatPosition;
            transform.rotation = CurrentTable.SeatRotation;
            CurrentState = State.Sitting;
            _stateTimer = 0.6f;
        }

        void TickSitting()
        {
            _stateTimer -= Time.deltaTime;
            if (_stateTimer <= 0f)
            {
                ShowOrderBubble();
                StartPatience(maxOrderWait + Indigo.Shop.Upgrades.PatienceBonusSeconds());
                CurrentState = State.WaitingForOrderTaken;
            }
        }

        void StartPatience(float duration)
        {
            PatienceMax = duration;
            PatienceRemaining = duration;
        }

        void TickPatience()
        {
            if (PatienceRemaining <= 0f) return;
            PatienceRemaining -= Time.deltaTime;
            if (PatienceRemaining < 0f) PatienceRemaining = 0f;
        }

        void TickWaitingForOrder()
        {
            TickPatience();
            if (PatienceRemaining <= 0f) GoLeave(angry: true);
        }

        void TickWaitingForDrink()
        {
            TickPayout(); // Ako imaš neku logiku za to
            TickPatience();
            if (PatienceRemaining <= 0f) GoLeave(angry: true);
        }

        void TickPayout() {}

        void TickDrinking()
        {
            _stateTimer -= Time.deltaTime;
            if (_stateTimer <= 0f) GoLeave(angry: false);
        }

        void TickLeaving()
        {
            if (_exitPoint == null) { Finish(); return; }
            if (!_agent.pathPending && _agent.remainingDistance <= arrivalDistance) Finish();
        }

        // --- POPRAVLJENO: SAKRIVANJE NARUDŽBE DOK JE IGRAČ NE UZME ---
        void ShowOrderBubble()
        {
            if (_bubble != null) return;
            _bubble = OrderBubble.Spawn(this);
            
            // Trik: U registar ulaznica upisujemo privremeno gosta, ali skripta 'OrderTicketsHUD'
            // će ga prepoznati kao "???" jer mu je stanje TicketState.Pending!
            OrderRegistry.Add(this, Order);
            
            // Ovdje ako tvoj OrderBubble ima opciju da se ugasi tekst/ikona, 
            // možeš je sakriti. Na primjer, ako prikazuje tekst, postavićemo ga na upitnik:
            var bubbleText = _bubble.GetComponentInChildren<UnityEngine.UI.Text>();
            if (bubbleText != null) 
            {
                bubbleText.text = "?";
                bubbleText.color = new Color(0.95f, 0.72f, 0.22f, 1f); // Žuta boja upitnika!
            }
        }

        void HideOrderBubble()
        {
            if (_bubble != null)
            {
                Destroy(_bubble.gameObject);
                _bubble = null;
            }
        }

        public bool TryTakeOrder()
        {
            if (CurrentState != State.WaitingForOrderTaken) return false;
            OrderTaken = true;
            CurrentState = State.WaitingForDrink;
            StartPatience(maxDeliveryWait + Indigo.Shop.Upgrades.PatienceBonusSeconds());
            
            // --- NOVO: REZERVACIJA I OTKRIVANJE PRAVE KAFE ---
            if (_bubble != null) 
            {
                _bubble.MarkOrderTaken();
                
                // Kada uzmeš narudžbu, ako bubble ispisuje tekst, otkrivamo pravo ime pića
                var bubbleText = _bubble.GetComponentInChildren<UnityEngine.UI.Text>();
                if (bubbleText != null) 
                {
                    bubbleText.text = Order.DisplayName();
                    bubbleText.color = Color.white; // Vraćamo u bijelo kad je aktivno
                }
            }
            
            OrderRegistry.SetState(this, OrderRegistry.TicketState.Taken);
            return true;
        }

        public bool TryDeliver(OrderType delivered)
        {
            if (CurrentState != State.WaitingForDrink) return false;
            
            if (delivered != Order) 
            {
                Debug.LogWarning($"Gost odbija piće! Naručio je: {Order}, a ti mu daješ: {delivered}");
                return false;
            }

            var sat = CurrentSatisfaction;
            int payout = ComputePayout(Order, sat);

            HideOrderBubble();
            CurrentState = State.Drinking;
            _stateTimer = drinkingDuration;
            OrderRegistry.Remove(this);

            if (payout > 0) MoneyManager.AddTip(payout, transform.position + Vector3.up * 2.0f, sat);

            return true;
        }

        public bool CanTakeOrder() => CurrentState == State.WaitingForOrderTaken;
        public bool CanDeliver()   => CurrentState == State.WaitingForDrink;

       void GoLeave(bool angry)
        {
            if (angry) MoneyManager.NotifyAngryLeave(transform.position + Vector3.up * 2.0f);
            HideOrderBubble();
            if (CurrentTable != null)
            {
                CurrentTable.Release(this);
                CurrentTable = null;
            }
            CurrentState = State.Leaving;
            
            _agent.enabled = true; 
            Vector3 pozicijaIzaStolice = transform.position - (transform.forward * 0.6f);
            _agent.Warp(pozicijaIzaStolice); 
            
            _agent.isStopped = false;
            if (_exitPoint != null) _agent.SetDestination(_exitPoint.position);
        }

        void Finish()
        {
            CurrentState = State.Done;
            HideOrderBubble();
            if (CurrentTable != null)
            {
                CurrentTable.Release(this);
                CurrentTable = null;
            }
            Destroy(gameObject);
        }

        void UpdateAnimator()
        {
            if (animator == null) return;
            float speed = _agent.velocity.magnitude;
            animator.SetFloat("Speed", speed);
            
            bool sitting = (CurrentState == State.Sitting || 
                            CurrentState == State.WaitingForOrderTaken || 
                            CurrentState == State.WaitingForDrink || 
                            CurrentState == State.Drinking);
            animator.SetBool("IsSitting", sitting);
        }

        Satisfaction SatisfactionForFraction(float frac)
        {
            float happyAdj = happyThreshold + Indigo.Shop.Upgrades.HappyThresholdAdjust();
            if (frac >= happyAdj) return Satisfaction.Happy;
            if (frac >= neutralThreshold) return Satisfaction.Neutral;
            if (frac > 0f) return Satisfaction.Annoyed;
            return Satisfaction.Angry;
        }

        int ComputePayout(OrderType order, Satisfaction sat)
        {
            float baseAmount = order.BasePrice();
            float multiplier = sat.TipMultiplier() * Indigo.Shop.Upgrades.TipMultiplier();
            return Mathf.Max(0, Mathf.RoundToInt(baseAmount * multiplier));
        }

        void OnDestroy()
        {
            if (CurrentTable != null) CurrentTable.Release(this);
            HideOrderBubble();
            OrderRegistry.Remove(this);
        }
    }
}