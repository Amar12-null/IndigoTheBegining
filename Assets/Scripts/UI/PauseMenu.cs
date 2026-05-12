using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Indigo.UI
{
    public class PauseMenu : MonoBehaviour
    {
        public string MainMenuScene = "MainMenu";

        private GameObject     _root;
        private SettingsPanel  _settings;
        private bool           _wasCursorVisible;
        private CursorLockMode _wasLockMode;

        public bool IsOpen => _root != null && _root.activeSelf;

        public void Build(RectTransform parent, SettingsPanel settings)
        {
            _settings = settings;

            var overlay = UIFactory.Panel("PauseOverlay", parent, new Color(0, 0, 0, 0.55f));
            UIFactory.StretchToParent((RectTransform)overlay.transform);
            _root = overlay;

            var card = UIFactory.Panel("Card", overlay.transform, UITheme.Panel);
            var cardRt = (RectTransform)card.transform;
            cardRt.anchorMin = new Vector2(0.5f, 0.5f);
            cardRt.anchorMax = new Vector2(0.5f, 0.5f);
            cardRt.pivot     = new Vector2(0.5f, 0.5f);
            cardRt.sizeDelta = new Vector2(380, 420);

            var title = UIFactory.Label("Title", card.transform, "PAUSED", UITheme.FontHeader, UITheme.TextPrimary, TextAnchor.UpperLeft);
            title.fontStyle = FontStyle.Bold;
            var tRt = title.rectTransform;
            tRt.anchorMin = new Vector2(0, 1);
            tRt.anchorMax = new Vector2(1, 1);
            tRt.pivot     = new Vector2(0, 1);
            tRt.anchoredPosition = new Vector2(32, -28);
            tRt.sizeDelta = new Vector2(-64, 40);

            var accent = UIFactory.Panel("Accent", card.transform, UITheme.Accent);
            var aRt = (RectTransform)accent.transform;
            aRt.anchorMin = new Vector2(0, 1);
            aRt.anchorMax = new Vector2(0, 1);
            aRt.pivot     = new Vector2(0, 1);
            aRt.anchoredPosition = new Vector2(32, -76);
            aRt.sizeDelta = new Vector2(48, 3);

            AddButton(card.transform, "Resume",      -120, Resume);
            AddButton(card.transform, "Settings",    -184, OpenSettings);
            AddButton(card.transform, "Main Menu",   -248, GoToMainMenu);
            AddButton(card.transform, "Quit Game",   -312, Quit);

            _root.SetActive(false);
        }

        void AddButton(Transform parent, string label, float yOffset, UnityEngine.Events.UnityAction onClick)
        {
            var btn = UIFactory.TextButton(label + "Button", parent, label, new Vector2(280, 52));
            var rt = btn.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 1);
            rt.anchorMax = new Vector2(0.5f, 1);
            rt.pivot     = new Vector2(0.5f, 1);
            rt.anchoredPosition = new Vector2(0, yOffset);
            btn.onClick.AddListener(onClick);
        }

        public void TogglePause()
        {
            if (IsOpen) Resume();
            else        Open();
        }

        public void Open()
        {
            if (IsOpen) return;
            _wasCursorVisible = Cursor.visible;
            _wasLockMode      = Cursor.lockState;
            Cursor.visible    = true;
            Cursor.lockState  = CursorLockMode.None;
            Time.timeScale    = 0f;
            _root.SetActive(true);
            UIEvents.SetPaused(true);
        }

        public void Resume()
        {
            if (_settings != null && _settings.IsOpen) _settings.Close();
            _root.SetActive(false);
            Cursor.visible    = _wasCursorVisible;
            Cursor.lockState  = _wasLockMode;
            Time.timeScale    = 1f;
            UIEvents.SetPaused(false);
        }

        void OpenSettings()
        {
            _settings?.Open();
        }

        void GoToMainMenu()
        {
            Time.timeScale = 1f;
            UIEvents.SetPaused(false);
            Cursor.visible   = true;
            Cursor.lockState = CursorLockMode.None;
            if (Application.CanStreamedLevelBeLoaded(MainMenuScene))
                SceneManager.LoadScene(MainMenuScene);
            else
                Debug.LogWarning($"PauseMenu: scene '{MainMenuScene}' is not in Build Settings.");
        }

        void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (_settings != null && _settings.IsOpen) _settings.Close();
                else                                       TogglePause();
            }
        }
    }
}
