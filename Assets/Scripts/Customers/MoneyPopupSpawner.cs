using UnityEngine;
using Indigo.UI;

namespace Indigo.Customers
{
    public class MoneyPopupSpawner : MonoBehaviour
    {
        [Tooltip("Prefiks ispred iznosa popupa (npr. '+$', '+').")]
        public string positivePrefix = "+$";

        [Tooltip("Tekst kad gost ode ljut.")]
        public string angryText = "ANGRY!";

        void OnEnable()
        {
            MoneyManager.OnTipReceived += HandleTip;
            MoneyManager.OnAngryLeave  += HandleAngry;
        }

        void OnDisable()
        {
            MoneyManager.OnTipReceived -= HandleTip;
            MoneyManager.OnAngryLeave  -= HandleAngry;
        }

        void HandleTip(int amount, Vector3 worldPos, Satisfaction sat)
        {
            MoneyPopup.Spawn(worldPos, positivePrefix + amount, sat.Color());
        }

        void HandleAngry(Vector3 worldPos)
        {
            MoneyPopup.Spawn(worldPos, angryText, Satisfaction.Angry.Color());
        }
    }
}