using System;
using System.Collections.Generic;

namespace Indigo.Customers
{
    public static class OrderRegistry
    {
        public enum TicketState { Pending, Taken, Delivered }

        public class Ticket
        {
            public Customer Customer;
            public OrderType Order;
            public TicketState State;
        }

        private static readonly List<Ticket> _tickets = new List<Ticket>();
        public static IReadOnlyList<Ticket> Tickets => _tickets;

        public static event Action OnChanged;

        public static Ticket Add(Customer c, OrderType o)
        {
            var t = new Ticket { Customer = c, Order = o, State = TicketState.Pending };
            _tickets.Add(t);
            OnChanged?.Invoke();
            return t;
        }

        public static void SetState(Customer c, TicketState s)
        {
            for (int i = 0; i < _tickets.Count; i++)
            {
                if (_tickets[i].Customer == c)
                {
                    _tickets[i].State = s;
                    OnChanged?.Invoke();
                    return;
                }
            }
        }

        public static void Remove(Customer c)
        {
            for (int i = _tickets.Count - 1; i >= 0; i--)
            {
                if (_tickets[i].Customer == c)
                {
                    _tickets.RemoveAt(i);
                    OnChanged?.Invoke();
                    return;
                }
            }
        }

        public static void Clear()
        {
            if (_tickets.Count == 0) return;
            _tickets.Clear();
            OnChanged?.Invoke();
        }
    }
}
