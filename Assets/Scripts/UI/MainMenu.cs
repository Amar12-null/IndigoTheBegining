using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Indigo.UI
{
    [DisallowMultipleComponent]
    public class MainMenu : MonoBehaviour
    {
        public string PlayScene = "SampleScene";
        public string GameTitle = "INDIGO";
        public string Subtitle  = "The Beginning";

        SettingsPanel _settings;
        CanvasGroup   _cg;

        void Awake()
        {
            SettingsService.ApplyOnLoad();
            Cursor.visible   = true;
            Cursor.lockState = CursorLockMode.None;
            BuildUI();
            StartCoroutine(UIAnimator.FadeScaleIn(_cg, (RectTransform)_cg.transform, 0.35f, 0.985f));
        }

        void BuildUI()
        {
            UIFactory.EnsureEventSystem();

            var canvasGo = new GameObject("MainMenu_Canvas",
                typeof(RectTransform), typeof(Canvas),
                typeof(CanvasScaler), typeof(GraphicRaycaster), typeof(CanvasGroup));
            canvasGo.transform.SetParent(transform, false);
            _cg = canvasGo.GetComponent<CanvasGroup>();

            var canvas = canvasGo.GetComponent<Canvas>();
            canvas.renderMode  = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;

            var scaler = canvasGo.GetComponent<CanvasScaler>();
            scaler.uiScaleMode         = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.screenMatchMode     = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight  = 0.5f;

            var root = (RectTransform)canvasGo.transform;

            var bg = UIFactory.Panel("Background", root, new Color(0.05f, 0.06f, 0.09f, 1f));
            UIFactory.StretchToParent((RectTransform)bg.transform);

            var accentBar = UIFactory.Panel("AccentBar", root, UITheme.Accent, 4);
            var abRt = (RectTransform)accentBar.transform;
            abRt.anchorMin = new Vector2(0, 0.5f);
            abRt.anchorMax = new Vector2(0, 0.5f);
            abRt.pivot     = new Vector2(0, 0.5f);
            abRt.sizeDelta = new Vector2(8, 220);
            abRt.anchoredPosition = new Vector2(160, 80);

            var title = UIFactory.Label("Title", root, GameTitle, UITheme.FontTitle, UITheme.TextPrimary, TextAnchor.MiddleLeft);
            title.fontStyle = FontStyle.Bold;
            var tRt = title.rectTransform;
            tRt.anchorMin = new Vector2(0, 0.5f);
            tRt.anchorMax = new Vector2(0, 0.5f);
            tRt.pivot     = new Vector2(0, 0.5f);
            tRt.sizeDelta = new Vector2(800, 80);
            tRt.anchoredPosition = new Vector2(200, 150);

            var underline = UIFactory.Panel("TitleUnderline", root, UITheme.Accent, 2);
            var unRt = (RectTransform)underline.transform;
            unRt.anchorMin = new Vector2(0, 0.5f);
            unRt.anchorMax = new Vector2(0, 0.5f);
            unRt.pivot     = new Vector2(0, 0.5f);
            unRt.sizeDelta = new Vector2(240, 4);
            unRt.anchoredPosition = new Vector2(200, 108);
            var pulse = underline.AddComponent<UIPulse>();
            pulse.scaleTarget = unRt;
            pulse.pulseAlpha  = true;
            pulse.minAlpha    = 0.55f;
            pulse.maxAlpha    = 1f;
            pulse.pulseScale  = true;
            pulse.scaleX      = true;
            pulse.scaleY      = false;
            pulse.minScale    = 0.65f;
            pulse.maxScale    = 1.0f;
            pulse.speed       = 1.6f;

            var sub = UIFactory.Label("Subtitle", root, Subtitle, UITheme.FontHeader, UITheme.Accent, TextAnchor.MiddleLeft);
            var sRt = sub.rectTransform;
            sRt.anchorMin = new Vector2(0, 0.5f);
            sRt.anchorMax = new Vector2(0, 0.5f);
            sRt.pivot     = new Vector2(0, 0.5f);
            sRt.sizeDelta = new Vector2(800, 40);
            sRt.anchoredPosition = new Vector2(200, 70);

            var buttons = new GameObject("Buttons", typeof(RectTransform));
            buttons.transform.SetParent(root, false);
            var bRt = (RectTransform)buttons.transform;
            bRt.anchorMin = new Vector2(0, 0.5f);
            bRt.anchorMax = new Vector2(0, 0.5f);
            bRt.pivot     = new Vector2(0, 0.5f);
            bRt.sizeDelta = new Vector2(260, 240);
            bRt.anchoredPosition = new Vector2(200, -80);

            var layout = buttons.AddComponent<VerticalLayoutGroup>();
            layout.spacing  = 12;
            layout.padding  = new RectOffset(0, 0, 0, 0);
            layout.childAlignment        = TextAnchor.UpperLeft;
            layout.childControlWidth     = true;
            layout.childControlHeight    = false;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            AddMenuButton(buttons.transform, "Play",     OnPlay);
            AddMenuButton(buttons.transform, "Settings", OnSettings);
            AddMenuButton(buttons.transform, "Quit",     OnQuit);

            var hint = UIFactory.Label("Hint", root, "v0.1  •  built with Unity 6", UITheme.FontSmall, UITheme.TextSecondary, TextAnchor.LowerRight);
            var hRt = hint.rectTransform;
            hRt.anchorMin = new Vector2(1, 0);
            hRt.anchorMax = new Vector2(1, 0);
            hRt.pivot     = new Vector2(1, 0);
            hRt.sizeDelta = new Vector2(400, 24);
            hRt.anchoredPosition = new Vector2(-24, 24);

            var settingsLayer = new GameObject("SettingsLayer", typeof(RectTransform));
            settingsLayer.transform.SetParent(root, false);
            UIFactory.StretchToParent((RectTransform)settingsLayer.transform);

            _settings = gameObject.AddComponent<SettingsPanel>();
            _settings.Build((RectTransform)settingsLayer.transform, onBack: () => { });
            var pauseMenu = gameObject.AddComponent<PauseMenu>();
            pauseMenu.Build(root, _settings);
        }

        void AddMenuButton(Transform parent, string label, UnityEngine.Events.UnityAction onClick)
        {
            var btn = UIFactory.TextButton(label + "Button", parent, label, new Vector2(260, 56));
            var le  = btn.gameObject.AddComponent<LayoutElement>();
            le.preferredHeight = 56;
            le.minHeight       = 56;

            var lbl = btn.GetComponentInChildren<Text>();
            if (lbl != null) lbl.alignment = TextAnchor.MiddleLeft;
            var lblRt = lbl.rectTransform;
            lblRt.offsetMin = new Vector2(24, 0);
            lblRt.offsetMax = new Vector2(-16, 0);

            btn.onClick.AddListener(onClick);
        }

        void OnPlay()
        {
        // Umjesto učitavanja nove scene, javljamo GameManageru da igra počinje!
        GameManager.Instance.UpdateGameState(GameState.Playing);
    
        // Gasimo MainMenu Canvas kako bi igrač vidio igru
        _cg.alpha = 0; 
        _cg.interactable = false;
        _cg.blocksRaycasts = false;
        }

        void OnSettings() { _settings?.Open(); }

        void OnQuit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
