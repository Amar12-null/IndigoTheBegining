using UnityEngine;

namespace Indigo.Shop
{
    public class ShopTrigger : MonoBehaviour
    {
        [Tooltip("Referenca na ShopPanel skriptu koja se nalazi na GameUI objektu.")]
        private ShopPanel _shopPanel;
        private bool _isPlayerNear = false;

        void Start()
        {
            // POPRAVLJENO: Umjesto FindFirstObjectByType koristimo FindAnyObjectByType za Unity 6!
            _shopPanel = FindAnyObjectByType<ShopPanel>();
        }

        void LateUpdate()
        {
            if (_isPlayerNear && _shopPanel != null)
            {
                if (Input.GetKeyDown(_shopPanel.toggleKey))
                {
                    _shopPanel.Toggle();
                }
            }
        }

        void OnGUI()
        {
            if (_isPlayerNear && _shopPanel != null)
            {
                var stil = new GUIStyle(GUI.skin.label);
                stil.alignment = TextAnchor.MiddleCenter;
                stil.fontSize = 22;
                stil.fontStyle = FontStyle.Bold;
                stil.normal.textColor = Color.green;
                
                GUI.Box(new Rect(Screen.width / 2 - 175, Screen.height - 120, 350, 45), 
                    $"[STATION] Press [{_shopPanel.toggleKey}] to Open Shop", GUI.skin.box);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") || other.GetComponent<CharacterController>() != null)
            {
                _isPlayerNear = true;
                Debug.Log("ShopTrigger: Igrač je ušao u zonu kase!");
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player") || other.GetComponent<CharacterController>() != null)
            {
                _isPlayerNear = false;
                Debug.Log("ShopTrigger: Igrač je izašao iz zone kase!");
                if (_shopPanel != null) _shopPanel.Close();
            }
        }
    }
}