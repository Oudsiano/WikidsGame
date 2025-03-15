using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace Loading.LoadingOperations.Preloading
{
    public class ScenePreloaderService
    {
        private const int LoadResourceChunkCount = 20;

        private readonly Dictionary<string, List<GameObject>> _cache = new();

        public async UniTask<List<GameObject>> PreloadSceneAssets(string sceneLabel)
        {
            if (_cache.TryGetValue(sceneLabel, out var cached))
                return cached;

            var locations = await Addressables.LoadResourceLocationsAsync(sceneLabel).Task;

            var chunks = ChunkLocations(locations, LoadResourceChunkCount);
            var loadedObjects = new List<GameObject>(locations.Count);

            foreach (var chunk in chunks)
            {
                var loader = new ResourceChunkLoader();

                foreach (var location in chunk)
                    loader.AddLocation(location);

                var result = await loader.Load();
                loadedObjects.AddRange(result);
            }

            _cache[sceneLabel] = loadedObjects;
            return loadedObjects;
        }

        public List<GameObject> GetPreloaded(string sceneLabel)
        {
            return _cache.TryGetValue(sceneLabel, out var list) ? list : null;
        }

        private List<List<IResourceLocation>> ChunkLocations(IList<IResourceLocation> locations, int chunkSize)
        {
            var chunks = new List<List<IResourceLocation>>();
            for (int i = 0; i < locations.Count; i += chunkSize)
            {
                chunks.Add(locations.Skip(i).Take(chunkSize).ToList());
            }

            return chunks;
        }
    }
}