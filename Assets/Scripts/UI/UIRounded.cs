using System.Collections.Generic;
using UnityEngine;

namespace Indigo.UI
{
    public static class UIRounded
    {
        static readonly Dictionary<int, Sprite> _cache = new Dictionary<int, Sprite>();

        public static Sprite Sprite(int radius = 16)
        {
            if (_cache.TryGetValue(radius, out var s) && s != null) return s;

            int size = radius * 2 + 4;
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false)
            {
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Bilinear
            };

            var pixels = new Color32[size * size];
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx = x < radius ? radius - x : (x > size - 1 - radius ? x - (size - 1 - radius) : 0f);
                    float dy = y < radius ? radius - y : (y > size - 1 - radius ? y - (size - 1 - radius) : 0f);
                    float dist = Mathf.Sqrt(dx * dx + dy * dy);
                    float a = Mathf.Clamp01(radius - dist + 0.5f);
                    pixels[y * size + x] = new Color32(255, 255, 255, (byte)(a * 255));
                }
            }
            tex.SetPixels32(pixels);
            tex.Apply(false, true);

            s = UnityEngine.Sprite.Create(
                tex,
                new Rect(0, 0, size, size),
                new Vector2(0.5f, 0.5f),
                100f,
                0,
                SpriteMeshType.FullRect,
                new Vector4(radius, radius, radius, radius));

            _cache[radius] = s;
            return s;
        }
    }
}
