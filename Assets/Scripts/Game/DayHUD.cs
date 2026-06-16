using UnityEngine;
using UnityEngine.UI;
using Indigo.UI;

namespace Indigo.Game
{
    public class DayHUD : MonoBehaviour
    {
        private RectTransform _panel;
        private Text _dayLabel;
        private Text _timeLabel;
        private Image _timeFill;
        private RectTransform _timeFillRt;
        private float _trackWidth;

        void Start()
        {
            BuildPanel();
        }

        void BuildPanel()
        {
            var canvas = FindHudParent();
            if (canvas == null)
            {
                Debug.LogWarning("DayHUD: HUD layer not found.");
                return;
            }

            var go = UIFactory.Panel("DayPanel", canvas, UITheme.Background, 16);
            _panel = (RectTransform)go.transform;
            _panel.anchorMin = new Vector2(0.5f, 1f);
            _panel.anchorMax = new Vector2(0.5f, 1f);
            _panel.pivot     = new Vector2(0.5f, 1f);
            _panel.anchoredPosition = new Vector2(0, -24);
            _panel.sizeDelta = new Vector2(360, 76);

            UIFactory.AddDropShadow(_panel, new Vector2(0, -4), 0.35f, 18, 10f);

            _dayLabel = UIFactory.Label("Day", _panel, "Day 1", UITheme.FontHeader, UITheme.TextPrimary, TextAnchor.UpperCenter);
            _dayLabel.fontStyle = FontStyle.Bold;
            var dRt = _dayLabel.rectTransform;
            dRt.anchorMin = new Vector2(0, 1);
            dRt.anchorMax = new Vector2(1, 1);
            dRt.pivot     = new Vector2(0.5f, 1);
            dRt.anchoredPosition = new Vector2(0, -6);
            dRt.sizeDelta = new Vector2(-24, 36);

            _timeLabel = UIFactory.Label("Time", _panel, "3:00", UITheme.FontSmall, UITheme.TextSecondary, TextAnchor.UpperCenter);
            var tRt = _timeLabel.rectTransform;
            tRt.anchorMin = new Vector2(0, 1);
            tRt.anchorMax = new Vector2(1, 1);
            tRt.pivot     = new Vector2(0.5f, 1);
            tRt.anchoredPosition = new Vector2(0, -38);
            tRt.sizeDelta = new Vector2(-24, 18);

            _trackWidth = 320f;
            var track = UIFactory.Panel("TimeTrack", _panel, UITheme.PanelLight, 4);
            var trRt = (RectTransform)track.transform;
            trRt.anchorMin = new Vector2(0.5f, 0);
            trRt.anchorMax = new Vector2(0.5f, 0);
            trRt.pivot     = new Vector2(0.5f, 0);
            trRt.anchoredPosition = new Vector2(0, 10);
            trRt.sizeDelta = new Vector2(_trackWidth, 6);

            var fill = UIFactory.Panel("TimeFill", track.transform, UITheme.Accent, 4);
            _timeFillRt = (RectTransform)fill.transform;
            _timeFillRt.anchorMin = new Vector2(0, 0);
            _timeFillRt.anchorMax = new Vector2(0, 1);
            _timeFillRt.pivot     = new Vector2(0, 0.5f);
            _timeFillRt.anchoredPosition = Vector2.zero;
            _timeFillRt.sizeDelta = new Vector2(_trackWidth, 0);
            _timeFill = fill.GetComponent<Image>();
            UIGradient.Apply(_timeFill, Color.white, new Color(0.78f, 0.78f, 0.82f, 1f));
        }

        Transform FindHudParent()
        {
            var gameUI = GameUI.Instance;
            if (gameUI == null) return null;
            return gameUI.transform.Find("GameUI_Canvas/HUD");
        }

        void Update()
        {
            var dm = DayManager.Instance;
            if (dm == null || _panel == null) return;

            int displayDay = Mathf.Max(1, dm.CurrentDay == 0 ? 1 : dm.CurrentDay);
            if (_dayLabel != null) _dayLabel.text = "Day " + displayDay;

            if (dm.Phase == DayManager.DayPhase.PreDay)
            {
                int seconds = Mathf.CeilToInt(Mathf.Max(0f, dm.PreDayCountdown));
                if (_timeLabel != null) _timeLabel.text = "Starts in " + seconds + "s";
                if (_timeFillRt != null) _timeFillRt.sizeDelta = new Vector2(_trackWidth, 0f);
                if (_timeFill != null) _timeFill.color = UITheme.Accent;
                return;
            }

            float t = Mathf.Max(0f, dm.TimeRemaining);
            int mins = Mathf.FloorToInt(t / 60f);
            int secs = Mathf.FloorToInt(t % 60f);
            if (_timeLabel != null) _timeLabel.text = $"{mins}:{secs:00}";

            float frac = 1f - dm.DayFraction;
            if (_timeFillRt != null) _timeFillRt.sizeDelta = new Vector2(_trackWidth * frac, 0f);
            if (_timeFill != null)
            {
                _timeFill.color = frac < 0.2f
                    ? UITheme.Danger
                    : (frac < 0.45f ? UITheme.Warning : UITheme.Accent);
            }
        }
    }
}
