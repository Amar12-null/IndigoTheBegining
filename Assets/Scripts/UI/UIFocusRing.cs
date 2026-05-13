using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Indigo.UI
{
    public class UIFocusRing : MonoBehaviour, ISelectHandler, IDeselectHandler
    {
        public Image ring;
        public float duration = 0.12f;
        public float maxAlpha = 0.55f;

        Coroutine _co;

        public void OnSelect(BaseEventData e)   { Fade(maxAlpha); }
        public void OnDeselect(BaseEventData e) { Fade(0f); }

        void Fade(float target)
        {
            if (ring == null) return;
            if (!isActiveAndEnabled)
            {
                var c0 = ring.color; c0.a = target; ring.color = c0;
                return;
            }
            if (_co != null) StopCoroutine(_co);
            _co = StartCoroutine(Run(target));
        }

        IEnumerator Run(float target)
        {
            float start = ring.color.a;
            float t = 0f;
            while (t < duration)
            {
                t += Time.unscaledDeltaTime;
                float k = Mathf.Clamp01(t / duration);
                var c = ring.color;
                c.a = Mathf.Lerp(start, target, k);
                ring.color = c;
                yield return null;
            }
            var cf = ring.color;
            cf.a = target;
            ring.color = cf;
        }
    }
}
