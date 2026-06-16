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
            // Koristimo FindAnyObjectByType za Unity 6
            _shopPanel = FindAnyObjectByType<ShopPanel>();
        }

        void LateUpdate()
        {
            if (_isPlayerNear && _shopPanel != null)
            {
                if (Input.GetKeyDown(_shopPanel.toggleKey))
                {
                    _shopPanel.Toggle();
                    
                    // Opciono: Gasimo UI tekst kad se shop otvori (da ne smeta iza panela)
                    // Ako se shop otvori, gasi tekst. Ako se zatvori, ponovo ga pali.
                    if (_shopPanel.gameObject.activeInHierarchy)
                    {
                        Indigo.UI.UIEvents.SetPrompt("", "");
                    }
                    else
                    {
                        Indigo.UI.UIEvents.SetPrompt(_shopPanel.toggleKey.ToString(), "Open Shop");
                    }
                }
            }
        }

        // Obrisali smo OnGUI() potpuno!

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") || other.GetComponent<CharacterController>() != null)
            {
                _isPlayerNear = true;
                
                // --- NOVO: Pozivamo moderni prompt sistem ---
                if (_shopPanel != null)
                {
                    Indigo.UI.UIEvents.SetPrompt(_shopPanel.toggleKey.ToString(), "Open Shop");
                }
                
                Debug.Log("ShopTrigger: Igrač je ušao u zonu kase!");
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player") || other.GetComponent<CharacterController>() != null)
            {
                _isPlayerNear = false;
                
                // --- NOVO: Čistimo prompt sistem kada igrač ode ---
                Indigo.UI.UIEvents.SetPrompt("", "");
                
                Debug.Log("ShopTrigger: Igrač je izašao iz zone kase!");
                if (_shopPanel != null) _shopPanel.Close();
            }
        }
    }
}