using System.Collections.Generic;
using UnityEngine;

namespace TheGreenMemoir.Unity.Pools
{
    /// <summary>
    /// GameObject Pool - Object Pool Pattern
    /// Tái sử dụng GameObjects để tối ưu performance
    /// </summary>
    public class GameObjectPool : IGameObjectPool
    {
        private readonly GameObject _prefab;
        private readonly Transform _parent;
        private readonly Queue<GameObject> _pool = new Queue<GameObject>();
        private readonly int _initialSize;
        private readonly int _maxSize;

        public GameObjectPool(GameObject prefab, Transform parent = null, int initialSize = 10, int maxSize = 50)
        {
            _prefab = prefab ?? throw new System.ArgumentNullException(nameof(prefab));
            _parent = parent;
            _initialSize = initialSize;
            _maxSize = maxSize;

            // Tạo pool ban đầu
            for (int i = 0; i < initialSize; i++)
            {
                var obj = CreateNew();
                obj.SetActive(false);
                _pool.Enqueue(obj);
            }
        }

        public GameObject Get()
        {
            GameObject obj;

            if (_pool.Count > 0)
            {
                obj = _pool.Dequeue();
            }
            else
            {
                obj = CreateNew();
            }

            obj.SetActive(true);
            return obj;
        }

        public void Return(GameObject obj)
        {
            if (obj == null) return;

            obj.SetActive(false);
            obj.transform.SetParent(_parent);

            // Chỉ thêm vào pool nếu chưa đạt max size
            if (_pool.Count < _maxSize)
            {
                _pool.Enqueue(obj);
            }
            else
            {
                // Nếu pool đầy, destroy object
                Object.Destroy(obj);
            }
        }

        public void Clear()
        {
            while (_pool.Count > 0)
            {
                var obj = _pool.Dequeue();
                if (obj != null)
                {
                    Object.Destroy(obj);
                }
            }
        }

        private GameObject CreateNew()
        {
            var obj = Object.Instantiate(_prefab, _parent);
            obj.name = $"{_prefab.name} (Pooled)";
            return obj;
        }
    }
}

