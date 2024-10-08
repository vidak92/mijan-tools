using System.Collections.Generic;
using UnityEngine;

namespace MijanTools.Components
{
    // public interface IPoolable<T> where T : MonoBehaviour, IPoolable<T>
    // {
    //     public ObjectPool<T> Pool { get; set; }
    // }

    // [System.Serializable]
    public class ObjectPool<T> where T : Component//, IPoolable<T>
    {
        private T _prefab;
        private int _initialCapacity;
        private List<T> _pool;
        private int _count;

        public Transform Parent { get; private set; }

        public static ObjectPool<T> CreateWithGameObject(T prefab, int initialCapacity, string gameObjectName)
        {
            var gameObject = new GameObject(gameObjectName);
            var pool = new ObjectPool<T>(prefab, initialCapacity, gameObject.transform);
            return pool;
        }

        public ObjectPool(T prefab, int initialCapacity, Transform parent)
        {
            _count = 0;
            _prefab = prefab;
            _initialCapacity = initialCapacity;
            _pool = new List<T>();
            Parent = parent;
            for (int i = 0; i < _initialCapacity; i++)
            {
                AddNewObject();
            }
        }

        public void Return(T poolable)
        {
            if (_pool.Contains(poolable))
            {
                Debug.LogWarning($"Object pool already contains object {poolable}, skipping return.");
                return;
            }
            
            poolable.gameObject.SetActive(false);
            poolable.transform.parent = Parent;
            _pool.Add(poolable);
        }

        public T Get()
        {
            if (_pool.Count == 0)
            {
                AddNewObject();
            }

            if (_pool.Count > 0)
            {
                var poolable = _pool[0];
                _pool.RemoveAt(0);
                poolable.gameObject.SetActive(true);
                poolable.gameObject.transform.parent = null;
                return poolable;
            }
            else
            {
                Debug.LogWarning($"{nameof(ObjectPool<T>)}: Pool is empty, cannot get new object.");
                return null;
            }
        }

        private void AddNewObject()
        {
            var poolable = Object.Instantiate(_prefab, Parent);
            if (poolable != null)
            {
                poolable.gameObject.name = $"{poolable.gameObject.name}_{_count}";
                poolable.gameObject.SetActive(false);
                // poolable.Pool = this;
                _pool.Add(poolable);
                _count++;
            }
            else
            {
                Debug.LogError($"{nameof(ObjectPool<T>)}: Cannot instantiate prefab.");
            }
        }
    }
}