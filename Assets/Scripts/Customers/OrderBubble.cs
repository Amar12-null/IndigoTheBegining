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
        private Camera _cam;

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
            rt.sizeDelta = new Vector2(240, 90);
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
            lRt.anchoredPosition = new Vector2(8, -6);
            lRt.sizeDelta = new Vector2(-28, -22);
            bubble._label = label;

            bubble._cam = Camera.main;
            return bubble;
        }

        public void MarkOrderTaken()
        {
            if (_accent != null) _accent.color = new Color(0.40f, 0.85f, 0.55f, 1f);
            if (_label != null) _label.text = _owner.Order.DisplayName() + "  ✓";
        }

        void LateUpdate()
        {
            if (_owner == null) { Destroy(gameObject); return; }
            transform.position = _owner.CurrentTable != null
                ? _owner.CurrentTable.BubblePosition
                : _owner.transform.position + Vector3.up * 1.8f;

            if (_cam == null) _cam = Camera.main;
            if (_cam != null)
            {
                var dir = transform.position - _cam.transform.position;
                dir.y = 0f;
                if (dir.sqrMagnitude > 0.0001f)
                    transform.rotation = Quaternion.LookRotation(dir);
            }
        }
    }
}
