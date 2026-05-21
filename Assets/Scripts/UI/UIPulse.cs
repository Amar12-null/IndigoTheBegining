using UnityEngine;

namespace Indigo.UI
{
    public class UIPulse : MonoBehaviour
    {
        public CanvasGroup canvasGroup;
        public RectTransform scaleTarget;

        public bool  pulseAlpha = true;
        public float minAlpha = 0.78f;
        public float maxAlpha = 1f;

        public bool  pulseScale;
        public bool  scaleX = true;
        public bool  scaleY = true;
        public float minScale = 1f;
        public float maxScale = 1.04f;

        public float speed = 2.4f;
        public float phaseOffset;

        void Awake()
        {
            if (scaleTarget == null) scaleTarget = (RectTransform)transform;
            if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null && pulseAlpha) canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        void Update()
        {
            float t = (Mathf.Sin(Time.unscaledTime * speed + phaseOffset) + 1f) * 0.5f;

            if (pulseAlpha && canvasGroup != null)
                canvasGroup.alpha = Mathf.Lerp(minAlpha, maxAlpha, t);

            if (pulseScale)
            {
                float s = Mathf.Lerp(minScale, maxScale, t);
                scaleTarget.localScale = new Vector3(scaleX ? s : 1f, scaleY ? s : 1f, 1f);
            }
        }
    }
}
