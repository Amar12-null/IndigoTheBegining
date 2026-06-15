using UnityEngine;

namespace Indigo.UI
{
    public static class UITheme
    {
        // ── Surfaces ──────────────────────────────────────────────
        // Deep indigo-charcoal with a faint purple tint — the "Indigo" identity.
        public static readonly Color Background      = new Color(0.072f, 0.076f, 0.110f, 0.94f);
        public static readonly Color Panel           = new Color(0.125f, 0.130f, 0.175f, 0.97f);
        public static readonly Color PanelLight      = new Color(0.190f, 0.200f, 0.260f, 1f);
        public static readonly Color PanelDark       = new Color(0.090f, 0.095f, 0.130f, 1f);

        // Gradient stops used for full-screen backdrops (top brighter, bottom deeper).
        public static readonly Color BackdropTop     = new Color(0.115f, 0.120f, 0.175f, 1f);
        public static readonly Color BackdropBottom  = new Color(0.045f, 0.048f, 0.075f, 1f);

        // ── Accent (cool aqua-teal) ───────────────────────────────
        public static readonly Color Accent          = new Color(0.34f, 0.78f, 0.84f, 1f);
        public static readonly Color AccentHover     = new Color(0.50f, 0.89f, 0.93f, 1f);
        public static readonly Color AccentPressed   = new Color(0.22f, 0.60f, 0.66f, 1f);
        public static readonly Color AccentSoft      = new Color(0.34f, 0.78f, 0.84f, 0.18f);

        // Warm gold — used for money / rewards so they read as valuable.
        public static readonly Color Gold            = new Color(0.98f, 0.78f, 0.40f, 1f);
        public static readonly Color GoldHover       = new Color(1.00f, 0.86f, 0.52f, 1f);

        // ── Semantic state colors ─────────────────────────────────
        public static readonly Color Success         = new Color(0.46f, 0.82f, 0.52f, 1f);
        public static readonly Color Warning         = new Color(0.98f, 0.74f, 0.34f, 1f);
        public static readonly Color Danger          = new Color(0.95f, 0.43f, 0.40f, 1f);

        // ── Text ──────────────────────────────────────────────────
        public static readonly Color TextPrimary     = new Color(0.95f, 0.96f, 0.97f, 1f);
        public static readonly Color TextSecondary   = new Color(0.66f, 0.69f, 0.76f, 1f);
        public static readonly Color TextMuted        = new Color(0.50f, 0.53f, 0.60f, 1f);
        public static readonly Color TextShadow      = new Color(0f, 0f, 0f, 0.45f);

        // ── Misc ──────────────────────────────────────────────────
        public static readonly Color KeyCapBg        = new Color(0.96f, 0.97f, 0.98f, 1f);
        public static readonly Color KeyCapText      = new Color(0.10f, 0.11f, 0.14f, 1f);
        public static readonly Color Divider         = new Color(1f, 1f, 1f, 0.07f);
        public static readonly Color Stroke          = new Color(1f, 1f, 1f, 0.10f);

        // ── Typography ────────────────────────────────────────────
        public const int FontTitle  = 68;
        public const int FontHeader = 32;
        public const int FontBody   = 20;
        public const int FontSmall  = 16;
        public const int FontTiny   = 13;

        public static Font UIFont
        {
            get
            {
                if (_font == null)
                    _font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                return _font;
            }
        }
        private static Font _font;
    }
}
