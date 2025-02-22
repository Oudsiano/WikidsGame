using Combat.Data;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI
{
    public class WeaponPanelUI : MonoBehaviour // TODO UI
    {
        [SerializeField] public Button CommonWeaponBTN; // TODO rename
        [SerializeField] public Button FireballBTN;
        [SerializeField] public Button BowBTN;  

        [SerializeField] public GameObject CommonWeaponPanell;
        [SerializeField] public GameObject FireballPanell;
        [SerializeField] public GameObject BowPanell;  

        [FormerlySerializedAs("FireballText")] [SerializeField] private TMPro.TextMeshProUGUI _fireballText;

        [FormerlySerializedAs("bowWeapon")] [SerializeField] private Weapon _bowWeapon;  

        public void Construct() // TODO construct
        {
            CommonWeaponBTN.onClick.AddListener(OnCLickCommonWeaponBTN);
            FireballBTN.onClick.AddListener(OnCLickFireballBTN);
            BowBTN.onClick.AddListener(OnClickBowBTN); 

            ResetWeaponToDefault();
            ResetFireballCount();
        }
        
        public void ResetWeaponToDefault()
        {
            IGame.Instance.playerController.GetFighter().SetCommonWeapon();
            FireballPanell.SetActive(false);
            CommonWeaponPanell.SetActive(true);
            BowPanell.SetActive(false);
        }

        public void ResetFireballCount()
        {
            if (IGame.Instance.dataPlayer.PlayerData.chargeEnergy > 0)
            {
                _fireballText.text = "Fireball";  // (" + IGame.Instance.dataPLayer.playerData.chargeEnergy.ToString() + ")";
                FireballBTN.gameObject.SetActive(true);
            }
            else
            {
                ResetWeaponToDefault();
                FireballBTN.gameObject.SetActive(false);
            }
        }
        
        private void OnCLickFireballBTN()
        {
            IGame.Instance.playerController.GetFighter().SetFireball();
            FireballPanell.SetActive(true);
            CommonWeaponPanell.SetActive(false);
            BowPanell.SetActive(false); 
        }

        private void OnCLickCommonWeaponBTN() => ResetWeaponToDefault();

        private void OnClickBowBTN()
        {
            IGame.Instance.playerController.GetFighter().SetBow(); 
            BowPanell.SetActive(true);
            CommonWeaponPanell.SetActive(false);
            FireballPanell.SetActive(false);  
        }
    }
}