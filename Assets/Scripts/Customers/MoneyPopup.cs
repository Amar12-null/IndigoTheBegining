using UnityEngine;
using UnityEngine.UI;
using Indigo.UI;

namespace Indigo.Customers
{
    public class MoneyPopup : MonoBehaviour
    {
        public float lifetime = 1.4f;
        public float riseSpeed = 1.2f;

        private Text _label;
        private CanvasGroup _cg;
        private Camera _cam;
        private float _t;

        public static MoneyPopup Spawn(Vector3 worldPos, string text, Color color)
        {
            var go = new GameObject("MoneyPopup",
                typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster), typeof(CanvasGroup));
            go.transform.position = worldPos;

            var canvas = go.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.sortingOrder = 60;

            var rt = (RectTransform)go.transform;
            rt.sizeDelta = new Vector2(180, 60);
            rt.localScale = Vector3.one * 0.006f;

            var popup = go.AddComponent<MoneyPopup>();
            popup._cg = go.GetComponent<CanvasGroup>();

            var label = UIFactory.Label("Amount", go.transform, text, UITheme.FontHeader, color, TextAnchor.MiddleCenter);
            label.fontStyle = FontStyle.Bold;
            var lRt = label.rectTransform;
            UIFactory.StretchToParent(lRt);

            popup._label = label;
            popup._cam = Camera.main;
            return popup;
        }

        void Update()
        {
            _t += Time.deltaTime;
            float k = Mathf.Clamp01(_t / lifetime);

            transform.position += Vector3.up * riseSpeed * Time.deltaTime;

            if (_cg != null)
                _cg.alpha = 1f - k;

            float scale = Mathf.Lerp(0.006f, 0.009f, k);
            transform.localScale = Vector3.one * scale;

            if (_cam == null) _cam = Camera.main;
            if (_cam != null)
            {
                var dir = transform.position - _cam.transform.position;
                dir.y = 0f;
                if (dir.sqrMagnitude > 0.0001f)
                    transform.rotation = Quaternion.LookRotation(dir);
            }

            if (_t >= lifetime) Destroy(gameObject);
        }
    }

    public class MoneyPopupSpawner : MonoBehaviour
    {
        [Tooltip("Prefiks ispred iznosa popupa (npr. '+$', '+').")]
        public string positivePrefix = "+$";

        [Tooltip("Tekst kad gost ode ljut.")]
        public string angryText = "ANGRY!";

        void OnEnable()
        {
            MoneyManager.OnTipReceived += HandleTip;
            MoneyManager.OnAngryLeave  += HandleAngry;
        }

        void OnDisable()
        {
            MoneyManager.OnTipReceived -= HandleTip;
            MoneyManager.OnAngryLeave  -= HandleAngry;
        }

        void HandleTip(int amount, Vector3 worldPos, Satisfaction sat)
        {
            MoneyPopup.Spawn(worldPos, positivePrefix + amount, sat.Color());
        }

        void HandleAngry(Vector3 worldPos)
        {
            MoneyPopup.Spawn(worldPos, angryText, Satisfaction.Angry.Color());
        }
    }
}
