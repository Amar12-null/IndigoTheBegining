using UnityEngine;
using UnityEngine.UI;

namespace Indigo.UI
{
    [DisallowMultipleComponent]
    public class GameUI : MonoBehaviour
    {
        public string MainMenuScene = "MainMenu";

        public static GameUI Instance { get; private set; }
        public HUDController  HUD     { get; private set; }
        public PauseMenu      Pause   { get; private set; }
        public SettingsPanel  Settings{ get; private set; }

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            SettingsService.ApplyOnLoad();
            BuildUI();
        }

        void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        void BuildUI()
        {
            UIFactory.EnsureEventSystem();

            var canvasGo = new GameObject("GameUI_Canvas",
                typeof(RectTransform), typeof(Canvas),
                typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvasGo.transform.SetParent(transform, false);

            var canvas = canvasGo.GetComponent<Canvas>();
            canvas.renderMode  = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;

            var scaler = canvasGo.GetComponent<CanvasScaler>();
            scaler.uiScaleMode             = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution     = new Vector2(1920, 1080);
            scaler.screenMatchMode         = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight      = 0.5f;

            var root = (RectTransform)canvasGo.transform;

            var hudLayer = new GameObject("HUD", typeof(RectTransform));
            hudLayer.transform.SetParent(root, false);
            UIFactory.StretchToParent((RectTransform)hudLayer.transform);

            HUD = gameObject.AddComponent<HUDController>();
            HUD.Build((RectTransform)hudLayer.transform);

            var settingsLayer = new GameObject("SettingsLayer", typeof(RectTransform));
            settingsLayer.transform.SetParent(root, false);
            UIFactory.StretchToParent((RectTransform)settingsLayer.transform);

            Settings = gameObject.AddComponent<SettingsPanel>();
            Settings.Build((RectTransform)settingsLayer.transform, onBack: () => { });

            var pauseLayer = new GameObject("PauseLayer", typeof(RectTransform));
            pauseLayer.transform.SetParent(root, false);
            UIFactory.StretchToParent((RectTransform)pauseLayer.transform);

            Pause = gameObject.AddComponent<PauseMenu>();
            Pause.MainMenuScene = MainMenuScene;
            Pause.Build((RectTransform)pauseLayer.transform, Settings);

            settingsLayer.transform.SetAsLastSibling();
        }
    }
}
