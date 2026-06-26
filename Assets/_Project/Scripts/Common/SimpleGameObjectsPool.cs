using System;
using System.Collections.Generic;
using R3;
using UnityEngine;

namespace CombatArena
{
    public class SimpleGameObjectsPool : IDisposable
    {
        private Transform _parentTransform;

        private Dictionary<string, Queue<GameObject>> _simpleGameObjectsPool;
        private Dictionary<int, IDisposable> _returnDisposables;
        private int _returnIndex;

        public SimpleGameObjectsPool(Transform parentTransform, params GameObject[] startedObjects)
        {
            _parentTransform = parentTransform;
            _simpleGameObjectsPool = new();
            _returnDisposables = new();
            _returnIndex = -1;

            Add(startedObjects);
        }

        public void Dispose()
        {
            foreach (var disposable in _returnDisposables.Values) disposable?.Dispose();
        }

        public void Add(params GameObject[] prefabs)
        {
            for (int i = 0; i < prefabs.Length; i++)
            {
                CreatePool(prefabs[i]);
            }
        }

        public GameObject Get(string gameObjectName)
        {
            if (!_simpleGameObjectsPool.ContainsKey(gameObjectName)) throw new NullReferenceException($"Object by name: {gameObjectName}  wasn't initialized as pool");

            var pool = _simpleGameObjectsPool[gameObjectName];
            var obj = pool.Dequeue();
            obj.SetActive(true);
            obj.transform.parent = null;

            return obj;
        }

        public GameObject Get(GameObject prefab)
        {
            if (!_simpleGameObjectsPool.ContainsKey(prefab.name)) CreatePool(prefab);

            var pool = _simpleGameObjectsPool[prefab.name];

            if (pool.Count == 0)
            {
                for (int i = 0; i < 10; i++)
                {
                    var addedObj = GameObject.Instantiate(prefab, _parentTransform);
                    addedObj.name = prefab.name;
                    addedObj.SetActive(false);
                    pool.Enqueue(addedObj);
                }
            }

            return Get(prefab.name);
        }

        public void Return(GameObject gameObject)
        {
            if (!_simpleGameObjectsPool.ContainsKey(gameObject.name))
            {
                Debug.LogWarning($"GameObject by name {gameObject.name} wasn't initialized as pool. Destroying instead.");
                GameObject.Destroy(gameObject);
                return;
            }

            gameObject.SetActive(false);
            gameObject.transform.position = _parentTransform.position;
            gameObject.transform.SetParent(_parentTransform);
            _simpleGameObjectsPool[gameObject.name].Enqueue(gameObject);
        }

        public void Return(GameObject gameObject, float delay)
        {
            if (delay <= 0)
            {
                Return(gameObject);
                return;
            }

            _returnIndex++;
            if (_returnIndex >= int.MaxValue) _returnIndex = 0;

            int index = _returnIndex;
            var disposable = Observable.Timer(TimeSpan.FromSeconds(delay)).Subscribe(_ =>
            {
                _returnDisposables[index]?.Dispose();
                _returnDisposables.Remove(index);

                Return(gameObject);
            });

            _returnDisposables.Add(_returnIndex, disposable);
        }

        private void CreatePool(GameObject prefab, int amount = 10)
        {
            if (_simpleGameObjectsPool.ContainsKey(prefab.name)) return;

            var newPool = new Queue<GameObject>();

            for (int i = 0; i < amount; i++)
            {
                var obj = GameObject.Instantiate(prefab, _parentTransform);
                obj.name = prefab.name;
                obj.SetActive(false);
                newPool.Enqueue(obj);
            }

            _simpleGameObjectsPool.Add(prefab.name, newPool);
        }
    }
}
