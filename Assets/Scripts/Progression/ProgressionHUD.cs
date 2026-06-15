using UnityEngine;
using UnityEngine.UI;
using Indigo.UI;

namespace Indigo.Progression
{
    public class ProgressionHUD : MonoBehaviour
    {
        private RectTransform _panel;
        private Text _levelLabel;
        private Text _xpLabel;
        private Image _xpFill;
        private RectTransform _xpFillRt;
        private float _trackWidth;

        void Start()
        {
            BuildPanel();
            PlayerProgression.OnXpGained += HandleXpGained;
            PlayerProgression.OnLevelUp += HandleLevelUp;
            Refresh();
        }

        void OnDestroy()
        {
            PlayerProgression.OnXpGained -= HandleXpGained;
            PlayerProgression.OnLevelUp -= HandleLevelUp;
        }

        void HandleXpGained(int _) { Refresh(); }
        void HandleLevelUp(int _) { Refresh(); }

        void BuildPanel()
        {
            var canvas = FindHudParent();
            if (canvas == null)
            {
                Debug.LogWarning("ProgressionHUD: HUD layer not found.");
                return;
            }

            var go = UIFactory.Panel("XpPanel", canvas, UITheme.Background, 16);
            _panel = (RectTransform)go.transform;
            _panel.anchorMin = new Vector2(1f, 1f);
            _panel.anchorMax = new Vector2(1f, 1f);
            _panel.pivot     = new Vector2(1f, 1f);
            _panel.anchoredPosition = new Vector2(-24, -132);
            _panel.sizeDelta = new Vector2(240, 76);

            UIFactory.AddDropShadow(_panel, new Vector2(0, -4), 0.35f, 18, 10f);

            var accent = UIFactory.Panel("Accent", _panel, UITheme.Accent, 2);
            var aRt = (RectTransform)accent.transform;
            aRt.anchorMin = new Vector2(0, 0);
            aRt.anchorMax = new Vector2(0, 1);
            aRt.pivot     = new Vector2(0, 0.5f);
            aRt.sizeDelta = new Vector2(4, 0);
            aRt.anchoredPosition = Vector2.zero;

            var caption = UIFactory.Label("Caption", _panel, "LEVEL", UITheme.FontSmall, UITheme.TextSecondary, TextAnchor.UpperLeft);
            var cRt = caption.rectTransform;
            cRt.anchorMin = new Vector2(0, 1);
            cRt.anchorMax = new Vector2(1, 1);
            cRt.pivot     = new Vector2(0, 1);
            cRt.anchoredPosition = new Vector2(16, -10);
            cRt.sizeDelta = new Vector2(-24, 18);

            _levelLabel = UIFactory.Label("Level", _panel, "1", UITheme.FontHeader, UITheme.TextPrimary, TextAnchor.MiddleLeft);
            _levelLabel.fontStyle = FontStyle.Bold;
            var lRt = _levelLabel.rectTransform;
            lRt.anchorMin = new Vector2(0, 0);
            lRt.anchorMax = new Vector2(1, 1);
            lRt.pivot     = new Vector2(0, 0.5f);
            lRt.anchoredPosition = new Vector2(16, 4);
            lRt.sizeDelta = new Vector2(-28, -32);

            _trackWidth = 200f;
            var track = UIFactory.Panel("XpTrack", _panel, UITheme.PanelLight, 4);
            var trRt = (RectTransform)track.transform;
            trRt.anchorMin = new Vector2(0, 0);
            trRt.anchorMax = new Vector2(0, 0);
            trRt.pivot     = new Vector2(0, 0);
            trRt.anchoredPosition = new Vector2(16, 12);
            trRt.sizeDelta = new Vector2(_trackWidth, 6);

            var fill = UIFactory.Panel("XpFill", track.transform, UITheme.Accent, 4);
            _xpFillRt = (RectTransform)fill.transform;
            _xpFillRt.anchorMin = new Vector2(0, 0);
            _xpFillRt.anchorMax = new Vector2(0, 1);
            _xpFillRt.pivot     = new Vector2(0, 0.5f);
            _xpFillRt.anchoredPosition = Vector2.zero;
            _xpFillRt.sizeDelta = new Vector2(_trackWidth, 0);
            _xpFill = fill.GetComponent<Image>();
            UIGradient.Apply(_xpFill, UITheme.AccentHover, UITheme.AccentPressed);

            _xpLabel = UIFactory.Label("XpText", _panel, "0 / 40 XP", UITheme.FontSmall, UITheme.TextSecondary, TextAnchor.LowerRight);
            var xRt = _xpLabel.rectTransform;
            xRt.anchorMin = new Vector2(1, 0);
            xRt.anchorMax = new Vector2(1, 0);
            xRt.pivot     = new Vector2(1, 0);
            xRt.anchoredPosition = new Vector2(-16, 22);
            xRt.sizeDelta = new Vector2(120, 18);
        }

        Transform FindHudParent()
        {
            var gameUI = GameUI.Instance;
            if (gameUI == null) return null;
            return gameUI.transform.Find("GameUI_Canvas/HUD");
        }

        void Refresh()
        {
            if (_levelLabel != null) _levelLabel.text = "Lv " + PlayerProgression.Level;
            if (_xpLabel != null)    _xpLabel.text    = $"{PlayerProgression.CurrentXp} / {PlayerProgression.XpForNextLevel} XP";
            if (_xpFillRt != null)   _xpFillRt.sizeDelta = new Vector2(_trackWidth * PlayerProgression.ProgressFraction, 0f);
        }
    }
}
