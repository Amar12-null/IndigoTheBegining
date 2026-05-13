using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Indigo.UI
{
    public class PauseMenu : MonoBehaviour
    {
        public string MainMenuScene = "MainMenu";

        GameObject       _root;
        CanvasGroup      _cg;
        RectTransform    _cardRt;
        UIBackgroundBlur _blur;
        SettingsPanel    _settings;
        Coroutine        _animCo;
        bool             _open, _closing;
        bool             _wasCursorVisible;
        CursorLockMode   _wasLockMode;

        public bool IsOpen => _open && !_closing;

        public void Build(RectTransform parent, SettingsPanel settings)
        {
            _settings = settings;

            var root = new GameObject("PauseOverlay", typeof(RectTransform), typeof(CanvasGroup));
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
            _cardRt.sizeDelta = new Vector2(380, 420);
            UIFactory.AddDropShadow(_cardRt, new Vector2(0, -8), 0.45f, 24, 18f);

            var title = UIFactory.Label("Title", card.transform, "PAUSED", UITheme.FontHeader, UITheme.TextPrimary, TextAnchor.UpperLeft);
            title.fontStyle = FontStyle.Bold;
            var tRt = title.rectTransform;
            tRt.anchorMin = new Vector2(0, 1);
            tRt.anchorMax = new Vector2(1, 1);
            tRt.pivot     = new Vector2(0, 1);
            tRt.anchoredPosition = new Vector2(32, -28);
            tRt.sizeDelta = new Vector2(-64, 40);

            var accent = UIFactory.Panel("Accent", card.transform, UITheme.Accent, 2);
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

            _cg.alpha = 0f;
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
            if (_open) return;
            _open = true;
            _closing = false;
            _wasCursorVisible = Cursor.visible;
            _wasLockMode = Cursor.lockState;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            UIEvents.SetPaused(true);

            _cg.alpha = 0f;
            _root.SetActive(true);
            if (_blur != null) _blur.Capture();
            Time.timeScale = 0f;

            if (_animCo != null) StopCoroutine(_animCo);
            _animCo = StartCoroutine(UIAnimator.FadeScaleIn(_cg, _cardRt, 0.18f));
        }

        public void Resume()
        {
            if (!_open || _closing) return;
            if (_settings != null && _settings.IsOpen) _settings.Close();
            _closing = true;
            if (_animCo != null) StopCoroutine(_animCo);
            _animCo = StartCoroutine(CloseRoutine());
        }

        IEnumerator CloseRoutine()
        {
            yield return UIAnimator.FadeScaleOut(_cg, _cardRt, 0.14f);
            _root.SetActive(false);
            Cursor.visible = _wasCursorVisible;
            Cursor.lockState = _wasLockMode;
            Time.timeScale = 1f;
            UIEvents.SetPaused(false);
            _open = false;
            _closing = false;
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
