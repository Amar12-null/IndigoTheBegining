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

        [Header("Movement")]
        public float arrivalDistance = 0.4f;

        [Header("Behaviour")]
        public float drinkingDuration = 6f;
        public float leaveAfterNoTableDelay = 3f;

        [Header("Order")]
        public OrderType[] possibleOrders = { OrderType.Espresso, OrderType.Cappuccino, OrderType.Latte, OrderType.Americano };

        public State CurrentState { get; private set; } = State.Entering;
        public OrderType Order { get; private set; }
        public Table CurrentTable { get; private set; }
        public bool OrderTaken { get; private set; }

        private NavMeshAgent _agent;
        private Transform _exitPoint;
        private OrderBubble _bubble;
        private float _stateTimer;

        public void Init(Transform exitPoint)
        {
            _exitPoint = exitPoint;
            Order = possibleOrders[Random.Range(0, possibleOrders.Length)];
        }

        void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            if (animator == null) animator = GetComponentInChildren<Animator>();
        }

        void Start()
        {
            EnterShop();
        }

        void Update()
        {
            UpdateAnimator();

            switch (CurrentState)
            {
                case State.Seeking: TickSeeking(); break;
                case State.Sitting: TickSitting(); break;
                case State.WaitingForOrderTaken: TickWaitingForOrder(); break;
                case State.WaitingForDrink: TickWaitingForDrink(); break;
                case State.Drinking: TickDrinking(); break;
                case State.Leaving: TickLeaving(); break;
            }
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
            if (CurrentTable == null) { GoLeave(); return; }
            if (!_agent.pathPending && _agent.remainingDistance <= arrivalDistance)
            {
                SitDown();
            }
        }

        void SitDown()
        {
            _agent.ResetPath();
            _agent.isStopped = true;
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
                CurrentState = State.WaitingForOrderTaken;
            }
        }

        void ShowOrderBubble()
        {
            if (_bubble != null) return;
            _bubble = OrderBubble.Spawn(this);
            OrderRegistry.Add(this, Order);
        }

        void HideOrderBubble()
        {
            if (_bubble != null)
            {
                Destroy(_bubble.gameObject);
                _bubble = null;
            }
        }

        void TickWaitingForOrder() { }
        void TickWaitingForDrink() { }

        void TickDrinking()
        {
            _stateTimer -= Time.deltaTime;
            if (_stateTimer <= 0f) GoLeave();
        }

        void TickLeaving()
        {
            if (_exitPoint == null) { Finish(); return; }
            if (!_agent.pathPending && _agent.remainingDistance <= arrivalDistance) Finish();
        }

        public bool TryTakeOrder()
        {
            if (CurrentState != State.WaitingForOrderTaken) return false;
            OrderTaken = true;
            CurrentState = State.WaitingForDrink;
            if (_bubble != null) _bubble.MarkOrderTaken();
            OrderRegistry.SetState(this, OrderRegistry.TicketState.Taken);
            return true;
        }

        public bool TryDeliver(OrderType delivered)
        {
            if (CurrentState != State.WaitingForDrink) return false;
            if (delivered != Order) return false;
            HideOrderBubble();
            CurrentState = State.Drinking;
            _stateTimer = drinkingDuration;
            OrderRegistry.Remove(this);
            return true;
        }

        public bool CanTakeOrder() => CurrentState == State.WaitingForOrderTaken;
        public bool CanDeliver()   => CurrentState == State.WaitingForDrink;

        void GoLeave()
        {
            HideOrderBubble();
            if (CurrentTable != null)
            {
                CurrentTable.Release(this);
                CurrentTable = null;
            }
            CurrentState = State.Leaving;
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
        }

        void OnDestroy()
        {
            if (CurrentTable != null) CurrentTable.Release(this);
            HideOrderBubble();
            OrderRegistry.Remove(this);
        }
    }
}
