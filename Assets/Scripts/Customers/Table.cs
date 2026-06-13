using UnityEngine;

namespace Indigo.Customers
{
    public class Table : MonoBehaviour
    {
        [Tooltip("Mjesto gdje NPC stoji/sjeda. Ako je null, koristi se Table transform.")]
        public Transform seatAnchor;

        [Tooltip("Mjesto gdje se prikazuje order bubble (iznad NPC-a). Ako je null, koristi seatAnchor + offset.")]
        public Transform bubbleAnchor;

        [Tooltip("Visina na koju se podiže bubble ako bubbleAnchor nije postavljen.")]
        public float bubbleHeight = 1.8f;

        public bool IsOccupied { get; private set; }
        public Customer Occupant { get; private set; }

        public Vector3 SeatPosition => (seatAnchor != null ? seatAnchor.position : transform.position);
        public Quaternion SeatRotation => (seatAnchor != null ? seatAnchor.rotation : transform.rotation);

        public Vector3 BubblePosition
        {
            get
            {
                if (bubbleAnchor != null) return bubbleAnchor.position;
                return SeatPosition + Vector3.up * bubbleHeight;
            }
        }

        public bool TryReserve(Customer customer)
        {
            if (IsOccupied) return false;
            IsOccupied = true;
            Occupant = customer;
            return true;
        }

        public void Release(Customer customer)
        {
            if (Occupant != customer) return;
            IsOccupied = false;
            Occupant = null;
        }

       void OnDrawGizmos()
        {
            Gizmos.color = IsOccupied ? new Color(1f, 0.4f, 0.2f, 0.9f) : new Color(0.3f, 0.9f, 0.4f, 0.9f);
            Gizmos.DrawSphere(SeatPosition, 0.15f);
            
            // ZAMIJENJENO: Plave linije za bubble u editoru su sada prelijepo zlatno-žute!
            Gizmos.color = new Color(0.95f, 0.72f, 0.22f, 0.7f); 
            Gizmos.DrawWireSphere(BubblePosition, 0.12f);
            Gizmos.DrawLine(SeatPosition, BubblePosition);
        }
    }
}
