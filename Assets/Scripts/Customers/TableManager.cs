using System.Collections.Generic;
using UnityEngine;

namespace Indigo.Customers
{
    public class TableManager : MonoBehaviour
    {
        public static TableManager Instance { get; private set; }

        private readonly List<Table> _tables = new List<Table>();

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            RefreshTables();
        }

        void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        public void RefreshTables()
        {
            _tables.Clear();
            _tables.AddRange(FindObjectsByType<Table>(FindObjectsInactive.Exclude, FindObjectsSortMode.None));
        }

        public Table FindFreeTable()
        {
            for (int i = 0; i < _tables.Count; i++)
            {
                var t = _tables[i];
                if (t != null && !t.IsOccupied) return t;
            }
            return null;
        }

        public bool HasFreeTable()
        {
            for (int i = 0; i < _tables.Count; i++)
            {
                if (_tables[i] != null && !_tables[i].IsOccupied) return true;
            }
            return false;
        }
    }
}
