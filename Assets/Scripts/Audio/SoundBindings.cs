using UnityEngine;
using Indigo.Customers;
using Indigo.Progression;
using Indigo.Game;
using Indigo.Shop;

namespace Indigo.Audio
{
    public class SoundBindings : MonoBehaviour
    {
        void OnEnable()
        {
            MoneyManager.OnTipReceived += HandleTip;
            MoneyManager.OnAngryLeave += HandleAngry;
            PlayerProgression.OnLevelUp += HandleLevelUp;
            Upgrades.OnPurchased += HandlePurchase;
            if (DayManager.Instance != null)
            {
                DayManager.Instance.OnDayStarted += HandleDayStarted;
                DayManager.Instance.OnDayEnded += HandleDayEnded;
            }
        }

        void Start()
        {
            if (DayManager.Instance != null)
            {
                DayManager.Instance.OnDayStarted -= HandleDayStarted;
                DayManager.Instance.OnDayStarted += HandleDayStarted;
                DayManager.Instance.OnDayEnded -= HandleDayEnded;
                DayManager.Instance.OnDayEnded += HandleDayEnded;
            }
        }

        void OnDisable()
        {
            MoneyManager.OnTipReceived -= HandleTip;
            MoneyManager.OnAngryLeave -= HandleAngry;
            PlayerProgression.OnLevelUp -= HandleLevelUp;
            Upgrades.OnPurchased -= HandlePurchase;
            if (DayManager.Instance != null)
            {
                DayManager.Instance.OnDayStarted -= HandleDayStarted;
                DayManager.Instance.OnDayEnded -= HandleDayEnded;
            }
        }

        void HandleTip(int _, Vector3 pos, Satisfaction __)
        {
            if (SoundManager.Instance == null) return;
            SoundManager.Instance.Play(SfxId.DeliverySuccess);
            SoundManager.Instance.Play(SfxId.TipCoin);
        }

        void HandleAngry(Vector3 pos)
        {
            if (SoundManager.Instance != null) SoundManager.Instance.Play(SfxId.CustomerAngry);
        }

        void HandleLevelUp(int _)
        {
            if (SoundManager.Instance != null) SoundManager.Instance.Play(SfxId.LevelUp);
        }

        void HandlePurchase(UpgradeId _)
        {
            if (SoundManager.Instance != null) SoundManager.Instance.Play(SfxId.Purchase);
        }

        void HandleDayStarted(int _)
        {
            if (SoundManager.Instance != null) SoundManager.Instance.Play(SfxId.DayStart);
        }

        void HandleDayEnded(DaySummary _)
        {
            if (SoundManager.Instance != null) SoundManager.Instance.Play(SfxId.DayEnd);
        }
    }
}
