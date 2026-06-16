using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;

namespace Indigo.UI
{
    /// <summary>
    /// Freezes the live scene behind a menu and blurs it in real time.
    ///
    /// The camera is rendered once into an off-screen RenderTexture, then run
    /// through a pyramidal "dual-filter" (Kawase-style) blur: the image is
    /// progressively half-sized down a chain of RenderTextures and then scaled
    /// back up. Because each bilinear blit averages a 2×2 neighbourhood, a few
    /// cheap passes approximate a very wide Gaussian without any custom shader —
    /// far smoother than a single hard downscale, and effectively free since it
    /// runs only on the frame the menu opens.
    /// </summary>
    [RequireComponent(typeof(RawImage))]
    public class UIBackgroundBlur : MonoBehaviour
    {
        [Tooltip("Colour multiplied over the captured frame (slight desaturated dim).")]
        public Color tint = new Color(0.55f, 0.58f, 0.62f, 1f);

        [Tooltip("Initial resolution divisor for the captured frame.")]
        [Range(1, 4)] public int startDownsample = 2;

        [Tooltip("Number of down/up sample passes — higher = softer, wider blur.")]
        [Range(1, 6)] public int iterations = 4;

        RawImage _image;
        RenderTexture _rt;

        void Awake() { _image = GetComponent<RawImage>(); }

        public void Capture()
        {
            var cam = Camera.main;
            if (cam == null)
            {
                // No camera to sample — fall back to a flat themed panel.
                _image.texture = null;
                _image.color = UITheme.BackdropBottom;
                return;
            }

            int w = Mathf.Max(2, Screen.width  / Mathf.Max(1, startDownsample));
            int h = Mathf.Max(2, Screen.height / Mathf.Max(1, startDownsample));

            // Persistent base target the RawImage samples from.
            if (_rt == null || _rt.width != w || _rt.height != h)
            {
                if (_rt != null) _rt.Release();
                _rt = new RenderTexture(w, h, 24) { filterMode = FilterMode.Bilinear };
            }

            // 1) Snapshot the live scene into the base RT. Scriptable Render
            //    Pipelines (URP/HDRP) ignore the legacy Camera.Render(), so use
            //    Unity 6's render-request API when the active pipeline supports
            //    it and fall back to the built-in path otherwise.
            var request = new RenderPipeline.StandardRequest { destination = _rt };
            if (RenderPipeline.SupportsRenderRequest(cam, request))
            {
                cam.SubmitRenderRequest(request);
            }
            else
            {
                var prev = cam.targetTexture;
                cam.targetTexture = _rt;
                cam.Render();
                cam.targetTexture = prev;
            }

            // 2) Pyramidal blur: down the chain, then back up. Each pass is a
            //    bilinear blit, so the kernel widens geometrically per iteration.
            int n = Mathf.Clamp(iterations, 1, 6);
            var chain = new RenderTexture[n];
            int cw = w, ch = h;
            RenderTexture current = _rt;

            for (int i = 0; i < n; i++)
            {
                cw = Mathf.Max(2, cw / 2);
                ch = Mathf.Max(2, ch / 2);
                chain[i] = RenderTexture.GetTemporary(cw, ch, 0);
                chain[i].filterMode = FilterMode.Bilinear;
                Graphics.Blit(current, chain[i]);
                current = chain[i];
            }
            for (int i = n - 2; i >= 0; i--)
            {
                Graphics.Blit(current, chain[i]);
                current = chain[i];
            }

            // 3) Composite the softened result back into the base RT.
            Graphics.Blit(current, _rt);

            for (int i = 0; i < n; i++)
                if (chain[i] != null) RenderTexture.ReleaseTemporary(chain[i]);

            _image.texture = _rt;
            _image.color   = tint;
        }

        void OnDestroy()
        {
            if (_rt != null) _rt.Release();
        }
    }
}
