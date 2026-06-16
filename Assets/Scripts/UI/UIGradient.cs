using UnityEngine;
using UnityEngine.UI;

namespace Indigo.UI
{
    /// <summary>
    /// Vertical two-colour gradient applied to any uGUI Graphic (Image/Text).
    /// Multiplies the existing vertex colours so it composes with Button tinting.
    /// </summary>
    [AddComponentMenu("UI/Effects/Indigo Gradient")]
    public class UIGradient : BaseMeshEffect
    {
        public Color top    = Color.white;
        public Color bottom = new Color(0.85f, 0.85f, 0.85f, 1f);

        public override void ModifyMesh(VertexHelper vh)
        {
            if (!IsActive() || vh.currentVertCount == 0) return;

            float min = float.MaxValue, max = float.MinValue;
            var v = new UIVertex();
            for (int i = 0; i < vh.currentVertCount; i++)
            {
                vh.PopulateUIVertex(ref v, i);
                if (v.position.y < min) min = v.position.y;
                if (v.position.y > max) max = v.position.y;
            }

            float range = Mathf.Max(0.0001f, max - min);
            for (int i = 0; i < vh.currentVertCount; i++)
            {
                vh.PopulateUIVertex(ref v, i);
                float t = (v.position.y - min) / range;
                v.color *= Color.Lerp(bottom, top, t);
                vh.SetUIVertex(v, i);
            }
        }

        public static UIGradient Apply(Graphic graphic, Color top, Color bottom)
        {
            var g = graphic.gameObject.GetComponent<UIGradient>();
            if (g == null) g = graphic.gameObject.AddComponent<UIGradient>();
            g.top = top;
            g.bottom = bottom;
            graphic.SetVerticesDirty();
            return g;
        }
    }
}
