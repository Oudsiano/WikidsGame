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


        [Inject]
        public void Compose(DiContainer diContainer)
        {
            _sceneContainer = _sceneContext.Container;

            ConstructComponents();
        }

        private async void Start()
        {
            Debug.Log("Loading modular character");
            // var modularCharacter = await _sceneContainer.Resolve<LocalAssetLoader>().Load<GameObject>("ModularCharacter");

            var handle = Addressables.LoadAssetAsync<GameObject>("PlayerModel");
            await handle;
            GameObject modularCharacter = handle.Result;
            var playerModel = Instantiate(modularCharacter, _sceneContainer.Resolve<MainPlayer>().transform);

            HandPositionKeeper handPositionKeeper = playerModel.GetComponent<HandPositionKeeper>();
            Debug.Log("ModularCharacters loaded");

            _sceneContainer.Resolve<MainPlayer>().SetArmorManager(handPositionKeeper.PlayerArmorManager);
            Debug.Log("SetArmorManager(armorManager)");
            _sceneContainer.Resolve<MainPlayer>().PlayerController.Fighter
                .SetHandPositions(handPositionKeeper.RightHandPosition, handPositionKeeper.LeftHandPosition);
            _sceneContainer.Resolve<MainPlayer>().PlayerController.Fighter.EquipWeapon();

            _sceneContainer.Resolve<MainPlayer>().PlayerController.SetModularCharacter(modularCharacter.gameObject);

            _sceneContainer.Resolve<MainPlayer>().PlayerController
                .SetPlayerArmorManager(handPositionKeeper.PlayerArmorManager);
            Debug.Log("Все отработало Start");
        }

        private void ConstructComponents()
        {
            _locationChange.Construct(_sceneContainer.Resolve<DataPlayer>(),
                _sceneContainer.Resolve<LevelChangeObserver>(),
                _sceneContainer.Resolve<GameAPI>());
        }
    }
}