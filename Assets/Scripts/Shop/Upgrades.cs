using System;
using System.Collections.Generic;

namespace Indigo.Shop
{
    public enum UpgradeId
    {
        TipJar,
        ComfyChairs,
        ReputationSign,
        MarketingPosters,
    }

    public class UpgradeDef
    {
        public UpgradeId Id;
        public string Name;
        public string Description;
        public int Price;
        public int LevelRequired;
    }

    public static class Upgrades
    {
        public static readonly UpgradeDef[] Catalog =
        {
            new UpgradeDef { Id = UpgradeId.TipJar,           Name = "Tip Jar",            Description = "+25% napojnice na svaku narudžbu", Price = 25,  LevelRequired = 1 },
            new UpgradeDef { Id = UpgradeId.ComfyChairs,      Name = "Comfy Chairs",       Description = "+10s strpljenja kod novih gostiju", Price = 50,  LevelRequired = 2 },
            new UpgradeDef { Id = UpgradeId.ReputationSign,   Name = "Reputation Sign",    Description = "Gosti su sretni duže (lakše dobiti Happy tier)", Price = 75,  LevelRequired = 3 },
            new UpgradeDef { Id = UpgradeId.MarketingPosters, Name = "Marketing Posters",  Description = "+1 maksimalan broj istovremenih gostiju", Price = 120, LevelRequired = 5 },
        };

        private static readonly HashSet<UpgradeId> _owned = new HashSet<UpgradeId>();

        public static event Action<UpgradeId> OnPurchased;

        public static bool IsOwned(UpgradeId id) => _owned.Contains(id);

        public static UpgradeDef Get(UpgradeId id)
        {
            for (int i = 0; i < Catalog.Length; i++)
                if (Catalog[i].Id == id) return Catalog[i];
            return null;
        }

        public static bool CanAfford(UpgradeDef def) =>
            def != null
            && !IsOwned(def.Id)
            && Indigo.Progression.PlayerProgression.Level >= def.LevelRequired
            && Indigo.Customers.MoneyManager.Balance >= def.Price;

        public static bool TryPurchase(UpgradeId id)
        {
            var def = Get(id);
            if (def == null || IsOwned(id)) return false;
            if (Indigo.Progression.PlayerProgression.Level < def.LevelRequired) return false;
            if (!Indigo.Customers.MoneyManager.TrySpend(def.Price)) return false;
            _owned.Add(id);
            OnPurchased?.Invoke(id);
            return true;
        }

        public static float TipMultiplier()
        {
            return IsOwned(UpgradeId.TipJar) ? 1.25f : 1f;
        }

        public static float PatienceBonusSeconds()
        {
            return IsOwned(UpgradeId.ComfyChairs) ? 10f : 0f;
        }

        public static float HappyThresholdAdjust()
        {
            return IsOwned(UpgradeId.ReputationSign) ? -0.15f : 0f;
        }

        public static int ExtraConcurrentCustomers()
        {
            return IsOwned(UpgradeId.MarketingPosters) ? 1 : 0;
        }

        public static void ResetAll()
        {
            _owned.Clear();
        }
    }
}
