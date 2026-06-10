using System;
using UnityEngine;

namespace Indigo.Customers
{
    public static class MoneyManager
    {
        public static int Balance { get; private set; }
        public static int TotalEarned { get; private set; }
        public static int CustomersServed { get; private set; }
        public static int CustomersLost { get; private set; }

        public static event Action<int> OnBalanceChanged;
        public static event Action<int, Vector3, Satisfaction> OnTipReceived;
        public static event Action<Vector3> OnAngryLeave;

        public static void AddTip(int amount, Vector3 worldPos, Satisfaction sat)
        {
            if (amount <= 0) return;
            Balance += amount;
            TotalEarned += amount;
            CustomersServed += 1;
            OnTipReceived?.Invoke(amount, worldPos, sat);
            OnBalanceChanged?.Invoke(Balance);
        }

        public static void NotifyAngryLeave(Vector3 worldPos)
        {
            CustomersLost += 1;
            OnAngryLeave?.Invoke(worldPos);
        }

        public static bool TrySpend(int amount)
        {
            if (amount <= 0) return true;
            if (Balance < amount) return false;
            Balance -= amount;
            OnBalanceChanged?.Invoke(Balance);
            return true;
        }

        public static void ResetSession()
        {
            Balance = 0;
            TotalEarned = 0;
            CustomersServed = 0;
            CustomersLost = 0;
            OnBalanceChanged?.Invoke(Balance);
        }
    }
}
