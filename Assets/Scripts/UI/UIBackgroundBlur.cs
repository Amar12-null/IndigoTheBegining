using UnityEngine;
using UnityEngine.UI;

namespace Indigo.UI
{
    [RequireComponent(typeof(RawImage))]
    public class UIBackgroundBlur : MonoBehaviour
    {
        public Color tint = new Color(0.55f, 0.58f, 0.62f, 1f);
        public int   downscale = 10;

        RawImage _image;
        RenderTexture _rt;

        void Awake() { _image = GetComponent<RawImage>(); }

        public void Capture()
        {
            var cam = Camera.main;
            if (cam == null)
            {
                _image.texture = null;
                _image.color = new Color(0.08f, 0.09f, 0.11f, 1f);
                return;
            }

            int w = Mathf.Max(64, Screen.width / Mathf.Max(2, downscale));
            int h = Mathf.Max(64, Screen.height / Mathf.Max(2, downscale));
            if (_rt == null || _rt.width != w || _rt.height != h)
            {
                if (_rt != null) _rt.Release();
                _rt = new RenderTexture(w, h, 16) { filterMode = FilterMode.Bilinear };
            }

            var prev = cam.targetTexture;
            cam.targetTexture = _rt;
            cam.Render();
            cam.targetTexture = prev;

            _image.texture = _rt;
            _image.color   = tint;
        }

        void OnDestroy()
        {
            if (_rt != null) _rt.Release();
        }
    }
}
