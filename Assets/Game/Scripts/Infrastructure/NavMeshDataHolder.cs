using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AI;

namespace Infrastructure
{
    public class NavMeshDataHolder: MonoBehaviour
    {
        [SerializeField] private AssetReferenceT<NavMeshData> _navMeshData;

        public AssetReferenceT<NavMeshData> NavMeshData => _navMeshData;
    }
}