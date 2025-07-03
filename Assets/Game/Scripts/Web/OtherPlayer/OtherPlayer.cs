using System.Collections;
using System.Collections.Generic;
using Healths;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class OtherPlayer : MonoBehaviour
{
    [SerializeField] private OtherPlayerController _otherPlayerController;
    [SerializeField] private IconForFarCamera _iconForFarCamera;
    [SerializeField] private OtherPlayerHealth _otherPlayerHealth;
    
    public OtherPlayerHealth OtherPlayerHealth => _otherPlayerHealth;
    
    private bool _ifModularCharacterCreated;
    
    

    
    public bool IfModularCharacterCreated=>_ifModularCharacterCreated;
    public OtherPlayerController OtherPlayerController => _otherPlayerController;

    public void Construct(UIManager uiManager)
    {
        _otherPlayerController.Construct();
        _iconForFarCamera.Construct(uiManager);
    }
    
   


    
    public void IsCreatedModularCharacter()
    {
        _ifModularCharacterCreated = true;
    }
}
