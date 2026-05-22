using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Indigo.UI;

namespace Indigo.Customers
{
    public class OrderTicketsHUD : MonoBehaviour
    {
        private RectTransform _panel;
        private RectTransform _list;
        private Text _emptyLabel;
        private readonly List<GameObject> _rows = new List<GameObject>();

        void Start()
        {
            BuildPanel();
            OrderRegistry.OnChanged += Rebuild;
            Rebuild();
        }

        void OnDestroy()
        {
            OrderRegistry.OnChanged -= Rebuild;
        }

        void BuildPanel()
        {
            var canvas = FindHudParent();
            if (canvas == null)
            {
                Debug.LogWarning("OrderTicketsHUD: HUD layer not found. Make sure GameUI is in the scene.");
                return;
            }

            var go = UIFactory.Panel("OrderTickets", canvas, UITheme.Background, 16);
            _panel = (RectTransform)go.transform;
            _panel.anchorMin = new Vector2(0f, 1f);
            _panel.anchorMax = new Vector2(0f, 1f);
            _panel.pivot     = new Vector2(0f, 1f);
            _panel.anchoredPosition = new Vector2(24, -24);
            _panel.sizeDelta = new Vector2(280, 220);

            UIFactory.AddDropShadow(_panel, new Vector2(0, -4), 0.35f, 18, 10f);

            var accent = UIFactory.Panel("Accent", _panel, UITheme.Accent, 2);
            var aRt = (RectTransform)accent.transform;
            aRt.anchorMin = new Vector2(0, 0);
            aRt.anchorMax = new Vector2(0, 1);
            aRt.pivot     = new Vector2(0, 0.5f);
            aRt.sizeDelta = new Vector2(4, 0);
            aRt.anchoredPosition = Vector2.zero;

            var caption = UIFactory.Label("Caption", _panel, "ORDERS", UITheme.FontSmall, UITheme.TextSecondary, TextAnchor.UpperLeft);
            var cRt = caption.rectTransform;
            cRt.anchorMin = new Vector2(0, 1);
            cRt.anchorMax = new Vector2(1, 1);
            cRt.pivot     = new Vector2(0, 1);
            cRt.anchoredPosition = new Vector2(16, -10);
            cRt.sizeDelta = new Vector2(-24, 18);

            var listGo = new GameObject("List", typeof(RectTransform));
            listGo.transform.SetParent(_panel, false);
            _list = (RectTransform)listGo.transform;
            _list.anchorMin = new Vector2(0, 1);
            _list.anchorMax = new Vector2(1, 1);
            _list.pivot     = new Vector2(0, 1);
            _list.anchoredPosition = new Vector2(16, -34);
            _list.sizeDelta = new Vector2(-24, 0);

            var layout = listGo.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 6;
            layout.childAlignment = TextAnchor.UpperLeft;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            var fitter = listGo.AddComponent<ContentSizeFitter>();
            fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            fitter.verticalFit   = ContentSizeFitter.FitMode.PreferredSize;

            _emptyLabel = UIFactory.Label("Empty", _panel, "No active orders", UITheme.FontSmall, UITheme.TextSecondary, TextAnchor.MiddleCenter);
            var eRt = _emptyLabel.rectTransform;
            eRt.anchorMin = new Vector2(0, 0);
            eRt.anchorMax = new Vector2(1, 0);
            eRt.pivot     = new Vector2(0.5f, 0);
            eRt.anchoredPosition = new Vector2(0, 16);
            eRt.sizeDelta = new Vector2(-24, 20);
        }

        Transform FindHudParent()
        {
            var gameUI = GameUI.Instance;
            if (gameUI == null) return null;
            var hudT = gameUI.transform.Find("GameUI_Canvas/HUD");
            return hudT;
        }

        void Rebuild()
        {
            if (_list == null) return;

            for (int i = 0; i < _rows.Count; i++) if (_rows[i] != null) Destroy(_rows[i]);
            _rows.Clear();

            var tickets = OrderRegistry.Tickets;
            if (_emptyLabel != null) _emptyLabel.gameObject.SetActive(tickets.Count == 0);

            for (int i = 0; i < tickets.Count; i++)
            {
                var t = tickets[i];
                var row = UIFactory.Panel("Ticket", _list, UITheme.PanelLight, 10);
                var rRt = (RectTransform)row.transform;
                var le = row.AddComponent<LayoutElement>();
                le.preferredHeight = 36;
                le.minHeight = 36;

                var dot = UIFactory.Panel("Dot", row.transform, t.State == OrderRegistry.TicketState.Taken ? new Color(0.40f, 0.85f, 0.55f) : UITheme.Accent, 6);
                var dRt = (RectTransform)dot.transform;
                dRt.anchorMin = new Vector2(0, 0.5f);
                dRt.anchorMax = new Vector2(0, 0.5f);
                dRt.pivot     = new Vector2(0, 0.5f);
                dRt.sizeDelta = new Vector2(10, 10);
                dRt.anchoredPosition = new Vector2(10, 0);

                var label = UIFactory.Label("Name", row.transform, t.Order.DisplayName(), UITheme.FontBody, UITheme.TextPrimary, TextAnchor.MiddleLeft);
                var lRt = label.rectTransform;
                lRt.anchorMin = Vector2.zero;
                lRt.anchorMax = Vector2.one;
                lRt.offsetMin = new Vector2(28, 0);
                lRt.offsetMax = new Vector2(-60, 0);

                var status = UIFactory.Label("Status", row.transform, t.State == OrderRegistry.TicketState.Taken ? "TAKEN" : "NEW", UITheme.FontSmall, t.State == OrderRegistry.TicketState.Taken ? new Color(0.40f, 0.85f, 0.55f) : UITheme.Accent, TextAnchor.MiddleRight);
                var sRt = status.rectTransform;
                sRt.anchorMin = new Vector2(1, 0);
                sRt.anchorMax = new Vector2(1, 1);
                sRt.pivot     = new Vector2(1, 0.5f);
                sRt.sizeDelta = new Vector2(56, 0);
                sRt.anchoredPosition = new Vector2(-10, 0);

                _rows.Add(row);
            }
        }
    }
}
