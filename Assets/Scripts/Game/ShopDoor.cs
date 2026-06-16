using UnityEngine;
using Indigo.Game;
using Indigo.UI;

namespace Indigo.Environment
{
    [RequireComponent(typeof(Collider))]
    public class ShopDoor : MonoBehaviour
    {
        [Header("Postavke Vrata")]
        public Transform doorPivot;
        public float openAngle = 90f;
        public float openSpeed = 5f;

        private bool _playerNear;
        private bool _promptShown;
        private Quaternion _closedRotation;
        private Quaternion _openRotation;
        private bool _isOpen;

        void Awake()
        {
            if (doorPivot == null) doorPivot = transform;
            
            _closedRotation = doorPivot.localRotation;
            _openRotation = _closedRotation * Quaternion.Euler(0, openAngle, 0);
        }

        // NOVO: Prebacili smo povezivanje u Start() da osiguramo da je DayManager već budan!
        void Start()
        {
            if (DayManager.Instance != null)
            {
                DayManager.Instance.OnPhaseChanged += HandlePhaseChanged;
            }
        }

        void OnDestroy()
        {
            if (DayManager.Instance != null)
            {
                DayManager.Instance.OnPhaseChanged -= HandlePhaseChanged;
            }
            ClearPromptIfMine();
        }

        void Update()
        {
            Quaternion targetRotation = _isOpen ? _openRotation : _closedRotation;
            doorPivot.localRotation = Quaternion.Slerp(doorPivot.localRotation, targetRotation, Time.deltaTime * openSpeed);

            if (!_playerNear) return;

            if (DayManager.Instance != null && DayManager.Instance.Phase == DayManager.DayPhase.PreDay)
            {
                ShowPrompt("E", "Open Shop & Start Day");
                
                if (Input.GetKeyDown(KeyCode.E))
                {
                    OpenDoorAndStartDay();
                }
            }
            else
            {
                ClearPromptIfMine();
            }
        }

        void OpenDoorAndStartDay()
        {
            // Odmah forsiramo otvaranje ovih vrata, bez čekanja menadžera!
            _isOpen = true; 

            if (Indigo.Audio.SoundManager.Instance != null)
            {
                Indigo.Audio.SoundManager.Instance.Play(Indigo.Audio.SfxId.DoorOpen);
            }

            DayManager.Instance.StartNextDay();
            ClearPromptIfMine();
        }

        void HandlePhaseChanged(DayManager.DayPhase phase)
        {
            if (phase == DayManager.DayPhase.PreDay)
            {
                _isOpen = false; 
            }
            // Ovo osigurava da se i DRUGA vrata automatski otvore!
            else if (phase == DayManager.DayPhase.Active)
            {
                _isOpen = true;
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            _playerNear = true;
        }

        void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            _playerNear = false;
            ClearPromptIfMine();
        }

        void ShowPrompt(string key, string text)
        {
            if (_promptShown && UIEvents.CurrentPromptText == text) return;
            UIEvents.SetPrompt(key, text);
            _promptShown = true;
        }

        void ClearPromptIfMine()
        {
            if (_promptShown)
            {
                UIEvents.ClearPrompt();
                _promptShown = false;
            }
        }
    }
}