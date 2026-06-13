using System;
using UnityEngine;
using Indigo.UI;

namespace Indigo.Game
{
    public class GameStateManager : MonoBehaviour
    {
        public enum GameState { Boot, DayActive, Paused, DayEnded, Shopping }

        public static GameStateManager Instance { get; private set; }

        public GameState Current { get; private set; } = GameState.Boot;

        public event Action<GameState, GameState> OnStateChanged;

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        void Start()
        {
            if (DayManager.Instance != null)
            {
                DayManager.Instance.OnPhaseChanged += HandleDayPhase;
                DayManager.Instance.OnDayEnded += HandleDayEnded;
            }
            UIEvents.OnPauseChanged += HandlePause;
        }

        void OnDestroy()
        {
            if (Instance == this) Instance = null;
            if (DayManager.Instance != null)
            {
                DayManager.Instance.OnPhaseChanged -= HandleDayPhase;
                DayManager.Instance.OnDayEnded -= HandleDayEnded;
            }
            UIEvents.OnPauseChanged -= HandlePause;
        }

        void HandleDayEnded(DaySummary _) { Set(GameState.DayEnded); }

        void HandleDayPhase(DayManager.DayPhase phase)
        {
            if (phase == DayManager.DayPhase.Active) Set(GameState.DayActive);
        }

        void HandlePause(bool paused)
        {
            if (Current == GameState.DayEnded || Current == GameState.Shopping) return;
            if (paused) Set(GameState.Paused);
            else if (DayManager.Instance != null && DayManager.Instance.Phase == DayManager.DayPhase.Active)
                Set(GameState.DayActive);
        }

        public void NotifyShoppingOpened() { if (Current != GameState.DayEnded) Set(GameState.Shopping); }
        public void NotifyShoppingClosed()
        {
            if (Current == GameState.Shopping)
            {
                if (DayManager.Instance != null && DayManager.Instance.Phase == DayManager.DayPhase.Active)
                    Set(GameState.DayActive);
                else
                    Set(GameState.Boot);
            }
        }

        void Set(GameState s)
        {
            if (Current == s) return;
            var prev = Current;
            Current = s;
            OnStateChanged?.Invoke(prev, s);
        }
    }
}
