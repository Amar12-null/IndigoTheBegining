using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Indigo.UI
{
    public class UIHoverScale : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public RectTransform target;
        public float hoverScale = 1.04f;
        public float duration = 0.1f;

        Vector3 _base = Vector3.one;
        Coroutine _co;

        void Awake()
        {
            if (target == null) target = (RectTransform)transform;
            _base = target.localScale;
        }

        public void OnPointerEnter(PointerEventData e) { Animate(_base * hoverScale); }
        public void OnPointerExit(PointerEventData e)  { Animate(_base); }

        void Animate(Vector3 to)
        {
            if (!isActiveAndEnabled) { target.localScale = to; return; }
            if (_co != null) StopCoroutine(_co);
            _co = StartCoroutine(Run(to));
        }

        IEnumerator Run(Vector3 to)
        {
            Vector3 from = target.localScale;
            float t = 0f;
            while (t < duration)
            {
                t += Time.unscaledDeltaTime;
                float k = Mathf.Clamp01(t / duration);
                target.localScale = Vector3.LerpUnclamped(from, to, k);
                yield return null;
            }
            target.localScale = to;
        }
    }
}
