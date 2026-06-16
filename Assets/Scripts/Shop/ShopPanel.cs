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

        [Header("Scroll Settings")]
        [Tooltip("Brzina skrolovanja točkićem miša.")]
        public float scrollSpeedMultiplier = 35f; 

        private RectTransform _root;
        private RectTransform _list;
        private bool _isOpen;
        private readonly List<GameObject> _rows = new List<GameObject>();

        private readonly Color _themeGold = new Color(0.95f, 0.72f, 0.22f, 1f);       
        private readonly Color _themeGoldMuted = new Color(0.95f, 0.72f, 0.22f, 0.35f); 
        private readonly Color _themeGreenMuted = new Color(0.40f, 0.85f, 0.55f, 0.4f); 

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

        void Update() {}

        void BuildPanel()
        {
            var canvas = FindHudParent();
            if (canvas == null)
            {
                Debug.LogWarning("ShopPanel: HUD layer not found.");
                return;
            }

            var dim = UIFactory.Panel("ShopDim", canvas, new Color(0f, 0f, 0f, 0.75f)); 
            UIFactory.StretchToParent((RectTransform)dim.transform);
            _root = (RectTransform)dim.transform;

            var panel = UIFactory.Panel("ShopPanel", _root, UITheme.Panel, 14);
            var pRt = (RectTransform)panel.transform;
            pRt.anchorMin = new Vector2(0.5f, 0.5f);
            pRt.anchorMax = new Vector2(0.5f, 0.5f);
            pRt.pivot     = new Vector2(0.5f, 0.5f);
            pRt.sizeDelta = new Vector2(840, 560);
            UIFactory.AddDropShadow(pRt, new Vector2(0, -8), 0.5f, 25, 16f);
            UIFactory.AddStroke(pRt, UITheme.Stroke, 1.5f, 14);

            var closeBtn = UIFactory.TextButton("CloseX", panel.transform, "X", new Vector2(36, 36));
            var clRt = (RectTransform)closeBtn.transform;
            clRt.anchorMin = new Vector2(1, 1);
            clRt.anchorMax = new Vector2(1, 1);
            clRt.pivot     = new Vector2(1, 1);
            clRt.anchoredPosition = new Vector2(-20, -18); 
            
            var closeImage = closeBtn.GetComponent<Image>();
            if (closeImage != null) closeImage.color = UITheme.PanelLight;
            var closeText = closeBtn.GetComponentInChildren<Text>();
            if (closeText != null) 
            {
                closeText.color = _themeGold;
                closeText.fontStyle = FontStyle.Bold;
            }
            closeBtn.onClick.AddListener(Close);

            var caption = UIFactory.Label("Caption", panel.transform, "MANAGEMENT STATION", UITheme.FontSmall, _themeGold, TextAnchor.UpperLeft);
            var cRt = caption.rectTransform;
            cRt.anchorMin = new Vector2(0, 1);
            cRt.anchorMax = new Vector2(1, 1);
            cRt.pivot     = new Vector2(0, 1);
            cRt.anchoredPosition = new Vector2(24, -18);
            cRt.sizeDelta = new Vector2(-100, 22);

            var title = UIFactory.Label("Title", panel.transform, "Cafe Upgrades & Expansion", UITheme.FontHeader, UITheme.TextPrimary, TextAnchor.UpperLeft);
            title.fontStyle = FontStyle.Bold;
            var tRt = title.rectTransform;
            tRt.anchorMin = new Vector2(0, 1);
            tRt.anchorMax = new Vector2(1, 1);
            tRt.pivot     = new Vector2(0, 1);
            tRt.anchoredPosition = new Vector2(24, -38);
            tRt.sizeDelta = new Vector2(-100, 40);

            var sep = UIFactory.Panel("Line", panel.transform, new Color(_themeGold.r, _themeGold.g, _themeGold.b, 0.4f));
            var sRt = (RectTransform)sep.transform;
            sRt.anchorMin = new Vector2(0.5f, 1);
            sRt.anchorMax = new Vector2(0.5f, 1);
            sRt.pivot = new Vector2(0.5f, 1);
            sRt.anchoredPosition = new Vector2(0, -85);
            sRt.sizeDelta = new Vector2(792, 2); 

            var scrollWindow = new GameObject("ScrollWindow", typeof(RectTransform));
            scrollWindow.transform.SetParent(panel.transform, false);
            var swRt = (RectTransform)scrollWindow.transform;
            swRt.anchorMin = new Vector2(0f, 0f);
            swRt.anchorMax = new Vector2(1f, 1f);
            swRt.pivot     = new Vector2(0.5f, 1f);
            swRt.offsetMin = new Vector2(24, 24);   
            swRt.offsetMax = new Vector2(-24, -105); 

            scrollWindow.AddComponent<RectMask2D>();

            var listGo = new GameObject("List", typeof(RectTransform));
            listGo.transform.SetParent(scrollWindow.transform, false);
            _list = (RectTransform)listGo.transform;
            _list.anchorMin = new Vector2(0f, 1f); 
            _list.anchorMax = new Vector2(1f, 1f);
            _list.pivot     = new Vector2(0.5f, 1f);
            _list.anchoredPosition = Vector2.zero;
            _list.offsetMin = Vector2.zero; 
            _list.offsetMax = Vector2.zero;

            // --- POPRAVLJENO: Layout Group dobija donji padding! ---
            var layout = listGo.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 8;
            layout.padding = new RectOffset(0, 0, 10, 80); // 80 piksela praznog prostora ispod zadnjeg elementa (Bašte)
            layout.childAlignment = TextAnchor.UpperLeft;
            layout.childControlWidth = true;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            // --- NOVO: Unityjev alat koji automatski kontroliše skrol visinu! ---
            var fitter = listGo.AddComponent<ContentSizeFitter>();
            fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            var scroll = scrollWindow.AddComponent<ScrollRect>();
            scroll.content = _list;
            scroll.horizontal = false;
            scroll.vertical = true;
            scroll.movementType = ScrollRect.MovementType.Clamped;
            scroll.scrollSensitivity = scrollSpeedMultiplier;

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
            var row = UIFactory.Panel("Row_" + def.Id, _list, UITheme.PanelLight, 8);
            var le = row.AddComponent<LayoutElement>();
            le.preferredHeight = 72;
            le.minHeight = 72;

            string categoryTag = $"[{def.Category}] ";
            
            var name = UIFactory.Label("Name", row.transform, categoryTag + def.Name, UITheme.FontBody, UITheme.TextPrimary, TextAnchor.UpperLeft);
            name.fontStyle = FontStyle.Bold;
            var nRt = name.rectTransform;
            nRt.anchorMin = new Vector2(0, 1);
            nRt.anchorMax = new Vector2(1, 1);
            nRt.pivot     = new Vector2(0, 1);
            nRt.anchoredPosition = new Vector2(24, -12);
            nRt.sizeDelta = new Vector2(-190, 22); 

            var desc = UIFactory.Label("Desc", row.transform, def.Description, UITheme.FontSmall, UITheme.TextSecondary, TextAnchor.UpperLeft);
            var dRt = desc.rectTransform;
            dRt.anchorMin = new Vector2(0, 1);
            dRt.anchorMax = new Vector2(1, 1);
            dRt.pivot     = new Vector2(0, 1);
            dRt.anchoredPosition = new Vector2(24, -36);
            dRt.sizeDelta = new Vector2(-190, 28); 

            var requires = UIFactory.Label("Req", row.transform, "Required Level: " + def.LevelRequired, UITheme.FontSmall, UITheme.TextSecondary, TextAnchor.LowerLeft);
            var rqRt = requires.rectTransform;
            rqRt.anchorMin = new Vector2(0, 0);
            rqRt.anchorMax = new Vector2(0, 0);
            rqRt.pivot     = new Vector2(0, 0);
            rqRt.anchoredPosition = new Vector2(24, 8);
            rqRt.sizeDelta = new Vector2(200, 16);

            var btn = UIFactory.TextButton("Buy", row.transform, "$" + def.Price, new Vector2(120, 42));
            var btRt = (RectTransform)btn.transform;
            btRt.anchorMin = new Vector2(1, 0.5f);
            btRt.anchorMax = new Vector2(1, 0.5f);
            btRt.pivot     = new Vector2(1, 0.5f);
            btRt.anchoredPosition = new Vector2(-24, 0); 
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
            var image = btn.GetComponent<Image>();

            if (label != null) label.color = Color.white;

            if (Upgrades.IsOwned(def.Id))
            {
                if (label != null) label.text = "OWNED";
                if (image != null) image.color = _themeGreenMuted;
                btn.interactable = false;
                return;
            }
            if (PlayerProgression.Level < def.LevelRequired)
            {
                if (label != null) label.text = "LVL " + def.LevelRequired;
                if (image != null) image.color = new Color(0.25f, 0.25f, 0.25f, 0.6f);
                btn.interactable = false;
                return;
            }
            
            if (label != null) label.text = "$" + def.Price;
            btn.interactable = Upgrades.CanAfford(def);
            
            if (image != null)
            {
                image.color = btn.interactable ? _themeGold : _themeGoldMuted;
            }
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

            BuildRows();
            RefreshRows();

            UIEvents.SetPaused(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            if (Indigo.Game.GameStateManager.Instance != null)
            {
                Indigo.Game.GameStateManager.Instance.NotifyShoppingOpened();
            }
        }

        public void Close()
        {
            if (_root == null) return;
            _root.gameObject.SetActive(false);
            _isOpen = false;
            UIEvents.SetPaused(false);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            if (Indigo.Game.GameStateManager.Instance != null)
            {
                Indigo.Game.GameStateManager.Instance.NotifyShoppingClosed();
            }
        }
    }
}