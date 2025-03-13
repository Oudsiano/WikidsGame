using System;
using System.Collections.Generic;
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
                Debug.LogWarning("[Bootstrap] ⏰ Timeout while waiting for GameAPI.GameLoad. Continuing anyway...");
            }

            Debug.Log("[Bootstrap] 🚀 Starting bootstrap flow");

            var openedScenes = _dataPlayer.PlayerData?.FinishedRegionsName;

            if (openedScenes == null || openedScenes.Count == 0)
            {
                Debug.LogWarning("[Bootstrap] FinishedRegionsName is null or empty, defaulting to FirstBattleScene");
                openedScenes = new List<string> { Constants.Scenes.FirstBattleScene };
            }
            else
            {
                Debug.Log($"[Bootstrap] Preloading opened scenes: {string.Join(", ", openedScenes)}");
            }

            _preloader.SetSceneKeys(openedScenes);

            await _preloader.Load(progress =>
            {
                Debug.Log($"[Bootstrap] 🎯 Preloading opened scenes... {(progress * 100f):F0}%");
            });

            Debug.Log("[Bootstrap] ✅ Opened scenes preloading complete");

            await UniTask.WaitUntil(() => _preloader.IsPreloadingComplete);
            Debug.Log(_preloader.IsPreloadingComplete + " QQQQ");

            _preloader.PreloadRemainingScenesInBackground().Forget();
        }
    }
}