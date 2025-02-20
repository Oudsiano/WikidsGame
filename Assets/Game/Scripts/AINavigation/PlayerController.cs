using System;
using System.Collections.Generic;
using Combat;
using Combat.EnumsCombat;
using DialogueEditor;
using Movement;
using RPG.Core;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace AINavigation
{
    public class PlayerController : MonoBehaviour
    {
        [FormerlySerializedAs("invizVFXPrefab")] [SerializeField]
        private GameObject _invisibilityVFXPrefab; // TODO GO

        [FormerlySerializedAs("exitInvizVFXPrefab")] [SerializeField]
        private GameObject _exitInvisibilityVFXPrefab; // TODO GO

        [FormerlySerializedAs("invizAudioSource")] [SerializeField]
        private AudioSource
            _invisibilityAudioSource; // Звуковой компонент для входа в невидимость // TODO -> AudioService

        [FormerlySerializedAs("exitInvizAudioSource")] [SerializeField]
        private AudioSource
            _exitInvisibilityAudioSource; // Звуковой компонент для выхода из невидимости // TODO -> AudioService

        [FormerlySerializedAs("playerArmorManager")]
        public PlayerArmorManager PlayerArmorManager; // TODO O/C breach

        public WeaponPanelUI WeaponPanelUI; // TODO O/C breach

        [FormerlySerializedAs("modularCharacter")]
        public GameObject ModularCharacter; // TODO O/C breach

        private Fighter _fighter;
        private Mover _mover;
        private Health _health;
        private int _enemyLayer = 9; // Номер слоя для врагов  // TODO change

        private List<Fighter> _allEnemies;
        private bool _invisibilityState = false; // невидимость (Стелс) выключена

        private GameObject _activeInvisibilityVFX; // Текущий активный VFX объект для невидимости

        private void Update()
        {
            if (pauseClass.GetPauseState())
            {
                return;
            }

            if (_health.IsDead()) // TODO rename players = isDead, not is health
            {
                return;
            }

            // Пытаемся взаимодействовать с боем
            if (InteractWithCombat())
            {
                return;
            }

            // Пытаемся взаимодействовать с клавиатурой
            InteractWithKeyboard();

            // Пытаемся взаимодействовать с перемещением
            if (InteractWithMovement())
            {
                return; // TODO not used
            }
        }

        public void Init() // TODO Construct
        {
            _mover = GetComponent<Mover>(); // TODO RequieredComponents  
            _fighter = GetComponent<Fighter>();
            _health = GetComponent<Health>();

            PlayerArmorManager = FindObjectOfType<PlayerArmorManager>(); // TODO Delete

            WeaponPanelUI = FindObjectOfType<WeaponPanelUI>(); // TODO Delete
            WeaponPanelUI.Init();

            SceneManager.sceneLoaded += SceneLoader_LevelChanged;
            IGame.Instance.saveGame.OnLoadItems += SaveGame_OnOnLoadItems;
        }

        public Health GetHealth() => _health; // TODO move to properties

        public Fighter GetFighter() => _fighter; // TODO move to properties

        public bool GetPlayerInvisibility() => _invisibilityState; // TODO move to properties

        public void SetInvisByBtn(bool isInvisibility)
        {
            if (_invisibilityState == isInvisibility)
            {
                return;
            }

            _invisibilityState = isInvisibility;

            if (isInvisibility)
            {
                // Создание VFX для входа в невидимость
                _activeInvisibilityVFX = PlayVFX(_invisibilityVFXPrefab);

                if (_invisibilityAudioSource != null)
                {
                    _invisibilityAudioSource.Play();
                }
            }
            else
            {
                // Уничтожение VFX для невидимости и создание VFX для выхода из невидимости
                if (_activeInvisibilityVFX != null)
                {
                    Destroy(_activeInvisibilityVFX);
                }

                PlayVFX(_exitInvisibilityVFXPrefab);

                if (_exitInvisibilityAudioSource != null)
                {
                    _exitInvisibilityAudioSource.Play();
                }
            }
        }

        private void SaveGame_OnOnLoadItems()
        {
            _fighter.EquipItem(IGame.Instance.saveGame.EquipedArmor);
            _fighter.EquipItem(IGame.Instance.saveGame.EquipedWeapon);
        }

        private void SceneLoader_LevelChanged(Scene scene, LoadSceneMode mode) // TODO move to one SceneLoaderService
        {
            IGame.Instance.saveGame.MakeLoad();
            IGame.Instance.saveGame.SetBonusWeaponAndArmorIfNeed(); //TODO перенести эту функцию куда то еще
            EquipWeaponAndArmorAfterLoad();
        }

        private GameObject PlayVFX(GameObject vfxPrefab)
        {
            if (vfxPrefab == null)
            {
                return null;
            }

            GameObject vfxInstance = Instantiate(vfxPrefab, transform.position, transform.rotation, transform);
            ParticleSystem vfx = vfxInstance.GetComponent<ParticleSystem>(); // TODO move to one call

            if (vfx != null)
            {
                vfx.Play();
                if (!vfx.main.loop)
                {
                    Destroy(vfxInstance, vfx.main.duration);
                }
            }

            return vfxInstance;
        }

        private void EquipWeaponAndArmorAfterLoad()
        {
            if (IGame.Instance.dataPlayer.playerData.weaponToLoad.Length > 1) // TODO magic number
            {
                _fighter.EquipWeapon(
                    IGame.Instance.WeaponArmorManager.TryGetWeaponByName(IGame.Instance.dataPlayer.playerData
                        .weaponToLoad));
            }

            IGame.Instance.WeaponArmorManager.GerArmorById((armorID)IGame.Instance.dataPlayer.playerData.armorIdToload)
                .EquipIt();
        }

        private void InteractWithKeyboard()
        {
            if (Input.GetKeyDown(KeyCode.D))
            {
                _fighter.UnequipWeapon();
            }
        }

        private bool InteractWithCombat()
        {
            // Применяем побитовую операцию, чтобы получить только слой врагов
            int layerMask = 1 << _enemyLayer; // TODO magic number

            var ray = GetMouseRay(); // TODO change

            RaycastHit[] hits;
            hits = Physics.RaycastAll(ray, Mathf.Infinity, layerMask);

            foreach (var enemyHit in hits)
            {
                CombatTarget target = enemyHit.transform.gameObject.GetComponent<CombatTarget>(); // TODO change 

                if (target == false || _fighter.CanAttack(target.gameObject) == false)
                {
                    continue;
                }

                if (Input.GetMouseButtonDown(0))
                {
                    _fighter.Attack(target.gameObject);
                }

                return true;
            }

            return false;
        }

        private bool InteractWithMovement()
        {
            // Получаем луч из мыши // TODO not used code
            /*Ray ray = GetMouseRay();
            int layerMask = ~LayerMask.GetMask("PLayer");
            bool hasHit = Physics.Raycast(ray, out RaycastHit hit, layerMask);*/
            
            if (Input.GetMouseButton(0))
                if (EventSystem.current.IsPointerOverGameObject() == false)
                {
                    Ray ray = GetMouseRay();
                    RaycastHit[] hits = Physics.RaycastAll(ray);
                    Array.Sort(hits, (h1, h2) => h1.distance.CompareTo(h2.distance));

                    RaycastHit hit;
                    
                    for (int i = 0; i < hits.Length; i++)
                    {
                        if (hits[i].collider.gameObject.name != "Player") // TODO change logic call for name object
                        {
                            hit = hits[i];

                            bool readyToGo = true;

                            if (ConversationManager.Instance != null)
                            {
                                if (ConversationManager.Instance.IsConversationActive)
                                {
                                    readyToGo = false;
                                }
                            }

                            if (readyToGo)
                            {
                                _mover.StartMoveAction(hit.point);
                            }

                            break; // Выходим, найдя первый подходящий объект
                        }
                    }
                }

            return true;
        }

        private Ray GetMouseRay() // TODO change
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // TODO Camera.main can be cached

            return ray;
        }
    }
}