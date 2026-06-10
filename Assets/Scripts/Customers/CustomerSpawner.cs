using System.Collections.Generic;
using UnityEngine;

namespace Indigo.Customers
{
    public class CustomerSpawner : MonoBehaviour
    {
        [Header("Prefab & Points")]
        public Customer customerPrefab;
        public Transform spawnPoint;
        public Transform exitPoint;

        [Header("Timing")]
        public float firstSpawnDelay = 3f;
        public float minSpawnInterval = 8f;
        public float maxSpawnInterval = 16f;

        [Header("Limits")]
        public int maxConcurrentCustomers = 4;
        public bool requireFreeTable = true;

        private float _nextSpawnAt;
        private readonly List<Customer> _active = new List<Customer>();

        void Start()
        {
            _nextSpawnAt = Time.time + firstSpawnDelay;
        }

        void Update()
        {
            if (Indigo.UI.UIEvents.IsPaused) return;

            PruneDead();
            if (Time.time < _nextSpawnAt) return;
            if (!IsDayAllowingSpawn()) { ScheduleNext(); return; }
            int effectiveMax = maxConcurrentCustomers + Indigo.Shop.Upgrades.ExtraConcurrentCustomers();
            if (_active.Count >= effectiveMax) { ScheduleNext(); return; }
            if (requireFreeTable && TableManager.Instance != null && !TableManager.Instance.HasFreeTable())
            {
                ScheduleNext();
                return;
            }
            SpawnOne();
            ScheduleNext();
        }

        bool IsDayAllowingSpawn()
        {
            var dm = Indigo.Game.DayManager.Instance;
            if (dm == null) return true;
            return dm.IsSpawningAllowed();
        }

        void SpawnOne()
        {
            if (customerPrefab == null || spawnPoint == null)
            {
                Debug.LogWarning("CustomerSpawner: prefab ili spawnPoint nije postavljen.");
                return;
            }
            var c = Instantiate(customerPrefab, spawnPoint.position, spawnPoint.rotation);
            c.Init(exitPoint);
            _active.Add(c);
        }

        void ScheduleNext()
        {
            _nextSpawnAt = Time.time + Random.Range(minSpawnInterval, maxSpawnInterval);
        }

        void PruneDead()
        {
            for (int i = _active.Count - 1; i >= 0; i--)
                if (_active[i] == null) _active.RemoveAt(i);
        }

        void OnDrawGizmos()
        {
            if (spawnPoint != null)
            {
                Gizmos.color = new Color(0.4f, 0.9f, 0.6f, 0.9f);
                Gizmos.DrawSphere(spawnPoint.position, 0.2f);
                Gizmos.DrawLine(spawnPoint.position, spawnPoint.position + spawnPoint.forward);
            }
            if (exitPoint != null)
            {
                Gizmos.color = new Color(0.95f, 0.55f, 0.35f, 0.9f);
                Gizmos.DrawSphere(exitPoint.position, 0.2f);
                Gizmos.DrawLine(exitPoint.position, exitPoint.position + exitPoint.forward);
            }
        }
    }
}
