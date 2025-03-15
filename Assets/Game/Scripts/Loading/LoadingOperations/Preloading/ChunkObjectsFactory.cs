using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Loading.LoadingOperations.Preloading
{
    public class ChunkObjectsFactory
    {
        private readonly Transform _parent;
        private readonly List<GameObject> _objects = new List<GameObject>();

        public ChunkObjectsFactory(Transform parent)
        {
            _parent = parent;
        }

        public void AddObject(GameObject obj)
        {
            _objects.Add(obj);
        }

        public async UniTask Create(Action<GameObject> createdCallback)
        {
            for (int i = 0; i < _objects.Count; i++)
            {
                var obj = Object.Instantiate(_objects[i], _parent);

                createdCallback(obj);
            }

            await UniTask.Yield();
        }

        public void Reset()
        {
            _objects.Clear();
        }
    }
}