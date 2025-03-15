using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace Loading.LoadingOperations.Preloading
{
    public class ResourceChunkLoader
    {
        private List<IResourceLocation> _locations = new List<IResourceLocation>();

        public void AddLocation(IResourceLocation location)
        {
            _locations.Add(location);
        }

        public async UniTask<List<GameObject>> Load()
        {
            var tasks = new UniTask<GameObject>[_locations.Count];

            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = Addressables.LoadAssetAsync<GameObject>(_locations[i]).ToUniTask();
            }

            var objects = await UniTask.WhenAll(tasks);

            return objects.ToList();
        }
    }
}