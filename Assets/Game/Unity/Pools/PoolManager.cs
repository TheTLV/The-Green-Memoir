using System.Collections.Generic;
using UnityEngine;

namespace TheGreenMemoir.Unity.Pools
{
    /// <summary>
    /// Pool Manager - Quản lý tất cả pools trong game
    /// Singleton pattern + Object Pool Pattern
    /// </summary>
    public class PoolManager : MonoBehaviour
    {
        private static PoolManager _instance;
        public static PoolManager Instance => _instance;

        private Dictionary<string, IGameObjectPool> _pools = new Dictionary<string, IGameObjectPool>();

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Tạo pool mới
        /// </summary>
        public void CreatePool(string poolName, GameObject prefab, Transform parent = null, int initialSize = 10, int maxSize = 50)
        {
            if (_pools.ContainsKey(poolName))
            {
                Debug.LogWarning($"Pool '{poolName}' already exists");
                return;
            }

            var pool = new GameObjectPool(prefab, parent, initialSize, maxSize);
            _pools[poolName] = pool;

            Debug.Log($"Pool '{poolName}' created with {initialSize} initial objects");
        }

        /// <summary>
        /// Lấy GameObject từ pool
        /// </summary>
        public GameObject Get(string poolName)
        {
            if (!_pools.TryGetValue(poolName, out var pool))
            {
                Debug.LogError($"Pool '{poolName}' not found");
                return null;
            }

            return pool.Get();
        }

        /// <summary>
        /// Trả GameObject về pool
        /// </summary>
        public void Return(string poolName, GameObject obj)
        {
            if (!_pools.TryGetValue(poolName, out var pool))
            {
                Debug.LogError($"Pool '{poolName}' not found");
                return;
            }

            pool.Return(obj);
        }

        /// <summary>
        /// Xóa pool
        /// </summary>
        public void ClearPool(string poolName)
        {
            if (_pools.TryGetValue(poolName, out var pool))
            {
                pool.Clear();
                _pools.Remove(poolName);
            }
        }

        /// <summary>
        /// Xóa tất cả pools
        /// </summary>
        public void ClearAll()
        {
            foreach (var pool in _pools.Values)
            {
                pool.Clear();
            }
            _pools.Clear();
        }

        private void OnDestroy()
        {
            if (_instance == this)
            {
                ClearAll();
                _instance = null;
            }
        }
    }
}

