using UnityEngine;
using UnityEngine.UI;
using Indigo.UI;

namespace Indigo.Machines
{
    public class MachineProgressWorldUI : MonoBehaviour
    {
        private Image _fillImage;
        private CanvasGroup _cg;
        private Camera _cam;
        private RectTransform _rectTransform;

        public static MachineProgressWorldUI Create(Transform parent, Vector3 localOffset)
        {
            var go = new GameObject("MachineProgressUI",
                typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(CanvasGroup));
            
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localOffset;

            var canvas = go.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.sortingOrder = 40;

            var scaler = go.GetComponent<CanvasScaler>();
            scaler.dynamicPixelsPerUnit = 12f;
            scaler.referencePixelsPerUnit = 100f;

            var rt = (RectTransform)go.transform;
            rt.sizeDelta = new Vector2(160, 30);
            rt.localScale = Vector3.one * 0.005f; // Smanjujemo za WorldSpace

            var ui = go.AddComponent<MachineProgressWorldUI>();
            ui._cg = go.GetComponent<CanvasGroup>();
            ui._rectTransform = rt;
            ui._cam = Camera.main;

            // Pozadina trake
            var bg = UIFactory.Panel("Background", go.transform, UITheme.Background, 8);
            UIFactory.StretchToParent((RectTransform)bg.transform);

            // Unutrašnja traka (Track)
            var track = UIFactory.Panel("Track", bg.transform, UITheme.PanelLight, 4);
            var tRt = (RectTransform)track.transform;
            tRt.anchorMin = new Vector2(0f, 0f);
            tRt.anchorMax = new Vector2(1f, 1f);
            tRt.pivot = new Vector2(0.5f, 0.5f);
            tRt.offsetMin = new Vector2(6, 6);
            tRt.offsetMax = new Vector2(-6, -6);

            // Punjenje (Fill)
            var fill = UIFactory.Panel("Fill", track.transform, UITheme.Accent, 4);
            var fRt = (RectTransform)fill.transform;
            fRt.anchorMin = new Vector2(0, 0);
            fRt.anchorMax = new Vector2(0, 1);
            fRt.pivot = new Vector2(0, 0.5f);
            fRt.anchoredPosition = Vector2.zero;
            fRt.sizeDelta = new Vector2(148f, 0); // Puna širina unutar tracka

            ui._fillImage = fill.GetComponent<Image>();
            UIGradient.Apply(ui._fillImage, Color.white, new Color(0.78f, 0.78f, 0.82f, 1f));
            
            // Na početku sakrijemo UI jer mašina ne radi ništa
            ui._cg.alpha = 0f;

            return ui;
        }

        // Glavna funkcija koju će tvoje mašine pozivati
        public void UpdateProgress(float progressFraction, bool isDone = false)
        {
            if (_cg == null || _fillImage == null) return;

            // Ako mašina ne radi i progres je 0, sakrij UI
            if (progressFraction <= 0f)
            {
                _cg.alpha = 0f;
                return;
            }

            // Prikaži UI kada mašina radi
            _cg.alpha = 1f;

            // Ažuriraj širinu trake (148f je puna širina)
            var rect = (RectTransform)_fillImage.transform;
            rect.sizeDelta = new Vector2(148f * Mathf.Clamp01(progressFraction), 0f);

            // Ako je gotovo, promijeni boju u zelenu (kao kod narudžbi)
            if (isDone)
            {
                _fillImage.color = new Color(0.40f, 0.85f, 0.55f, 1f); // Prelijepa zelena
            }
            else
            {
                _fillImage.color = UITheme.Accent; // Standardna plava/brendirana boja iz teme
            }
        }

        void LateUpdate()
        {
            // Billboard efekat — drži traku okrenutu prema kameri (samo Y osa, kao kod OrderBubble-a)
            if (_cam == null) _cam = Camera.main;
            if (_cam != null)
            {
                var dir = transform.position - _cam.transform.position;
                dir.y = 0f;
                if (dir.sqrMagnitude > 0.0001f)
                    transform.rotation = Quaternion.LookRotation(dir);
            }
        }
    }
}