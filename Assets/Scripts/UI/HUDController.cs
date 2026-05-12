using UnityEngine;
using UnityEngine.UI;

namespace Indigo.UI
{
    public class HUDController : MonoBehaviour
    {
        private RectTransform _promptPanel;
        private GameObject    _promptKey;
        private Text          _promptKeyLabel;
        private Text          _promptText;

        private RectTransform _heldPanel;
        private Text          _heldTitle;
        private Text          _heldState;

        public void Build(RectTransform parent)
        {
            BuildPrompt(parent);
            BuildHeldCard(parent);
            UIEvents.OnInteractionPromptChanged += HandlePrompt;
            UIEvents.OnHeldItemChanged          += HandleHeld;
            HandlePrompt(UIEvents.CurrentPromptKey, UIEvents.CurrentPromptText);
            HandleHeld(UIEvents.CurrentHeldName, UIEvents.CurrentHeldState);
        }

        void OnDestroy()
        {
            UIEvents.OnInteractionPromptChanged -= HandlePrompt;
            UIEvents.OnHeldItemChanged          -= HandleHeld;
        }

        void BuildPrompt(RectTransform parent)
        {
            var go = UIFactory.Panel("InteractionPrompt", parent, UITheme.Background);
            _promptPanel = (RectTransform)go.transform;
            _promptPanel.anchorMin = new Vector2(0.5f, 0f);
            _promptPanel.anchorMax = new Vector2(0.5f, 0f);
            _promptPanel.pivot     = new Vector2(0.5f, 0f);
            _promptPanel.anchoredPosition = new Vector2(0, 120);
            _promptPanel.sizeDelta = new Vector2(360, 56);

            var layout = go.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(16, 18, 12, 12);
            layout.spacing = 12;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlHeight = true;
            layout.childControlWidth  = false;
            layout.childForceExpandHeight = false;
            layout.childForceExpandWidth  = false;

            var fitter = go.AddComponent<ContentSizeFitter>();
            fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            fitter.verticalFit   = ContentSizeFitter.FitMode.PreferredSize;

            _promptKey = UIFactory.KeyCap("Key", go.transform, "E");
            var keyRt = (RectTransform)_promptKey.transform;
            keyRt.sizeDelta = new Vector2(36, 36);
            var keyLayout = _promptKey.AddComponent<LayoutElement>();
            keyLayout.preferredWidth  = 36;
            keyLayout.preferredHeight = 36;
            _promptKeyLabel = _promptKey.GetComponentInChildren<Text>();

            _promptText = UIFactory.Label("Text", go.transform, "", UITheme.FontBody, UITheme.TextPrimary, TextAnchor.MiddleLeft);
            var textLayout = _promptText.gameObject.AddComponent<LayoutElement>();
            textLayout.preferredHeight = 36;

            _promptPanel.gameObject.SetActive(false);
        }

        void BuildHeldCard(RectTransform parent)
        {
            var go = UIFactory.Panel("HeldItemCard", parent, UITheme.Background);
            _heldPanel = (RectTransform)go.transform;
            _heldPanel.anchorMin = new Vector2(1f, 0f);
            _heldPanel.anchorMax = new Vector2(1f, 0f);
            _heldPanel.pivot     = new Vector2(1f, 0f);
            _heldPanel.anchoredPosition = new Vector2(-24, 24);
            _heldPanel.sizeDelta = new Vector2(240, 76);

            var accent = UIFactory.Panel("Accent", go.transform, UITheme.Accent);
            var aRt = (RectTransform)accent.transform;
            aRt.anchorMin = new Vector2(0, 0);
            aRt.anchorMax = new Vector2(0, 1);
            aRt.pivot     = new Vector2(0, 0.5f);
            aRt.sizeDelta = new Vector2(4, 0);
            aRt.anchoredPosition = Vector2.zero;

            var caption = UIFactory.Label("Caption", go.transform, "HOLDING", UITheme.FontSmall, UITheme.TextSecondary, TextAnchor.UpperLeft);
            var cRt = caption.rectTransform;
            cRt.anchorMin = new Vector2(0, 1);
            cRt.anchorMax = new Vector2(1, 1);
            cRt.pivot     = new Vector2(0, 1);
            cRt.anchoredPosition = new Vector2(16, -10);
            cRt.sizeDelta = new Vector2(-24, 18);

            _heldTitle = UIFactory.Label("Title", go.transform, "Nothing", UITheme.FontBody, UITheme.TextPrimary, TextAnchor.UpperLeft);
            _heldTitle.fontStyle = FontStyle.Bold;
            var tRt = _heldTitle.rectTransform;
            tRt.anchorMin = new Vector2(0, 1);
            tRt.anchorMax = new Vector2(1, 1);
            tRt.pivot     = new Vector2(0, 1);
            tRt.anchoredPosition = new Vector2(16, -28);
            tRt.sizeDelta = new Vector2(-24, 24);

            _heldState = UIFactory.Label("State", go.transform, "", UITheme.FontSmall, UITheme.Accent, TextAnchor.LowerLeft);
            var sRt = _heldState.rectTransform;
            sRt.anchorMin = new Vector2(0, 0);
            sRt.anchorMax = new Vector2(1, 0);
            sRt.pivot     = new Vector2(0, 0);
            sRt.anchoredPosition = new Vector2(16, 10);
            sRt.sizeDelta = new Vector2(-24, 18);

            _heldPanel.gameObject.SetActive(false);
        }

        void HandlePrompt(string key, string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                _promptPanel.gameObject.SetActive(false);
                return;
            }
            _promptPanel.gameObject.SetActive(true);
            _promptKeyLabel.text = string.IsNullOrEmpty(key) ? "E" : key;
            _promptText.text = text;
        }

        void HandleHeld(string name, string state)
        {
            if (string.IsNullOrEmpty(name))
            {
                _heldPanel.gameObject.SetActive(false);
                return;
            }
            _heldPanel.gameObject.SetActive(true);
            _heldTitle.text = name;
            _heldState.text = string.IsNullOrEmpty(state) ? "" : state.ToUpperInvariant();
        }
    }
}
