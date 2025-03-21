using AINavigation;
using Combat.Data;
using Data;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI
{
    public class WeaponPanelUI : MonoBehaviour // TODO UI
    {
        // [SerializeField] public Button CommonWeaponBTN; // TODO rename
        [SerializeField] public Button FireballBTN;
        [SerializeField] public Button BowBTN;  
        
        [FormerlySerializedAs("FireballText")] [SerializeField] private TMPro.TextMeshProUGUI _fireballText;

        [FormerlySerializedAs("bowWeapon")] [SerializeField] private Weapon _bowWeapon;

        private PlayerController _playerController;
        private DataPlayer _dataPlayer;
        
        public void Construct(PlayerController playerController, DataPlayer dataPlayer) // TODO construct
        {
            Debug.Log("***********************************");
            Debug.Log("Start Construct WeaponPanel");
            _playerController = playerController;
            Debug.Log("PlayerController assigned");
            _dataPlayer = dataPlayer;
            Debug.Log("dataPlayer assigned");
            // CommonWeaponBTN.onClick.AddListener(OnCLickCommonWeaponBTN);
            FireballBTN.onClick.AddListener(OnCLickFireballBTN);
            BowBTN.onClick.AddListener(OnClickBowBTN); 

            // ResetWeaponToDefault();
            Debug.Log("ResetWeaponToDefault");
            // ResetFireballCount();
            Debug.Log("ResetFireballCount");
            Debug.Log("Finish Construct WeaponPanel");
            Debug.Log("***********************************");
        }
        
        public void ResetWeaponToDefault()
        {
            if (_playerController == null)
            {
                Debug.LogError("Player controller is null");
            }
            var fighter = _playerController.Fighter;
            
            if (fighter == null)
            {
                Debug.LogError("Fighter is NULL in ResetWeaponToDefault()");
                return;
            }
            _playerController.Fighter.SetCommonWeapon();
        }

        public void ResetFireballCount()
        {
            if (_dataPlayer.PlayerData.chargeEnergy > 0)
            {
                _fireballText.text = "Fireball";  // (" + _dataPLayer.playerData.chargeEnergy.ToString() + ")";
                FireballBTN.gameObject.SetActive(true);
            }
            else
            {
                ResetWeaponToDefault();
            }
        }
        
        private void OnCLickFireballBTN()
        {
            _playerController.Fighter.SetFireball();
        }

        // private void OnCLickCommonWeaponBTN() => ResetWeaponToDefault();

        private void OnClickBowBTN()
        {
            _playerController.Fighter.SetBow(); 
        }
    }
}