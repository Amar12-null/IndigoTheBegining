using System;
using UnityEngine;
using Indigo.Customers;

namespace Indigo.Game
{
    public class DayManager : MonoBehaviour
    {
        public enum DayPhase { PreDay, Active, Ended }

        public static DayManager Instance { get; private set; }

        [Header("Day timing")]
        [Tooltip("Trajanje jednog radnog dana u sekundama.")]
        public float dayDuration = 180f;

        [Tooltip("Sekundi prije nego što dan 1 automatski počne (npr. da igrač dobije par sekundi).")]
        public float startDelay = 2f;

        [Tooltip("Automatski počinje sljedeći dan kad igrač zatvori summary.")]
        public bool autoStartFirstDay = true;

        [Header("Difficulty scaling")]
        [Tooltip("Bazna stopa spawnanja se dijeli sa ovim svakim danom (faster spawning).")]
        public float spawnRateGrowth = 1.15f;

        [Tooltip("Bazna patience za narudžbe se množi sa ovim svakim danom (manje strpljenja).")]
        public float patienceShrink = 0.92f;

        [Header("Spawner reference (optional)")]
        public CustomerSpawner spawner;

        public int CurrentDay { get; private set; } = 0;
        public DayPhase Phase { get; private set; } = DayPhase.PreDay;
        public float TimeRemaining { get; private set; }
        public float DayFraction => dayDuration > 0 ? Mathf.Clamp01(1f - TimeRemaining / dayDuration) : 0f;

        public int DayServed { get; private set; }
        public int DayLost { get; private set; }
        public int DayEarnedAtStart { get; private set; }

        public event Action<int> OnDayStarted;
        public event Action<DaySummary> OnDayEnded;
        public event Action<DayPhase> OnPhaseChanged;

        private float _startCountdown;

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            if (spawner == null) spawner = FindAnyObjectByType<CustomerSpawner>();
        }

        void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        void Start()
        {
            if (autoStartFirstDay) _startCountdown = startDelay;
        }

        public float PreDayCountdown => _startCountdown;

        void Update()
        {
            if (Indigo.UI.UIEvents.IsPaused) return;

            if (Phase == DayPhase.PreDay && _startCountdown > 0f)
            {
                _startCountdown -= Time.deltaTime;
                if (_startCountdown <= 0f) StartNextDay();
                return;
            }

            if (Phase != DayPhase.Active) return;

            TimeRemaining -= Time.deltaTime;
            if (TimeRemaining <= 0f) EndDay();
        }

        public void StartNextDay()
        {
            CurrentDay += 1;
            TimeRemaining = dayDuration;
            DayServed = MoneyManager.CustomersServed;
            DayLost = MoneyManager.CustomersLost;
            DayEarnedAtStart = MoneyManager.TotalEarned;

            ApplyDifficulty(CurrentDay);

            SetPhase(DayPhase.Active);
            OnDayStarted?.Invoke(CurrentDay);
        }

        void ApplyDifficulty(int day)
        {
            if (spawner == null) return;
            float k = Mathf.Pow(spawnRateGrowth, day - 1);
            spawner.minSpawnInterval = Mathf.Max(1.5f, spawner.minSpawnInterval / k);
            spawner.maxSpawnInterval = Mathf.Max(3f,   spawner.maxSpawnInterval / k);
        }

        void EndDay()
        {
            TimeRemaining = 0f;
            SetPhase(DayPhase.Ended);

            int dayServed = MoneyManager.CustomersServed - DayServed;
            int dayLost   = MoneyManager.CustomersLost - DayLost;
            int dayEarned = MoneyManager.TotalEarned - DayEarnedAtStart;

            var summary = new DaySummary
            {
                Day = CurrentDay,
                CustomersServed = dayServed,
                CustomersLost = dayLost,
                Earned = dayEarned,
                Rating = ComputeRating(dayServed, dayLost),
            };
            OnDayEnded?.Invoke(summary);
        }

        static string ComputeRating(int served, int lost)
        {
            if (served + lost == 0) return "—";
            float ratio = (float)served / (served + lost);
            if (ratio >= 0.95f) return "S";
            if (ratio >= 0.80f) return "A";
            if (ratio >= 0.60f) return "B";
            if (ratio >= 0.40f) return "C";
            return "D";
        }

        void SetPhase(DayPhase p)
        {
            if (Phase == p) return;
            Phase = p;
            OnPhaseChanged?.Invoke(p);
        }

        public bool IsSpawningAllowed() => Phase == DayPhase.Active;
    }

    public struct DaySummary
    {
        public int Day;
        public int CustomersServed;
        public int CustomersLost;
        public int Earned;
        public string Rating;
    }
}
