using UnityEngine;
using UnityEngine.UI;
using Indigo.UI;

namespace Indigo.Game
{
    public class EndOfDayPanel : MonoBehaviour
    {
        private RectTransform _root;
        private CanvasGroup _cg;
        private Text _dayLabel;
        private Text _ratingLabel;
        private Text _servedLabel;
        private Text _lostLabel;
        private Text _earnedLabel;
        private Button _nextDayButton;

        [Header("Player Reset")]
        public GameObject playerObject; // Tvoj Player GameObject
        public Vector3 playerStartPositions = new Vector3(0f, 1f, 0f); // Početne koordinate (promijenićemo u Inspectoru)
        public Vector3 playerStartRotation = Vector3.zero; // Početna rotacija igrača

        void Start()
        {
            BuildPanel();
            if (DayManager.Instance != null) DayManager.Instance.OnDayEnded += Show;
        }

        void OnDestroy()
        {
            if (DayManager.Instance != null) DayManager.Instance.OnDayEnded -= Show;
        }

        void BuildPanel()
        {
            var canvas = FindHudParent();
            if (canvas == null)
            {
                Debug.LogWarning("EndOfDayPanel: HUD layer not found.");
                return;
            }

            var dim = UIFactory.Panel("EndOfDayDim", canvas, new Color(0f, 0f, 0f, 0.55f));
            UIFactory.StretchToParent((RectTransform)dim.transform);
            _root = (RectTransform)dim.transform;
            _cg = dim.AddComponent<CanvasGroup>();

            var panel = UIFactory.Panel("EndOfDayPanel", _root, UITheme.Panel, 18);
            var pRt = (RectTransform)panel.transform;
            pRt.anchorMin = new Vector2(0.5f, 0.5f);
            pRt.anchorMax = new Vector2(0.5f, 0.5f);
            pRt.pivot     = new Vector2(0.5f, 0.5f);
            pRt.sizeDelta = new Vector2(480, 460);
            UIFactory.AddDropShadow(pRt, new Vector2(0, -6), 0.45f, 20, 14f);
            UIFactory.AddStroke(pRt, UITheme.Stroke, 1.5f, 18);

            var accent = UIFactory.Panel("Accent", panel.transform, UITheme.Accent, 2);
            var aRt = (RectTransform)accent.transform;
            aRt.anchorMin = new Vector2(0, 1);
            aRt.anchorMax = new Vector2(1, 1);
            aRt.pivot     = new Vector2(0.5f, 1);
            aRt.sizeDelta = new Vector2(-40, 4);
            aRt.anchoredPosition = new Vector2(0, -24);

            var caption = UIFactory.Label("Caption", panel.transform, "END OF DAY", UITheme.FontSmall, UITheme.TextSecondary, TextAnchor.UpperCenter);
            var cRt = caption.rectTransform;
            cRt.anchorMin = new Vector2(0, 1);
            cRt.anchorMax = new Vector2(1, 1);
            cRt.pivot     = new Vector2(0.5f, 1);
            cRt.anchoredPosition = new Vector2(0, -36);
            cRt.sizeDelta = new Vector2(-40, 22);

            _dayLabel = UIFactory.Label("Day", panel.transform, "DAY 1", UITheme.FontTitle, UITheme.TextPrimary, TextAnchor.UpperCenter);
            _dayLabel.fontStyle = FontStyle.Bold;
            var dRt = _dayLabel.rectTransform;
            dRt.anchorMin = new Vector2(0, 1);
            dRt.anchorMax = new Vector2(1, 1);
            dRt.pivot     = new Vector2(0.5f, 1);
            dRt.anchoredPosition = new Vector2(0, -56);
            dRt.sizeDelta = new Vector2(-40, 70);

            _ratingLabel = UIFactory.Label("Rating", panel.transform, "A", UITheme.FontTitle, UITheme.Accent, TextAnchor.MiddleCenter);
            _ratingLabel.fontStyle = FontStyle.Bold;
            var rRt = _ratingLabel.rectTransform;
            rRt.anchorMin = new Vector2(0.5f, 0.5f);
            rRt.anchorMax = new Vector2(0.5f, 0.5f);
            rRt.pivot     = new Vector2(0.5f, 0.5f);
            rRt.anchoredPosition = new Vector2(0, 60);
            rRt.sizeDelta = new Vector2(200, 90);

            _servedLabel = AddStat(panel.transform, "Served", new Vector2(0, -20));
            _lostLabel   = AddStat(panel.transform, "Lost",   new Vector2(0, -56));
            _earnedLabel = AddStat(panel.transform, "Earned", new Vector2(0, -92));

            var btn = UIFactory.TextButton("NextDayButton", panel.transform, "Start Next Day", new Vector2(280, 56));
            var btRt = (RectTransform)btn.transform;
            btRt.anchorMin = new Vector2(0.5f, 0);
            btRt.anchorMax = new Vector2(0.5f, 0);
            btRt.pivot     = new Vector2(0.5f, 0);
            btRt.anchoredPosition = new Vector2(0, 24);
            btn.onClick.AddListener(OnNextDay);
            _nextDayButton = btn;

            _root.gameObject.SetActive(false);
        }

        Text AddStat(Transform parent, string title, Vector2 anchored)
        {
            var row = UIFactory.Panel("StatRow_" + title, parent, UITheme.PanelLight, 8);
            var rRt = (RectTransform)row.transform;
            rRt.anchorMin = new Vector2(0.5f, 0.5f);
            rRt.anchorMax = new Vector2(0.5f, 0.5f);
            rRt.pivot     = new Vector2(0.5f, 0.5f);
            rRt.anchoredPosition = anchored;
            rRt.sizeDelta = new Vector2(380, 30);

            var name = UIFactory.Label("Name", row.transform, title, UITheme.FontBody, UITheme.TextSecondary, TextAnchor.MiddleLeft);
            var nRt = name.rectTransform;
            nRt.anchorMin = Vector2.zero;
            nRt.anchorMax = Vector2.one;
            nRt.offsetMin = new Vector2(16, 0);
            nRt.offsetMax = new Vector2(-120, 0);

            var value = UIFactory.Label("Value", row.transform, "0", UITheme.FontBody, UITheme.TextPrimary, TextAnchor.MiddleRight);
            value.fontStyle = FontStyle.Bold;
            var vRt = value.rectTransform;
            vRt.anchorMin = Vector2.zero;
            vRt.anchorMax = Vector2.one;
            vRt.offsetMin = new Vector2(120, 0);
            vRt.offsetMax = new Vector2(-16, 0);
            return value;
        }

        Transform FindHudParent()
        {
            var gameUI = GameUI.Instance;
            if (gameUI == null) return null;
            return gameUI.transform.Find("GameUI_Canvas/HUD");
        }

        static Color RatingColor(string rating)
        {
            if (string.IsNullOrEmpty(rating)) return UITheme.Accent;
            switch (rating.Trim().ToUpperInvariant()[0])
            {
                case 'S':
                case 'A': return UITheme.Success;
                case 'B': return UITheme.Accent;
                case 'C': return UITheme.Warning;
                default:  return UITheme.Danger;
            }
        }

        void Show(DaySummary s)
        {
            if (_root == null) return;
            _root.gameObject.SetActive(true);
            _root.SetAsLastSibling();

            if (_dayLabel    != null) _dayLabel.text    = "DAY " + s.Day;
            if (_ratingLabel != null)
            {
                _ratingLabel.text  = s.Rating;
                _ratingLabel.color = RatingColor(s.Rating);
            }
            if (_servedLabel != null) _servedLabel.text = s.CustomersServed.ToString();
            if (_lostLabel   != null) _lostLabel.text   = s.CustomersLost.ToString();
            if (_earnedLabel != null) _earnedLabel.text = "$" + s.Earned;

            UIEvents.SetPaused(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

       void OnNextDay()
        {
            if (_root != null) _root.gameObject.SetActive(false);
            UIEvents.SetPaused(false);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            // --- NOVO: RESET POZICIJE IGRAČA ---
            if (playerObject != null)
            {
                // Ako koristiš CharacterController na igraču, moramo ga na blic ugasiti da nam dopusti teleportaciju
                var controller = playerObject.GetComponent<CharacterController>();
                if (controller != null) controller.enabled = false;

                playerObject.transform.position = playerStartPositions;
                playerObject.transform.rotation = Quaternion.Euler(playerStartRotation);

                if (controller != null) controller.enabled = true;
            }
            // ------------------------------------

            if (DayManager.Instance != null) DayManager.Instance.StartNextDay();
        }
    }
}
