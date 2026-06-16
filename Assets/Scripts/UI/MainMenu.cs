using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Indigo.UI
{
    [DisallowMultipleComponent]
    public class MainMenu : MonoBehaviour
    {
        public string PlayScene = "KaficDesign";
        public string GameTitle = "INDIGO";
        
        
        [Header("Izgled Menija")]
        public Sprite BackgroundImage;

        // --- NOVO: Polje za muziku ---
        [Header("Muzika")]
        public AudioClip MenuMusic;
        private AudioSource _musicSource;

        SettingsPanel _settings;
        CanvasGroup   _cg;

        void Awake()
        {
            SettingsService.ApplyOnLoad();
            Cursor.visible   = true;
            Cursor.lockState = CursorLockMode.None;
            
            // --- NOVO: Paljenje muzike čim se meni upali ---
            if (MenuMusic != null)
            {
                _musicSource = gameObject.AddComponent<AudioSource>();
                _musicSource.clip = MenuMusic;
                _musicSource.loop = true; // Pjesma se ponavlja beskonačno
                _musicSource.playOnAwake = false;
                _musicSource.volume = SettingsService.MusicVolume; // Čita tvoj klizač iz Settingsa
                _musicSource.Play();
            }

            BuildUI();
            StartCoroutine(UIAnimator.FadeScaleIn(_cg, (RectTransform)_cg.transform, 0.35f, 0.985f));
        }

        // --- NOVO: Ako uđeš u Settings i pomjeriš klizač za muziku, zvuk se odmah stišava/pojačava ---
        void Update()
        {
            if (_musicSource != null)
            {
                _musicSource.volume = SettingsService.MusicVolume;
            }
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

            bool hasImage = BackgroundImage != null;

            if (hasImage)
            {
                var bgGo = new GameObject("BackgroundImage", typeof(RectTransform), typeof(Image));
                bgGo.transform.SetParent(root, false);
                UIFactory.StretchToParent((RectTransform)bgGo.transform);
                
                var bgImg = bgGo.GetComponent<Image>();
                bgImg.sprite = BackgroundImage;
                bgImg.color = Color.white; 
            }
            else
            {
                UIFactory.GradientBackdrop("Background", root, UITheme.BackdropTop, UITheme.BackdropBottom);
            }

            UIFactory.AddVignette(root, 0.55f);

            if (!hasImage)
            {
                var accentBar = UIFactory.Panel("AccentBar", root, UITheme.Accent, 4);
                var abRt = (RectTransform)accentBar.transform;
                abRt.anchorMin = new Vector2(0, 0.5f);
                abRt.anchorMax = new Vector2(0, 0.5f);
                abRt.pivot     = new Vector2(0, 0.5f);
                abRt.sizeDelta = new Vector2(8, 220);
                abRt.anchoredPosition = new Vector2(160, 80);

                var title = UIFactory.Label("Title", root, GameTitle, UITheme.FontTitle, UITheme.TextPrimary, TextAnchor.MiddleLeft);
                title.fontStyle = FontStyle.Bold;
                UIFactory.GradientLabel(title, UITheme.TextPrimary, new Color(0.62f, 0.78f, 0.86f, 1f));
                UIFactory.AddTextShadow(title, new Vector2(2f, -3f), new Color(0f, 0f, 0f, 0.55f));
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

                
                
               
            }

            var buttons = new GameObject("Buttons", typeof(RectTransform));
            buttons.transform.SetParent(root, false);
            var bRt = (RectTransform)buttons.transform;
            bRt.sizeDelta = new Vector2(260, 240);

            if (hasImage)
            {
                bRt.anchorMin = new Vector2(0.5f, 0f);
                bRt.anchorMax = new Vector2(0.5f, 0f);
                bRt.pivot     = new Vector2(0.5f, 0f);
                bRt.anchoredPosition = new Vector2(0, 60); 
            }
            else
            {
                bRt.anchorMin = new Vector2(0, 0.5f);
                bRt.anchorMax = new Vector2(0, 0.5f);
                bRt.pivot     = new Vector2(0, 0.5f);
                bRt.anchoredPosition = new Vector2(200, -80);
            }

            var layout = buttons.AddComponent<VerticalLayoutGroup>();
            layout.spacing  = 12;
            layout.padding  = new RectOffset(0, 0, 0, 0);
            layout.childAlignment        = hasImage ? TextAnchor.UpperCenter : TextAnchor.UpperLeft;
            layout.childControlWidth     = true;
            layout.childControlHeight    = false;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            AddMenuButton(buttons.transform, "Play",     OnPlay, hasImage);
            AddMenuButton(buttons.transform, "Settings", OnSettings, hasImage);
            AddMenuButton(buttons.transform, "Quit",     OnQuit, hasImage);

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
        }

        void AddMenuButton(Transform parent, string label, UnityEngine.Events.UnityAction onClick, bool centered)
        {
            var btn = UIFactory.TextButton(label + "Button", parent, label, new Vector2(260, 56));
            var le  = btn.gameObject.AddComponent<LayoutElement>();
            le.preferredHeight = 56;
            le.minHeight       = 56;

            var lbl = btn.GetComponentInChildren<Text>();
            if (lbl != null) 
            {
                lbl.alignment = centered ? TextAnchor.MiddleCenter : TextAnchor.MiddleLeft;
            }
            
            var lblRt = lbl.rectTransform;
            if (centered)
            {
                lblRt.offsetMin = Vector2.zero;
                lblRt.offsetMax = Vector2.zero;
            }
            else
            {
                lblRt.offsetMin = new Vector2(24, 0);
                lblRt.offsetMax = new Vector2(-16, 0);
            }

            btn.onClick.AddListener(onClick);
        }

        void OnPlay()
        {
            if (Application.CanStreamedLevelBeLoaded(PlayScene))
                SceneManager.LoadScene(PlayScene);
            else
                Debug.LogWarning($"MainMenu: scene '{PlayScene}' is not in Build Settings.");
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