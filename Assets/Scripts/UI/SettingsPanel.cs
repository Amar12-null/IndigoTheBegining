using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Indigo.UI
{
    public class SettingsPanel : MonoBehaviour
    {
        GameObject       _root;
        CanvasGroup      _cg;
        RectTransform    _cardRt;
        UIBackgroundBlur _blur;
        System.Action    _onBack;
        Coroutine        _animCo;
        bool             _open, _closing;

        public bool IsOpen => _open && !_closing;

        public void Build(RectTransform parent, System.Action onBack)
        {
            _onBack = onBack;

            var root = new GameObject("SettingsOverlay", typeof(RectTransform), typeof(CanvasGroup));
            root.transform.SetParent(parent, false);
            UIFactory.StretchToParent((RectTransform)root.transform);
            _root = root;
            _cg = root.GetComponent<CanvasGroup>();

            var blurGo = new GameObject("Blur", typeof(RectTransform), typeof(CanvasRenderer), typeof(RawImage));
            blurGo.transform.SetParent(root.transform, false);
            UIFactory.StretchToParent((RectTransform)blurGo.transform);
            _blur = blurGo.AddComponent<UIBackgroundBlur>();

            var dim = UIFactory.Panel("Dim", root.transform, new Color(0f, 0f, 0f, 0.45f));
            UIFactory.StretchToParent((RectTransform)dim.transform);

            var card = UIFactory.Panel("Card", root.transform, UITheme.Panel, 20);
            _cardRt = (RectTransform)card.transform;
            _cardRt.anchorMin = new Vector2(0.5f, 0.5f);
            _cardRt.anchorMax = new Vector2(0.5f, 0.5f);
            _cardRt.pivot     = new Vector2(0.5f, 0.5f);
            _cardRt.sizeDelta = new Vector2(520, 460);
            UIFactory.AddDropShadow(_cardRt, new Vector2(0, -8), 0.45f, 24, 18f);
            UIFactory.AddStroke(_cardRt, UITheme.Stroke, 1.5f, 20);

            var title = UIFactory.Label("Title", card.transform, "SETTINGS", UITheme.FontHeader, UITheme.TextPrimary, TextAnchor.UpperLeft);
            title.fontStyle = FontStyle.Bold;
            var titleRt = title.rectTransform;
            titleRt.anchorMin = new Vector2(0, 1);
            titleRt.anchorMax = new Vector2(1, 1);
            titleRt.pivot     = new Vector2(0, 1);
            titleRt.anchoredPosition = new Vector2(32, -28);
            titleRt.sizeDelta = new Vector2(-64, 40);

            var accent = UIFactory.Panel("Accent", card.transform, UITheme.Accent, 2);
            var aRt = (RectTransform)accent.transform;
            aRt.anchorMin = new Vector2(0, 1);
            aRt.anchorMax = new Vector2(0, 1);
            aRt.pivot     = new Vector2(0, 1);
            aRt.anchoredPosition = new Vector2(32, -76);
            aRt.sizeDelta = new Vector2(48, 3);

            AddSliderRow(card.transform, "Master Volume", -110, 0f, 1f, SettingsService.MasterVolume,
                v => SettingsService.MasterVolume = v, v => Mathf.RoundToInt(v * 100f) + "%");

            AddSliderRow(card.transform, "Mouse Sensitivity", -180, 0.5f, 8f, SettingsService.MouseSensitivity,
                v => SettingsService.MouseSensitivity = v, v => v.ToString("0.0"));

            AddToggleRow(card.transform, "Fullscreen", -250, SettingsService.Fullscreen,
                v => SettingsService.Fullscreen = v);

            var back = UIFactory.TextButton("BackButton", card.transform, "Back", new Vector2(160, 48));
            var backRt = back.GetComponent<RectTransform>();
            backRt.anchorMin = new Vector2(0.5f, 0);
            backRt.anchorMax = new Vector2(0.5f, 0);
            backRt.pivot     = new Vector2(0.5f, 0);
            backRt.anchoredPosition = new Vector2(0, 32);
            back.onClick.AddListener(Close);

            _cg.alpha = 0f;
            _root.SetActive(false);
        }

        void AddSliderRow(Transform parent, string labelText, float yOffset, float min, float max, float value,
                          System.Action<float> onChange, System.Func<float, string> formatter)
        {
            var row = new GameObject(labelText, typeof(RectTransform));
            row.transform.SetParent(parent, false);
            var rRt = (RectTransform)row.transform;
            rRt.anchorMin = new Vector2(0, 1);
            rRt.anchorMax = new Vector2(1, 1);
            rRt.pivot     = new Vector2(0.5f, 1);
            rRt.anchoredPosition = new Vector2(0, yOffset);
            rRt.sizeDelta = new Vector2(-64, 56);

            var label = UIFactory.Label("Label", row.transform, labelText, UITheme.FontBody, UITheme.TextPrimary, TextAnchor.MiddleLeft);
            var lRt = label.rectTransform;
            lRt.anchorMin = new Vector2(0, 0);
            lRt.anchorMax = new Vector2(0, 1);
            lRt.pivot     = new Vector2(0, 0.5f);
            lRt.anchoredPosition = new Vector2(0, 0);
            lRt.sizeDelta = new Vector2(220, 0);

            var valueLabel = UIFactory.Label("Value", row.transform, formatter(value), UITheme.FontBody, UITheme.Accent, TextAnchor.MiddleRight);
            var vRt = valueLabel.rectTransform;
            vRt.anchorMin = new Vector2(1, 0);
            vRt.anchorMax = new Vector2(1, 1);
            vRt.pivot     = new Vector2(1, 0.5f);
            vRt.anchoredPosition = new Vector2(0, 0);
            vRt.sizeDelta = new Vector2(80, 0);

            var slider = UIFactory.HorizontalSlider("Slider", row.transform, new Vector2(220, 32), min, max, value);
            var sRt = (RectTransform)slider.transform;
            sRt.anchorMin = new Vector2(0.5f, 0.5f);
            sRt.anchorMax = new Vector2(0.5f, 0.5f);
            sRt.pivot     = new Vector2(0.5f, 0.5f);
            sRt.anchoredPosition = new Vector2(20, 0);
            sRt.sizeDelta = new Vector2(220, 32);

            slider.onValueChanged.AddListener(v =>
            {
                onChange(v);
                valueLabel.text = formatter(v);
            });
        }

        void AddToggleRow(Transform parent, string labelText, float yOffset, bool value, System.Action<bool> onChange)
        {
            var row = new GameObject(labelText, typeof(RectTransform));
            row.transform.SetParent(parent, false);
            var rRt = (RectTransform)row.transform;
            rRt.anchorMin = new Vector2(0, 1);
            rRt.anchorMax = new Vector2(1, 1);
            rRt.pivot     = new Vector2(0.5f, 1);
            rRt.anchoredPosition = new Vector2(0, yOffset);
            rRt.sizeDelta = new Vector2(-64, 40);

            var label = UIFactory.Label("Label", row.transform, labelText, UITheme.FontBody, UITheme.TextPrimary, TextAnchor.MiddleLeft);
            var lRt = label.rectTransform;
            lRt.anchorMin = new Vector2(0, 0);
            lRt.anchorMax = new Vector2(0, 1);
            lRt.pivot     = new Vector2(0, 0.5f);
            lRt.sizeDelta = new Vector2(220, 0);

            var toggle = UIFactory.CheckboxToggle("Toggle", row.transform, labelText, value);
            var tRt = (RectTransform)toggle.transform;
            tRt.anchorMin = new Vector2(1, 0.5f);
            tRt.anchorMax = new Vector2(1, 0.5f);
            tRt.pivot     = new Vector2(1, 0.5f);
            tRt.anchoredPosition = new Vector2(0, 0);
            tRt.sizeDelta = new Vector2(32, 32);
            toggle.onValueChanged.AddListener(v => onChange(v));
        }

        public void Open()
        {
            if (_open) return;
            _open = true;
            _closing = false;
            _cg.alpha = 0f;
            _root.SetActive(true);
            if (_blur != null) _blur.Capture();
            if (_animCo != null) StopCoroutine(_animCo);
            _animCo = StartCoroutine(UIAnimator.FadeScaleIn(_cg, _cardRt, 0.18f));
        }

        public void Close()
        {
            if (!_open || _closing) return;
            _closing = true;
            if (_animCo != null) StopCoroutine(_animCo);
            _animCo = StartCoroutine(CloseRoutine());
        }

        IEnumerator CloseRoutine()
        {
            yield return UIAnimator.FadeScaleOut(_cg, _cardRt, 0.14f);
            _root.SetActive(false);
            _open = false;
            _closing = false;
            _onBack?.Invoke();
        }
    }
}
