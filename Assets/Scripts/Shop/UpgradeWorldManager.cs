using UnityEngine;
using Indigo.Customers;

namespace Indigo.Shop
{
    public class UpgradeWorldManager : MonoBehaviour
    {
        [Header("Fizički objekti iz stare skripte")]
        public GameObject interior2;
        public GameObject interior3;
        public GameObject basta;
        public GameObject dekoracije;

        void OnEnable()
        {
            Upgrades.OnPurchased += HandleUpgradePurchased;
        }

        void OnDisable()
        {
            Upgrades.OnPurchased -= HandleUpgradePurchased;
        }

        void Start()
        {
            // Čim igra počne, automatski provjeravamo šta je kupljeno, ostalo gasimo
            RefreshWorldObjects();
        }

        private void HandleUpgradePurchased(UpgradeId id)
        {
            RefreshWorldObjects();
        }

        private void RefreshWorldObjects()
        {
            bool triggerTableRefresh = false;

            // Interior 2
            if (interior2 != null)
            {
                bool shouldBeActive = Upgrades.IsOwned(UpgradeId.Interior2);
                if (interior2.activeSelf != shouldBeActive)
                {
                    interior2.SetActive(shouldBeActive);
                    triggerTableRefresh = true;
                }
            }

            // Interior 3
            if (interior3 != null)
            {
                bool shouldBeActive = Upgrades.IsOwned(UpgradeId.Interior3);
                if (interior3.activeSelf != shouldBeActive)
                {
                    interior3.SetActive(shouldBeActive);
                    triggerTableRefresh = true;
                }
            }

            // Dekoracije
            if (dekoracije != null)
            {
                dekoracije.SetActive(Upgrades.IsOwned(UpgradeId.Decorations));
            }

            // Bašta
            if (basta != null)
            {
                bool shouldBeActive = Upgrades.IsOwned(UpgradeId.Garden);
                if (basta.activeSelf != shouldBeActive)
                {
                    basta.SetActive(shouldBeActive);
                    triggerTableRefresh = true;
                }
            }

            // Ako se otključala prostorija, osvježi stolove za goste
            if (triggerTableRefresh && TableManager.Instance != null)
            {
                TableManager.Instance.RefreshTables();
            }
        }
    }
}