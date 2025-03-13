using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Data;
using Loading.LoadingOperations;
using Saving;
using UnityEngine;
using Utils;

namespace Infrastructure.Installers.EntryPoint
{
    public class BootstrapFlowService
    {
        private MultiScenePreloader _preloader;
        private DataPlayer _dataPlayer;

        public async UniTask RunBootstrapFlow(GameAPI gameAPI, MultiScenePreloader preloader, DataPlayer dataPlayer)
        {
            _preloader = preloader;
            _dataPlayer = dataPlayer;

            bool isGameReady = false;

            try
            {
                await UniTask.WaitUntil(() => gameAPI.GameLoad)
                    .Timeout(TimeSpan.FromSeconds(20));
                isGameReady = true;
            }
            catch (TimeoutException)
            {
                Debug.LogWarning("[Bootstrap] ⏰ Timeout waiting for GameAPI.GameLoad. Continuing anyway...");
            }

            Debug.Log("[Bootstrap] 🚀 Starting bootstrap flow");

            var openedScenes = _dataPlayer.PlayerData?.FinishedRegionsName ?? new List<string> { Constants.Scenes.FirstBattleScene };
            Debug.Log($"[Bootstrap] Preloading opened scenes: {string.Join(", ", openedScenes)}");

            _preloader.SetSceneKeys(openedScenes);

            await _preloader.Load(progress =>
            {
                Debug.Log($"[Bootstrap] 🎯 Preloading opened scenes... {(progress * 100f):F0}%");
            });

            Debug.Log("[Bootstrap] ✅ Opened scenes preloading complete");

            await UniTask.WaitUntil(() => _preloader.IsPreloadingComplete);
            Debug.Log($"[Bootstrap] IsPreloadingComplete: {_preloader.IsPreloadingComplete}, SuccessfullyPreloaded: {string.Join(", ", _preloader.GetPreloadedKeys().Where(k => _preloader.WasSceneSuccessfullyPreloaded(k)))}");

            _preloader.PreloadRemainingScenesInBackground().Forget();
        }
    }
}