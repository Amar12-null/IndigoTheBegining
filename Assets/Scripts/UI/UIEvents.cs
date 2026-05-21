using System;

namespace Indigo.UI
{
    public static class UIEvents
    {
        public static event Action<string, string> OnInteractionPromptChanged;
        public static event Action<string, string> OnHeldItemChanged;
        public static event Action<bool>            OnPauseChanged;

        public static string CurrentPromptKey;
        public static string CurrentPromptText;
        public static string CurrentHeldName;
        public static string CurrentHeldState;
        public static bool   IsPaused;

        public static void SetPrompt(string key, string text)
        {
            CurrentPromptKey  = key;
            CurrentPromptText = text;
            OnInteractionPromptChanged?.Invoke(key, text);
        }

        public static void ClearPrompt()
        {
            CurrentPromptKey  = null;
            CurrentPromptText = null;
            OnInteractionPromptChanged?.Invoke(null, null);
        }

        public static void SetHeldItem(string name, string state)
        {
            CurrentHeldName  = name;
            CurrentHeldState = state;
            OnHeldItemChanged?.Invoke(name, state);
        }

        public static void ClearHeldItem()
        {
            CurrentHeldName  = null;
            CurrentHeldState = null;
            OnHeldItemChanged?.Invoke(null, null);
        }

        public static void SetPaused(bool paused)
        {
            if (IsPaused == paused) return;
            IsPaused = paused;
            OnPauseChanged?.Invoke(paused);
        }
    }
}
