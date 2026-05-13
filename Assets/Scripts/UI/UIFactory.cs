using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Indigo.UI
{
    public static class UIFactory
    {
        public static GameObject Panel(string name, Transform parent, Color color, int radius = 0)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            go.transform.SetParent(parent, false);
            var img = go.GetComponent<Image>();
            img.color = color;
            if (radius > 0)
            {
                img.sprite = UIRounded.Sprite(radius);
                img.type = Image.Type.Sliced;
            }
            return go;
        }

        public static GameObject AddDropShadow(RectTransform target, Vector2 offset, float alpha = 0.35f, int radius = 16, float expand = 12f)
        {
            var go = new GameObject("Shadow", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            go.transform.SetParent(target.parent, false);
            go.transform.SetSiblingIndex(target.GetSiblingIndex());

            var srt = (RectTransform)go.transform;
            srt.anchorMin = target.anchorMin;
            srt.anchorMax = target.anchorMax;
            srt.pivot = target.pivot;
            srt.sizeDelta = target.sizeDelta + new Vector2(expand, expand);
            srt.anchoredPosition = target.anchoredPosition + offset;

            var img = go.GetComponent<Image>();
            img.color = new Color(0f, 0f, 0f, alpha);
            img.raycastTarget = false;
            if (radius > 0)
            {
                img.sprite = UIRounded.Sprite(radius);
                img.type = Image.Type.Sliced;
            }
            return go;
        }

        public static Text Label(string name, Transform parent, string text, int size, Color color, TextAnchor anchor = TextAnchor.MiddleCenter)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            go.transform.SetParent(parent, false);
            var t = go.GetComponent<Text>();
            t.font = UITheme.UIFont;
            t.fontSize = size;
            t.color = color;
            t.alignment = anchor;
            t.text = text;
            t.horizontalOverflow = HorizontalWrapMode.Overflow;
            t.verticalOverflow = VerticalWrapMode.Overflow;
            t.raycastTarget = false;
            return t;
        }

        public static Button TextButton(string name, Transform parent, string label, Vector2 size)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Button));
            go.transform.SetParent(parent, false);
            var rt = (RectTransform)go.transform;
            rt.sizeDelta = size;

            // Focus ring (renders first/behind, slightly larger than face)
            var ring = new GameObject("FocusRing", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            ring.transform.SetParent(go.transform, false);
            var rrt = (RectTransform)ring.transform;
            rrt.anchorMin = Vector2.zero;
            rrt.anchorMax = Vector2.one;
            rrt.offsetMin = new Vector2(-3f, -3f);
            rrt.offsetMax = new Vector2(3f, 3f);
            var rimg = ring.GetComponent<Image>();
            rimg.sprite = UIRounded.Sprite(16);
            rimg.type = Image.Type.Sliced;
            rimg.color = new Color(UITheme.Accent.r, UITheme.Accent.g, UITheme.Accent.b, 0f);
            rimg.raycastTarget = false;

            // Face (the visible button surface — receives raycasts)
            var face = new GameObject("Face", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            face.transform.SetParent(go.transform, false);
            var frt = (RectTransform)face.transform;
            frt.anchorMin = Vector2.zero;
            frt.anchorMax = Vector2.one;
            frt.offsetMin = Vector2.zero;
            frt.offsetMax = Vector2.zero;
            var fimg = face.GetComponent<Image>();
            fimg.sprite = UIRounded.Sprite(14);
            fimg.type = Image.Type.Sliced;
            fimg.color = UITheme.PanelLight;

            var btn = go.GetComponent<Button>();
            btn.targetGraphic = fimg;
            var colors = btn.colors;
            colors.normalColor      = UITheme.PanelLight;
            colors.highlightedColor = UITheme.AccentHover;
            colors.pressedColor     = UITheme.AccentPressed;
            colors.selectedColor    = UITheme.PanelLight;
            colors.disabledColor    = new Color(0.3f, 0.3f, 0.3f, 0.6f);
            colors.colorMultiplier  = 1f;
            colors.fadeDuration     = 0.1f;
            btn.colors = colors;

            var text = Label("Label", go.transform, label, UITheme.FontBody, UITheme.TextPrimary);
            var trt = text.rectTransform;
            trt.anchorMin = Vector2.zero;
            trt.anchorMax = Vector2.one;
            trt.offsetMin = Vector2.zero;
            trt.offsetMax = Vector2.zero;

            var hover = go.AddComponent<UIHoverScale>();
            hover.target = rt;

            var focus = go.AddComponent<UIFocusRing>();
            focus.ring = rimg;

            btn.onClick.AddListener(() =>
            {
                if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject == go)
                    EventSystem.current.SetSelectedGameObject(null);
            });

            return btn;
        }

        public static GameObject KeyCap(string name, Transform parent, string key)
        {
            var go = Panel(name, parent, UITheme.KeyCapBg, 6);
            var rt = (RectTransform)go.transform;
            rt.sizeDelta = new Vector2(32, 32);
            var label = Label("Key", go.transform, key, UITheme.FontSmall, UITheme.KeyCapText);
            label.fontStyle = FontStyle.Bold;
            var lrt = label.rectTransform;
            lrt.anchorMin = Vector2.zero;
            lrt.anchorMax = Vector2.one;
            lrt.offsetMin = Vector2.zero;
            lrt.offsetMax = Vector2.zero;
            return go;
        }

        public static Slider HorizontalSlider(string name, Transform parent, Vector2 size, float min, float max, float value)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Slider));
            go.transform.SetParent(parent, false);
            var rt = (RectTransform)go.transform;
            rt.sizeDelta = size;

            var bg = Panel("Background", go.transform, UITheme.PanelLight, 3);
            var bgRt = (RectTransform)bg.transform;
            bgRt.anchorMin = new Vector2(0, 0.5f);
            bgRt.anchorMax = new Vector2(1, 0.5f);
            bgRt.sizeDelta = new Vector2(0, 6);
            bgRt.offsetMin = new Vector2(0, -3);
            bgRt.offsetMax = new Vector2(0,  3);

            var fillArea = new GameObject("Fill Area", typeof(RectTransform));
            fillArea.transform.SetParent(go.transform, false);
            var farRt = (RectTransform)fillArea.transform;
            farRt.anchorMin = new Vector2(0, 0.5f);
            farRt.anchorMax = new Vector2(1, 0.5f);
            farRt.sizeDelta = new Vector2(-10, 6);
            farRt.offsetMin = new Vector2(5, -3);
            farRt.offsetMax = new Vector2(-5, 3);

            var fill = Panel("Fill", fillArea.transform, UITheme.Accent, 3);
            var fillRt = (RectTransform)fill.transform;
            fillRt.anchorMin = Vector2.zero;
            fillRt.anchorMax = Vector2.one;
            fillRt.sizeDelta = Vector2.zero;

            var handleArea = new GameObject("Handle Slide Area", typeof(RectTransform));
            handleArea.transform.SetParent(go.transform, false);
            var haRt = (RectTransform)handleArea.transform;
            haRt.anchorMin = new Vector2(0, 0);
            haRt.anchorMax = new Vector2(1, 1);
            haRt.offsetMin = new Vector2(10, 0);
            haRt.offsetMax = new Vector2(-10, 0);

            var handle = Panel("Handle", handleArea.transform, UITheme.TextPrimary, 10);
            var hRt = (RectTransform)handle.transform;
            hRt.sizeDelta = new Vector2(20, 20);

            var slider = go.GetComponent<Slider>();
            slider.targetGraphic = handle.GetComponent<Image>();
            slider.fillRect      = fillRt;
            slider.handleRect    = hRt;
            slider.direction     = Slider.Direction.LeftToRight;
            slider.minValue      = min;
            slider.maxValue      = max;
            slider.value         = value;

            return slider;
        }

        public static Toggle CheckboxToggle(string name, Transform parent, string label, bool value)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Toggle));
            go.transform.SetParent(parent, false);
            var rt = (RectTransform)go.transform;
            rt.sizeDelta = new Vector2(40, 28);

            var box = Panel("Box", go.transform, UITheme.PanelLight, 6);
            var bRt = (RectTransform)box.transform;
            bRt.anchorMin = new Vector2(0, 0.5f);
            bRt.anchorMax = new Vector2(0, 0.5f);
            bRt.sizeDelta = new Vector2(28, 28);
            bRt.anchoredPosition = new Vector2(14, 0);

            var check = Panel("Check", box.transform, UITheme.Accent, 4);
            var cRt = (RectTransform)check.transform;
            cRt.anchorMin = new Vector2(0.5f, 0.5f);
            cRt.anchorMax = new Vector2(0.5f, 0.5f);
            cRt.sizeDelta = new Vector2(16, 16);

            var toggle = go.GetComponent<Toggle>();
            toggle.targetGraphic = box.GetComponent<Image>();
            toggle.graphic       = check.GetComponent<Image>();
            toggle.isOn          = value;

            return toggle;
        }

        public static void StretchToParent(RectTransform rt)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }

        public static void EnsureEventSystem()
        {
            if (Object.FindAnyObjectByType<EventSystem>() == null)
            {
                var es = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
                Object.DontDestroyOnLoad(es);
            }
        }
    }
}
