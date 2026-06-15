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

            // --- POPRAVLJENO: Interior 3 je sada ispravno vezan za svoj 3D objekat ---
            if (interior3 != null)
            {
                bool shouldBeActive = Upgrades.IsOwned(UpgradeId.Interior3);
                if (interior3.activeSelf != shouldBeActive)
                {
                    interior3.SetActive(shouldBeActive); // POPRAVLJENO: Aktivira interior3, a ne interior2!
                    triggerTableRefresh = true;
                }
            }

            // --- POPRAVLJENO: Sigurna provjera za dekoracije ---
            if (dekoracije != null)
            {
                bool shouldBeActive = Upgrades.IsOwned(UpgradeId.Decorations);
                if (dekoracije.activeSelf != shouldBeActive)
                {
                    dekoracije.SetActive(shouldBeActive);
                }
            }

            // --- POPRAVLJENO: Bašta sada ispravno javlja menadžeru stolova da osvježi goste ---
            if (basta != null)
            {
                bool shouldBeActive = Upgrades.IsOwned(UpgradeId.Garden);
                if (basta.activeSelf != shouldBeActive)
                {
                    basta.SetActive(shouldBeActive);
                    triggerTableRefresh = true; // NOVO: Bašta sada takođe dodaje nove stolove u igru!
                }
            }

            // Ako se otključala bilo koja prostorija ili bašta, osvježi stolove za goste
            if (triggerTableRefresh && TableManager.Instance != null)
            {
                TableManager.Instance.RefreshTables();
            }
        }
    }
}