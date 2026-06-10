using UnityEngine;
using UnityEngine.UI;
using Indigo.UI;

namespace Indigo.Customers
{
    public class MoneyHUD : MonoBehaviour
    {
        [Tooltip("Prefiks ispred iznosa (npr. '$', '€', 'KM ').")]
        public string currencyPrefix = "$";

        [Tooltip("Da li resetovati balance na 0 kad scena startuje.")]
        public bool resetOnStart = true;

        private RectTransform _panel;
        private Text _amountLabel;
        private Text _statsLabel;

        void Start()
        {
            if (resetOnStart) MoneyManager.ResetSession();
            BuildPanel();
            MoneyManager.OnBalanceChanged += HandleBalance;
            MoneyManager.OnTipReceived += HandleTip;
            Refresh();
        }

        void OnDestroy()
        {
            MoneyManager.OnBalanceChanged -= HandleBalance;
            MoneyManager.OnTipReceived -= HandleTip;
        }

        void BuildPanel()
        {
            var canvas = FindHudParent();
            if (canvas == null)
            {
                Debug.LogWarning("MoneyHUD: HUD layer not found. Make sure GameUI is in the scene.");
                return;
            }

            var go = UIFactory.Panel("MoneyPanel", canvas, UITheme.Background, 16);
            _panel = (RectTransform)go.transform;
            _panel.anchorMin = new Vector2(1f, 1f);
            _panel.anchorMax = new Vector2(1f, 1f);
            _panel.pivot     = new Vector2(1f, 1f);
            _panel.anchoredPosition = new Vector2(-24, -24);
            _panel.sizeDelta = new Vector2(240, 92);

            UIFactory.AddDropShadow(_panel, new Vector2(0, -4), 0.35f, 18, 10f);

            var accent = UIFactory.Panel("Accent", _panel, UITheme.Accent, 2);
            var aRt = (RectTransform)accent.transform;
            aRt.anchorMin = new Vector2(0, 0);
            aRt.anchorMax = new Vector2(0, 1);
            aRt.pivot     = new Vector2(0, 0.5f);
            aRt.sizeDelta = new Vector2(4, 0);
            aRt.anchoredPosition = Vector2.zero;

            var caption = UIFactory.Label("Caption", _panel, "TIPS", UITheme.FontSmall, UITheme.TextSecondary, TextAnchor.UpperLeft);
            var cRt = caption.rectTransform;
            cRt.anchorMin = new Vector2(0, 1);
            cRt.anchorMax = new Vector2(1, 1);
            cRt.pivot     = new Vector2(0, 1);
            cRt.anchoredPosition = new Vector2(16, -10);
            cRt.sizeDelta = new Vector2(-24, 18);

            _amountLabel = UIFactory.Label("Amount", _panel, currencyPrefix + "0", UITheme.FontHeader, UITheme.TextPrimary, TextAnchor.MiddleLeft);
            _amountLabel.fontStyle = FontStyle.Bold;
            var lRt = _amountLabel.rectTransform;
            lRt.anchorMin = new Vector2(0, 0);
            lRt.anchorMax = new Vector2(1, 1);
            lRt.pivot     = new Vector2(0, 0.5f);
            lRt.anchoredPosition = new Vector2(16, -2);
            lRt.sizeDelta = new Vector2(-28, -28);

            _statsLabel = UIFactory.Label("Stats", _panel, "Served 0 · Lost 0", UITheme.FontSmall, UITheme.TextSecondary, TextAnchor.LowerLeft);
            var sRt = _statsLabel.rectTransform;
            sRt.anchorMin = new Vector2(0, 0);
            sRt.anchorMax = new Vector2(1, 0);
            sRt.pivot     = new Vector2(0, 0);
            sRt.anchoredPosition = new Vector2(16, 8);
            sRt.sizeDelta = new Vector2(-24, 18);
        }

        Transform FindHudParent()
        {
            var gameUI = GameUI.Instance;
            if (gameUI == null) return null;
            return gameUI.transform.Find("GameUI_Canvas/HUD");
        }

        void HandleBalance(int _) { Refresh(); }

        void HandleTip(int amount, Vector3 _, Satisfaction __)
        {
            if (_panel == null) return;
            StartCoroutine(PunchScale());
        }

        System.Collections.IEnumerator PunchScale()
        {
            float t = 0f;
            const float dur = 0.22f;
            while (t < dur)
            {
                t += Time.deltaTime;
                float k = t / dur;
                float s = 1f + Mathf.Sin(k * Mathf.PI) * 0.12f;
                _panel.localScale = new Vector3(s, s, 1f);
                yield return null;
            }
            _panel.localScale = Vector3.one;
        }

        void Refresh()
        {
            if (_amountLabel != null) _amountLabel.text = currencyPrefix + MoneyManager.Balance;
            if (_statsLabel != null)  _statsLabel.text  = $"Served {MoneyManager.CustomersServed} · Lost {MoneyManager.CustomersLost}";
        }
    }
}
