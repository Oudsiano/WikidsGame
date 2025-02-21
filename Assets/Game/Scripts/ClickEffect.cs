using UnityEngine;
using UnityEngine.Serialization;

public class ClickEffect : MonoBehaviour // TODO Factory 
{
    [FormerlySerializedAs("effectPrefab")][SerializeField] private ParticleSystem _prefab; 
    
    public void CreateEffect(Vector3 position)
    {
        Instantiate(_prefab, position, Quaternion.identity);
    }
}