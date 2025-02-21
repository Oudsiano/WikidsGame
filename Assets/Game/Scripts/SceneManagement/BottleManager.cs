using System.Collections;
using System.Collections.Generic;
using Core;
using Core.PickableItems;
using UnityEngine;

public class BottleManager : MonoBehaviour
{
    [SerializeField] private GameObject _Bottle;

    public void MakeBottleOnSceneWithCount(float count, Vector3 pos)
    {
        double countCoins = count;
        Instantiate(_Bottle, pos, Quaternion.Euler(0, 0, 0))
            .GetComponent<PickableHPBottle>()
            .Init(count);

    }
}
