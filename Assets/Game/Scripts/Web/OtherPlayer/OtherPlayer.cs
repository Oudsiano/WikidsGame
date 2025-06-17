using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class OtherPlayer : MonoBehaviour
{
    [SerializeField] private OtherPlayerController _otherPlayerController;
    [SerializeField] private IconForFarCamera _iconForFarCamera;
    [SerializeField] private Slider _healthSlider;
    
    private bool _ifModularCharacterCreated;
    private float _currentHealth;
    
    public bool IfModularCharacterCreated=>_ifModularCharacterCreated;
    public OtherPlayerController OtherPlayerController => _otherPlayerController;

    public void Construct(UIManager uiManager)
    {
        _otherPlayerController.Construct();
        _iconForFarCamera.Construct(uiManager);
    }
    
   

    public void UpdateHealth(float health)
    {
        _currentHealth = health;

        if (_healthSlider != null)
        {
            _healthSlider.value = _currentHealth;
        }
        else
        {
            Debug.LogWarning("[OtherPlayer] Health slider is not assigned!");
        }
        
        // Например, можно сделать визуальный фидбек
        Debug.Log($"[OtherPlayerController] Current health updated: {_currentHealth}");

        // Здесь можно обновить UI, полоску HP и т.п.
    }
    
    public void IsCreatedModularCharacter()
    {
        _ifModularCharacterCreated = true;
    }
}
