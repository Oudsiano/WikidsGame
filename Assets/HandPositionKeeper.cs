using System.Collections;
using System.Collections.Generic;
using Combat;
using UnityEngine;

public class HandPositionKeeper : MonoBehaviour
{
    [SerializeField] private Transform _rightHandPosition;
    [SerializeField] private Transform _leftHandPosition;
    [SerializeField] private PlayerArmorManager _playerArmorManager;

    public Transform RightHandPosition => _rightHandPosition;
    public Transform LeftHandPosition => _leftHandPosition;
    public PlayerArmorManager PlayerArmorManager => _playerArmorManager;
}
