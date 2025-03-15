using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.SceneManagement;

namespace Loading.LoadingOperations.Preloading
{
    public class SceneResourceLoader : MonoBehaviour
    {
        private const int CreateObjectChunkCount = 3;
        private const int LoadResourceChunkCount = 20;

        [SerializeField] private bool _dontLoad;
        [SerializeField] private float _delay;
        [SerializeField] private Transform _parent;

        private string _label;

        public bool Loaded { get; private set; }

        public async UniTask Init()
        {
            _label = SceneManager.GetActiveScene().name;

            if (_dontLoad)
            {
                await UniTask.WaitForSeconds(_delay);
            }
            else
            {
                await Load();
            }
        }

        private void Awake()
        {
            if (_dontLoad)
            {
                _ = Timer();
            }
        }

        private async UniTaskVoid Timer()
        {
            await UniTask.WaitForSeconds(_delay);

            Loaded = true;
        }

        private async UniTask Load()
        {
            var locations = await Addressables.LoadResourceLocationsAsync(_label).Task;

            await CreateLevelObjects(await LoadResources());

            Loaded = true;

            return;

            async UniTask<List<GameObject>> LoadResources()
            {
                var result = await UniTask.WhenAll(GetLoadChunks(locations));

                var objects = new List<GameObject>(locations.Count);

                for (int i = 0; i < result.Length; i++)
                {
                    objects.AddRange(result[i]);
                }

                return objects;
            }
        }

        private async UniTask CreateLevelObjects(List<GameObject> loadedResources)
        {
            var factory = new ChunkObjectsFactory(_parent);

            var objCounter = 0;

            var counter = 0;

            for (int i = 0; i < loadedResources.Count; i++)
            {
                factory.AddObject(loadedResources[i]);
                counter++;

                if (i < loadedResources.Count - 1 && counter < CreateObjectChunkCount)
                {
                    continue;
                }

                counter = 0;

                await factory.Create(obj => { objCounter++; });

                factory.Reset();
            }
        }

        private List<UniTask<List<GameObject>>> GetLoadChunks(IList<IResourceLocation> locations)
        {
            var tasks = new List<UniTask<List<GameObject>>>();

            var loader = new ResourceChunkLoader();

            var counter = 0;

            for (int i = 0; i < locations.Count; i++)
            {
                loader.AddLocation(locations[i]);
                counter++;

                if (i < locations.Count - 1 && counter < LoadResourceChunkCount)
                {
                    continue;
                }

                counter = 0;

                tasks.Add(StartLoadChunk(loader));
                loader = new ResourceChunkLoader();
            }

            return tasks;

            async UniTask<List<GameObject>> StartLoadChunk(
                ResourceChunkLoader loader)
            {
                var objects = await loader.Load();

                return objects;
            }
        }
    }
}