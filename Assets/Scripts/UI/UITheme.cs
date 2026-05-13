using UnityEngine;

namespace Indigo.UI
{
    public static class UITheme
    {
        public static readonly Color Background      = new Color(0.07f, 0.08f, 0.10f, 0.92f);
        public static readonly Color Panel           = new Color(0.12f, 0.13f, 0.16f, 0.95f);
        public static readonly Color PanelLight      = new Color(0.18f, 0.19f, 0.22f, 1f);
        public static readonly Color Accent          = new Color(0.36f, 0.78f, 0.95f, 1f);
        public static readonly Color AccentHover     = new Color(0.50f, 0.86f, 1.00f, 1f);
        public static readonly Color AccentPressed   = new Color(0.22f, 0.62f, 0.82f, 1f);
        public static readonly Color TextPrimary     = new Color(0.96f, 0.97f, 0.98f, 1f);
        public static readonly Color TextSecondary   = new Color(0.72f, 0.74f, 0.78f, 1f);
        public static readonly Color KeyCapBg        = new Color(0.96f, 0.97f, 0.98f, 1f);
        public static readonly Color KeyCapText      = new Color(0.10f, 0.11f, 0.14f, 1f);

        public const int FontTitle  = 64;
        public const int FontHeader = 32;
        public const int FontBody   = 20;
        public const int FontSmall  = 16;

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
