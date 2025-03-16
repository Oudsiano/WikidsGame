using System;
using System.Collections.Generic;
using AINavigation;
using Combat;
using Core.Player;
using Core.Camera;
using Cysharp.Threading.Tasks;
using Data;
using Loading;
using Loading.LoadingOperations;
using Loading.LoadingOperations.Preloading;
using Saving;
using SceneManagement;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Utils;
using Zenject;

namespace Infrastructure.Installers.EntryPoint
{
    public class MapCompositionRoot : MonoBehaviour, ICompose
    {
        [SerializeField] private SceneContext _sceneContext;
        [SerializeField] private LocationChange _locationChange;
        [SerializeField] private LocationChange _locationChangeMobie;

        private DiContainer _sceneContainer;
        private MainPlayer _player;

        [Inject]
        public void Compose(DiContainer diContainer)
        {
            _sceneContainer = _sceneContext.Container;
            _player = _sceneContainer.Resolve<MainPlayer>();

            ConstructComponents();
        }

        private async void Start()
        {
            try
            {
                Debug.Log("Загрузка персонажа...");

                await LoadModularCharacter();
                await InitializeCharacter();
                SetupPlayerController();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Ошибка при настройке персонажа: {ex.Message}");
            }
        }

        private void ConstructComponents()
        {
            _locationChange.Construct(_sceneContainer.Resolve<DataPlayer>(),
                _sceneContainer.Resolve<LevelChangeObserver>(),
                _sceneContainer.Resolve<GameAPI>());
        }

        private async UniTask LoadModularCharacter()
        {
            var handle = Addressables.LoadAssetAsync<GameObject>("PlayerModel");

            var modularCharacter = await handle;

            if (modularCharacter == null)
                throw new Exception("Не удалось загрузить PlayerModel через Addressables");

            var playerModel = Instantiate(modularCharacter, _player.transform);

            HandPositionKeeper handPositionKeeper = playerModel.GetComponent<HandPositionKeeper>();

            _player.SetArmorManager(handPositionKeeper.PlayerArmorManager);
        }

        private async UniTask InitializeCharacter()
        {
            await UniTask.Yield();
            
            var handPositionKeeper = _player.GetComponentInChildren<HandPositionKeeper>();

            if (handPositionKeeper == null)
                throw new Exception("HandPositionKeeper не найден у персонажа");

            _player.PlayerController.Fighter
                .SetHandPositions(handPositionKeeper.RightHandPosition, handPositionKeeper.LeftHandPosition);

            _player.PlayerController.Fighter.EquipWeapon();
        }

        private void SetupPlayerController()
        {
            var handPositionKeeper = _player.GetComponentInChildren<HandPositionKeeper>();

            _player.PlayerController.SetModularCharacter(_player.gameObject);
            _player.PlayerController.SetPlayerArmorManager(handPositionKeeper.PlayerArmorManager);
        }
    }
}