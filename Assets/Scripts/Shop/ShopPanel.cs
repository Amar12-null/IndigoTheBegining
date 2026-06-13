using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Indigo.UI;
using Indigo.Customers;
using Indigo.Progression;

namespace Indigo.Shop
{
    public class ShopPanel : MonoBehaviour
    {
        [Tooltip("Tipka za otvaranje/zatvaranje shopa.")]
        public KeyCode toggleKey = KeyCode.B;

        private RectTransform _root;
        private RectTransform _list;
        private bool _isOpen;
        private readonly List<GameObject> _rows = new List<GameObject>();

        void Start()
        {
            BuildPanel();
            MoneyManager.OnBalanceChanged += HandleBalance;
            PlayerProgression.OnLevelUp += HandleLevel;
            Upgrades.OnPurchased += HandlePurchased;
        }

        void OnDestroy()
        {
            MoneyManager.OnBalanceChanged -= HandleBalance;
            PlayerProgression.OnLevelUp -= HandleLevel;
            Upgrades.OnPurchased -= HandlePurchased;
        }

        void HandleBalance(int _) { RefreshRows(); }
        void HandleLevel(int _) { RefreshRows(); }
        void HandlePurchased(UpgradeId _) { RefreshRows(); }

        void Update()
        {
            if (Input.GetKeyDown(toggleKey)) Toggle();
        }

        void BuildPanel()
        {
            var canvas = FindHudParent();
            if (canvas == null)
            {
                Debug.LogWarning("ShopPanel: HUD layer not found.");
                return;
            }

            var dim = UIFactory.Panel("ShopDim", canvas, new Color(0f, 0f, 0f, 0.55f));
            UIFactory.StretchToParent((RectTransform)dim.transform);
            _root = (RectTransform)dim.transform;

            var panel = UIFactory.Panel("ShopPanel", _root, UITheme.Panel, 18);
            var pRt = (RectTransform)panel.transform;
            pRt.anchorMin = new Vector2(0.5f, 0.5f);
            pRt.anchorMax = new Vector2(0.5f, 0.5f);
            pRt.pivot     = new Vector2(0.5f, 0.5f);
            pRt.sizeDelta = new Vector2(620, 520);
            UIFactory.AddDropShadow(pRt, new Vector2(0, -6), 0.45f, 20, 14f);

            var caption = UIFactory.Label("Caption", panel.transform, "SHOP", UITheme.FontSmall, UITheme.TextSecondary, TextAnchor.UpperCenter);
            var cRt = caption.rectTransform;
            cRt.anchorMin = new Vector2(0, 1);
            cRt.anchorMax = new Vector2(1, 1);
            cRt.pivot     = new Vector2(0.5f, 1);
            cRt.anchoredPosition = new Vector2(0, -16);
            cRt.sizeDelta = new Vector2(-40, 22);

            var title = UIFactory.Label("Title", panel.transform, "Upgrades", UITheme.FontHeader, UITheme.TextPrimary, TextAnchor.UpperCenter);
            title.fontStyle = FontStyle.Bold;
            var tRt = title.rectTransform;
            tRt.anchorMin = new Vector2(0, 1);
            tRt.anchorMax = new Vector2(1, 1);
            tRt.pivot     = new Vector2(0.5f, 1);
            tRt.anchoredPosition = new Vector2(0, -38);
            tRt.sizeDelta = new Vector2(-40, 40);

            var listGo = new GameObject("List", typeof(RectTransform));
            listGo.transform.SetParent(panel.transform, false);
            _list = (RectTransform)listGo.transform;
            _list.anchorMin = new Vector2(0, 0);
            _list.anchorMax = new Vector2(1, 1);
            _list.pivot     = new Vector2(0.5f, 1);
            _list.offsetMin = new Vector2(24, 80);
            _list.offsetMax = new Vector2(-24, -90);

            var layout = listGo.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 10;
            layout.childAlignment = TextAnchor.UpperLeft;
            layout.childControlWidth = true;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            var close = UIFactory.TextButton("Close", panel.transform, "Close (B)", new Vector2(220, 48));
            var clRt = (RectTransform)close.transform;
            clRt.anchorMin = new Vector2(0.5f, 0);
            clRt.anchorMax = new Vector2(0.5f, 0);
            clRt.pivot     = new Vector2(0.5f, 0);
            clRt.anchoredPosition = new Vector2(0, 18);
            close.onClick.AddListener(Close);

            BuildRows();
            _root.gameObject.SetActive(false);
        }

        void BuildRows()
        {
            for (int i = 0; i < _rows.Count; i++) if (_rows[i] != null) Destroy(_rows[i]);
            _rows.Clear();

            foreach (var def in Upgrades.Catalog) _rows.Add(BuildRow(def));
        }

        GameObject BuildRow(UpgradeDef def)
        {
            var row = UIFactory.Panel("Row_" + def.Id, _list, UITheme.PanelLight, 10);
            var le = row.AddComponent<LayoutElement>();
            le.preferredHeight = 76;
            le.minHeight = 76;

            var name = UIFactory.Label("Name", row.transform, def.Name, UITheme.FontBody, UITheme.TextPrimary, TextAnchor.UpperLeft);
            name.fontStyle = FontStyle.Bold;
            var nRt = name.rectTransform;
            nRt.anchorMin = new Vector2(0, 1);
            nRt.anchorMax = new Vector2(1, 1);
            nRt.pivot     = new Vector2(0, 1);
            nRt.anchoredPosition = new Vector2(18, -10);
            nRt.sizeDelta = new Vector2(-200, 22);

            var desc = UIFactory.Label("Desc", row.transform, def.Description, UITheme.FontSmall, UITheme.TextSecondary, TextAnchor.UpperLeft);
            var dRt = desc.rectTransform;
            dRt.anchorMin = new Vector2(0, 1);
            dRt.anchorMax = new Vector2(1, 1);
            dRt.pivot     = new Vector2(0, 1);
            dRt.anchoredPosition = new Vector2(18, -36);
            dRt.sizeDelta = new Vector2(-200, 32);

            var requires = UIFactory.Label("Req", row.transform, "Req Lv " + def.LevelRequired, UITheme.FontSmall, UITheme.TextSecondary, TextAnchor.LowerLeft);
            var rqRt = requires.rectTransform;
            rqRt.anchorMin = new Vector2(0, 0);
            rqRt.anchorMax = new Vector2(0, 0);
            rqRt.pivot     = new Vector2(0, 0);
            rqRt.anchoredPosition = new Vector2(18, 8);
            rqRt.sizeDelta = new Vector2(200, 18);

            var btn = UIFactory.TextButton("Buy", row.transform, "$" + def.Price, new Vector2(160, 48));
            var btRt = (RectTransform)btn.transform;
            btRt.anchorMin = new Vector2(1, 0.5f);
            btRt.anchorMax = new Vector2(1, 0.5f);
            btRt.pivot     = new Vector2(1, 0.5f);
            btRt.anchoredPosition = new Vector2(-14, 0);
            btn.onClick.AddListener(() => OnBuy(def, btn));

            return row;
        }

        void OnBuy(UpgradeDef def, Button btn)
        {
            if (Upgrades.TryPurchase(def.Id))
            {
                RefreshRow(def, btn);
            }
        }

        void RefreshRows()
        {
            if (_list == null) return;
            int i = 0;
            foreach (var def in Upgrades.Catalog)
            {
                if (i < _rows.Count && _rows[i] != null)
                {
                    var btn = _rows[i].GetComponentInChildren<Button>(true);
                    RefreshRow(def, btn);
                }
                i++;
            }
        }

        void RefreshRow(UpgradeDef def, Button btn)
        {
            if (btn == null) return;
            var label = btn.GetComponentInChildren<Text>();
            if (Upgrades.IsOwned(def.Id))
            {
                if (label != null) label.text = "OWNED";
                btn.interactable = false;
                return;
            }
            if (PlayerProgression.Level < def.LevelRequired)
            {
                if (label != null) label.text = "Lv " + def.LevelRequired;
                btn.interactable = false;
                return;
            }
            if (label != null) label.text = "$" + def.Price;
            btn.interactable = Upgrades.CanAfford(def);
        }

        Transform FindHudParent()
        {
            var gameUI = GameUI.Instance;
            if (gameUI == null) return null;
            return gameUI.transform.Find("GameUI_Canvas/HUD");
        }

        public void Toggle() { if (_isOpen) Close(); else Open(); }

        public void Open()
        {
            if (_root == null) return;
            _root.gameObject.SetActive(true);
            _root.SetAsLastSibling();
            _isOpen = true;
            RefreshRows();
            UIEvents.SetPaused(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        public void Close()
        {
            if (_root == null) return;
            _root.gameObject.SetActive(false);
            _isOpen = false;
            UIEvents.SetPaused(false);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
