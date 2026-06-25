using System;
using System.Collections.Generic;
using R3;
using UnityEngine;

namespace CombatArena
{
    public class SimpleGameObjectsPool : IDisposable
    {
        private Transform _parentTransform;

        private Dictionary<string, Queue<GameObject>> _simpleParticlesPool;
        private Dictionary<int, IDisposable> _returnDisposables;
        private int _returnIndex;

        public SimpleGameObjectsPool(Transform parentTransform, params GameObject[] startedObjects)
        {
            _parentTransform = parentTransform;
            _simpleParticlesPool = new();
            _returnDisposables = new();
            _returnIndex = -1;

            for (int i = 0; i < startedObjects.Length; i++)
            {
                CreatePool(startedObjects[i]);
            }
        }

        public void Dispose()
        {
            foreach (var disposable in _returnDisposables.Values) disposable?.Dispose();
        }

        public GameObject GetParticle(GameObject prefab)
        {
            if (!_simpleParticlesPool.ContainsKey(prefab.name)) CreatePool(prefab);

            var pool = _simpleParticlesPool[prefab.name];

            if (pool.Count == 0)
            {
                for (int i = 0; i < 10; i++)
                {
                    var particle = GameObject.Instantiate(prefab, _parentTransform);
                    particle.name = prefab.name;
                    particle.SetActive(false);
                    pool.Enqueue(particle);
                }
            }

            var obj = pool.Dequeue();
            obj.SetActive(true);
            obj.transform.parent = null;

            return obj;
        }

        public void ReturnParticle(GameObject gameObject)
        {
            if (!_simpleParticlesPool.ContainsKey(gameObject.name))
            {
                GameObject.Destroy(gameObject);
                return;
            }

            gameObject.SetActive(false);
            gameObject.transform.SetParent(_parentTransform);
            _simpleParticlesPool[gameObject.name].Enqueue(gameObject);
        }

        public void ReturnParticle(GameObject gameObject, float delay)
        {
            if (delay <= 0)
            {
                ReturnParticle(gameObject);
                return;
            }

            _returnIndex++;
            if (_returnIndex >= int.MaxValue) _returnIndex = 0;

            int index = _returnIndex;
            var disposable = Observable.Timer(TimeSpan.FromSeconds(delay)).Subscribe(_ =>
            {
                _returnDisposables[index]?.Dispose();
                _returnDisposables.Remove(index);

                ReturnParticle(gameObject);
            });

            _returnDisposables.Add(_returnIndex, disposable);
        }

        private void CreatePool(GameObject prefab)
        {
            var newPool = new Queue<GameObject>();

            for (int i = 0; i < 20; i++)
            {
                var particle = GameObject.Instantiate(prefab, _parentTransform);
                particle.name = prefab.name;
                particle.SetActive(false);
                newPool.Enqueue(particle);
            }

            _simpleParticlesPool.Add(prefab.name, newPool);
        }
    }
}
