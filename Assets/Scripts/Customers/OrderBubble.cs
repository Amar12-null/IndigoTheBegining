using UnityEngine;
using UnityEngine.UI;
using Indigo.UI;

namespace Indigo.Customers
{
    public class OrderBubble : MonoBehaviour
    {
        private Customer _owner;
        private Text _label;
        private Image _panel;
        private Image _accent;
        private Image _patienceFill;
        private RectTransform _patienceFillRt;
        private float _patienceFillFullWidth;
        private Camera _cam;
        private bool _orderTaken;

        public static OrderBubble Spawn(Customer owner)
        {
            var go = new GameObject("OrderBubble_" + owner.name,
                typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            var canvas = go.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.sortingOrder = 50;

            var scaler = go.GetComponent<CanvasScaler>();
            scaler.dynamicPixelsPerUnit = 12f;
            scaler.referencePixelsPerUnit = 100f;

            var rt = (RectTransform)go.transform;
            rt.sizeDelta = new Vector2(260, 110);
            rt.localScale = Vector3.one * 0.005f;

            var bubble = go.AddComponent<OrderBubble>();
            bubble._owner = owner;

            var panel = UIFactory.Panel("Panel", go.transform, UITheme.Background, 14);
            UIFactory.StretchToParent((RectTransform)panel.transform);
            bubble._panel = panel.GetComponent<Image>();

            var accent = UIFactory.Panel("Accent", panel.transform, UITheme.Accent, 2);
            var aRt = (RectTransform)accent.transform;
            aRt.anchorMin = new Vector2(0, 0);
            aRt.anchorMax = new Vector2(0, 1);
            aRt.pivot = new Vector2(0, 0.5f);
            aRt.sizeDelta = new Vector2(4, 0);
            aRt.anchoredPosition = Vector2.zero;
            bubble._accent = accent.GetComponent<Image>();

            var caption = UIFactory.Label("Caption", panel.transform, "WANTS", UITheme.FontSmall, UITheme.TextSecondary, TextAnchor.UpperLeft);
            var cRt = caption.rectTransform;
            cRt.anchorMin = new Vector2(0, 1);
            cRt.anchorMax = new Vector2(1, 1);
            cRt.pivot = new Vector2(0, 1);
            cRt.anchoredPosition = new Vector2(18, -10);
            cRt.sizeDelta = new Vector2(-28, 18);

            var label = UIFactory.Label("Order", panel.transform, owner.Order.DisplayName(), UITheme.FontHeader, UITheme.TextPrimary, TextAnchor.MiddleLeft);
            label.fontStyle = FontStyle.Bold;
            var lRt = label.rectTransform;
            lRt.anchorMin = new Vector2(0, 0);
            lRt.anchorMax = new Vector2(1, 1);
            lRt.pivot = new Vector2(0.5f, 0.5f);
            lRt.anchoredPosition = new Vector2(8, 4);
            lRt.sizeDelta = new Vector2(-28, -40);
            bubble._label = label;

            // Patience bar background (track)
            float trackWidth = 220f;
            var track = UIFactory.Panel("PatienceTrack", panel.transform, UITheme.PanelLight, 4);
            var tRt = (RectTransform)track.transform;
            tRt.anchorMin = new Vector2(0, 0);
            tRt.anchorMax = new Vector2(0, 0);
            tRt.pivot = new Vector2(0, 0);
            tRt.anchoredPosition = new Vector2(18, 10);
            tRt.sizeDelta = new Vector2(trackWidth, 8);

            // Patience bar fill
            var fill = UIFactory.Panel("PatienceFill", track.transform, UITheme.Accent, 4);
            var fRt = (RectTransform)fill.transform;
            fRt.anchorMin = new Vector2(0, 0);
            fRt.anchorMax = new Vector2(0, 1);
            fRt.pivot = new Vector2(0, 0.5f);
            fRt.anchoredPosition = Vector2.zero;
            fRt.sizeDelta = new Vector2(trackWidth, 0);
            bubble._patienceFill = fill.GetComponent<Image>();
            bubble._patienceFillRt = fRt;
            bubble._patienceFillFullWidth = trackWidth;

            bubble._cam = Camera.main;
            return bubble;
        }

        public void MarkOrderTaken()
        {
            _orderTaken = true;
            if (_label != null) _label.text = _owner.Order.DisplayName() + "  ✓";
        }

        void LateUpdate()
        {
            if (_owner == null) { Destroy(gameObject); return; }

            // Position above the seated NPC
            transform.position = _owner.CurrentTable != null
                ? _owner.CurrentTable.BubblePosition
                : _owner.transform.position + Vector3.up * 1.8f;

            // Billboard toward main camera (yaw only — keeps bubble upright)
            if (_cam == null) _cam = Camera.main;
            if (_cam != null)
            {
                var dir = transform.position - _cam.transform.position;
                dir.y = 0f;
                if (dir.sqrMagnitude > 0.0001f)
                    transform.rotation = Quaternion.LookRotation(dir);
            }

            // Patience bar + mood color
            var sat = _owner.CurrentSatisfaction;
            var moodColor = sat.Color();

            if (_patienceFillRt != null)
            {
                float frac = _owner.PatienceFraction;
                _patienceFillRt.sizeDelta = new Vector2(_patienceFillFullWidth * frac, 0f);
                if (_patienceFill != null) _patienceFill.color = moodColor;
            }

            if (_accent != null)
                _accent.color = _orderTaken ? new Color(0.40f, 0.85f, 0.55f, 1f) : moodColor;
        }
    }
}
