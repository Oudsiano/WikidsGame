using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using Core;
using Core.PickableItems;

public class Curency
{
    private double _count;
    public double Count { get { return _count; } }
    public double OnLoadCount
    {
        set
        {
            _count = value;
            OnChangeCount?.Invoke(_count);
        }
    }

    public event Action<double> OnChangeCount;
    public void ChangeCount(double change)
    {
        _count += change;
        OnChangeCount?.Invoke(_count);
    }

    public void SetCount(double count)
    {
        _count = count;
        IGame.Instance.saveGame.MakeSave();
        OnChangeCount?.Invoke(_count);
    }
}

public class CoinManager : MonoBehaviour
{
    [SerializeField] private GameObject _Coin;

    private Curency coins;

    public void Init()
    {
        Coins = new Curency();
    }

    public Curency Coins { get => coins; set => coins = value; }

    public void MakeGoldOnSceneWithCount(float count, Vector3 pos)
    {
        double countCoins = count;
        Instantiate(_Coin, pos, Quaternion.Euler(0, 0, 0))
            .GetComponent<PickableCoin>()
            .Init(count);
        AudioManager.instance.PlaySound("CoinPickup");

    }

}
