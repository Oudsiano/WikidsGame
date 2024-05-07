using FarrokhGames.Inventory.Examples;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiMarketPanel : MonoBehaviour
{
    [SerializeField] private GameObject _confirmPanel;
    [SerializeField] private Button _buttonAccept;
    [SerializeField] private Button _buttonDecline;

    [SerializeField] public SizeInventoryExample Inventory;
    [SerializeField] public SizeInventoryExample InventoryWeapon;
    [SerializeField] public SizeInventoryExample InventoryArmor;

    private Action<Vector2Int> _accept;
    private Action _decline;
    private Vector2Int _grid;

    private void Awake()
    {
        _buttonAccept.onClick.AddListener(OnClickAccept);
        _buttonDecline.onClick.AddListener(OnClickDecline);
        _confirmPanel.SetActive(false);
    }

    public void Init()
    {
        Inventory.Init();
        InventoryWeapon.Init();
        InventoryArmor.Init();

        foreach (var item in IGame.Instance.WeaponManager.allWeaponsInGame)
        {
            if (item.sprite != null)
                Inventory.inventory.TryAdd(item);
        }
    }

    public void InitMarketUI(Action<Vector2Int> accept, Action decline, Vector2Int grid)
    {
        _accept = accept;
        _decline = decline;
        _grid = grid;
        _confirmPanel.SetActive(true);
    }

    private void OnClickDecline()
    {
        _decline?.Invoke();
        _confirmPanel.SetActive(false);
    }

    private void OnClickAccept()
    {
        _accept?.Invoke(_grid);
        _confirmPanel.SetActive(false);
    }
}
