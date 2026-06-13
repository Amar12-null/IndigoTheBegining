using UnityEngine;
using UnityEngine.UI;

namespace Indigo.UI
{
    public class HelpHUD : MonoBehaviour
    {
        [TextArea]
        public string controlsText = "E Interact   ·   Q Drop   ·   B Shop   ·   ESC Pause";

        void Start()
        {
            BuildPanel();
        }

        void BuildPanel()
        {
            var canvas = FindHudParent();
            if (canvas == null)
            {
                Debug.LogWarning("HelpHUD: HUD layer not found.");
                return;
            }

            var go = UIFactory.Panel("HelpHud", canvas, new Color(0, 0, 0, 0.35f), 8);
            var rt = (RectTransform)go.transform;
            rt.anchorMin = new Vector2(0.5f, 0f);
            rt.anchorMax = new Vector2(0.5f, 0f);
            rt.pivot     = new Vector2(0.5f, 0f);
            rt.anchoredPosition = new Vector2(0, 24);
            rt.sizeDelta = new Vector2(540, 30);

            var label = UIFactory.Label("Text", go.transform, controlsText, UITheme.FontSmall, UITheme.TextSecondary, TextAnchor.MiddleCenter);
            var lRt = label.rectTransform;
            UIFactory.StretchToParent(lRt);
        }

        Transform FindHudParent()
        {
            var gameUI = GameUI.Instance;
            if (gameUI == null) return null;
            return gameUI.transform.Find("GameUI_Canvas/HUD");
        }
    }
}
