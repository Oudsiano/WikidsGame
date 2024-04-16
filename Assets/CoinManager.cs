using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinManager : MonoBehaviour
{
    [SerializeField] private GameObject _Coin;

    public void MakeGoldOnSceneWithCount(float count, Vector3 pos)
    {
        double countCoins = count;
        Instantiate(_Coin, pos, Quaternion.identity)
            .GetComponent<PickableCoin>()
            .Init(count);
    }

}
