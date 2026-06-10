using System;
using UnityEngine;
using Indigo.Customers;

namespace Indigo.Progression
{
    public static class PlayerProgression
    {
        public static int Level { get; private set; } = 1;
        public static int CurrentXp { get; private set; }
        public static int TotalXp { get; private set; }

        public static int XpForNextLevel => XpRequired(Level);
        public static float ProgressFraction => XpForNextLevel > 0 ? Mathf.Clamp01((float)CurrentXp / XpForNextLevel) : 0f;

        public static event Action<int> OnXpGained;
        public static event Action<int> OnLevelUp;

        public static int XpForSatisfaction(Satisfaction s)
        {
            switch (s)
            {
                case Satisfaction.Happy:   return 12;
                case Satisfaction.Neutral: return 7;
                case Satisfaction.Annoyed: return 3;
                default: return 0;
            }
        }

        public static int XpRequired(int level)
        {
            return 40 + (level - 1) * 30;
        }

        public static void AddXp(int amount)
        {
            if (amount <= 0) return;
            CurrentXp += amount;
            TotalXp += amount;
            OnXpGained?.Invoke(amount);

            while (CurrentXp >= XpForNextLevel)
            {
                CurrentXp -= XpForNextLevel;
                Level += 1;
                OnLevelUp?.Invoke(Level);
            }
        }

        public static void ResetProgress()
        {
            Level = 1;
            CurrentXp = 0;
            TotalXp = 0;
        }
    }

    public class XpBridge : MonoBehaviour
    {
        [Tooltip("Resetuje XP na 0 i level na 1 kad scena startuje.")]
        public bool resetOnStart = true;

        void OnEnable()
        {
            if (resetOnStart) PlayerProgression.ResetProgress();
            MoneyManager.OnTipReceived += HandleTip;
        }

        void OnDisable()
        {
            MoneyManager.OnTipReceived -= HandleTip;
        }

        void HandleTip(int _, Vector3 __, Satisfaction sat)
        {
            int xp = PlayerProgression.XpForSatisfaction(sat);
            PlayerProgression.AddXp(xp);
        }
    }
}
