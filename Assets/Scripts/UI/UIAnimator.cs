using System.Collections;
using UnityEngine;

namespace Indigo.UI
{
    public static class UIAnimator
    {
        public static IEnumerator FadeScaleIn(CanvasGroup cg, RectTransform rt, float duration = 0.18f, float fromScale = 0.95f)
        {
            cg.alpha = 0f;
            rt.localScale = Vector3.one * fromScale;
            float t = 0f;
            while (t < duration)
            {
                t += Time.unscaledDeltaTime;
                float k = Mathf.Clamp01(t / duration);
                float ease = 1f - (1f - k) * (1f - k);
                cg.alpha = ease;
                rt.localScale = Vector3.LerpUnclamped(Vector3.one * fromScale, Vector3.one, ease);
                yield return null;
            }
            cg.alpha = 1f;
            rt.localScale = Vector3.one;
        }

        public static IEnumerator FadeScaleOut(CanvasGroup cg, RectTransform rt, float duration = 0.14f, float toScale = 0.96f)
        {
            float startAlpha = cg.alpha;
            Vector3 startScale = rt.localScale;
            Vector3 endScale = Vector3.one * toScale;
            float t = 0f;
            while (t < duration)
            {
                t += Time.unscaledDeltaTime;
                float k = Mathf.Clamp01(t / duration);
                cg.alpha = Mathf.Lerp(startAlpha, 0f, k);
                rt.localScale = Vector3.LerpUnclamped(startScale, endScale, k);
                yield return null;
            }
            cg.alpha = 0f;
            rt.localScale = endScale;
        }
    }
}
