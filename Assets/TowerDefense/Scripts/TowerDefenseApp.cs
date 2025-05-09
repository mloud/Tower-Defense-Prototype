using TowerDefense.Data;
using TowerDefense.Data.Definitions;
using TowerDefense.Data.Progress;
using TowerDefense.Managers;
using TowerDefense.States;
using Core.Modules.Ui.Loading;
using Cysharp.Threading.Tasks;
using Firebase;
using Firebase.Extensions;
using OneDay.Core;
using OneDay.Core.Modules.Analytics;
using OneDay.Core.Modules.Assets;
using OneDay.Core.Modules.Audio;
using OneDay.Core.Modules.Data;
using OneDay.Core.Modules.Performance;
using OneDay.Core.Modules.Pooling;
using OneDay.Core.Modules.Settings;
using OneDay.Core.Modules.Sm;
using OneDay.Core.Modules.Ui;
using OneDay.Core.Modules.Update;
using TowerDefense.Managers.Simulation;
using UnityEngine;


namespace TowerDefense
{
    public class TowerDefenseApp : ABaseApp
    {
        [SerializeField] private bool developmentMode;
        [SerializeField] private UiManager uiManager;
        [SerializeField] private DataManager dataManager;
        [SerializeField] private AssetManager assetManager;
        [SerializeField] private PlayerManager playerManager;
        [SerializeField] private UpdateManager updateManager;
        [SerializeField] private AudioManager audioManager;
        [SerializeField] private SettingsManager settingsManager;
        [SerializeField] private PerformanceManager performanceManager;
        [SerializeField] private PoolManager poolManager;
        [SerializeField] private RemoteConfigManager remoteConfigManager;
        [SerializeField] private BufferedEventsManager bufferedEventsManager;
        [SerializeField] private AutomaticPlayManager automaticPlayManager;
        [SerializeField] private LoadingLayer loadingLayer;
        


        protected override async UniTask RegisterServices()
        {
            ServiceLocator.Register<IUiManager>(uiManager);
            ServiceLocator.Register<IDataManager>(dataManager);
            ServiceLocator.Register<IAssetManager>(assetManager);
            ServiceLocator.Register<IPlayerManager>(playerManager);
            ServiceLocator.Register<IUpdateManager>(updateManager);
            ServiceLocator.Register<IAudioManager>(audioManager);
            ServiceLocator.Register<ISettingsManager>(settingsManager);
            ServiceLocator.Register<IPerformanceManager>(performanceManager);
            ServiceLocator.Register<IPoolManager>(poolManager);
            ServiceLocator.Register<IBufferedEventsManager>(bufferedEventsManager);
            ServiceLocator.Register<IRemoteConfigManager>(remoteConfigManager);
            ServiceLocator.Register<IAutomaticPlayManager>(automaticPlayManager);
            ServiceLocator.Register<ILoading>(loadingLayer);
            ServiceLocator.Register<ISimulationMode>(automaticPlayManager);

            remoteConfigManager.SetRemoteConfigServiceServices(new FirebaseRemoteConfigService());
            
            var initializeTasks = ServiceLocator.GetAll().Select(x => x.Initialize());
            await UniTask.WhenAll(initializeTasks);

            await InitializeStorages();
            await ServiceLocator.Get<IPlayerManager>().InitializePlayer();

            var postInitializeTasks = ServiceLocator.GetAll().Select(x => x.PostInitialize());
            await UniTask.WhenAll(postInitializeTasks);

            settingsManager.RegisterModule<IVolumeModule>(new VolumeModule());
            ServiceLocator.Get<IAudioManager>().MusicVolume = -20f;
        }

        protected override async UniTask SetApplicationStateMachine()
        {
            StateMachineEnvironment.UnregisterAll();
            StateMachineEnvironment.RegisterStateMachine("Application", new StateMachine(), true);

            await StateMachineEnvironment.Default.RegisterState<BootState>();
            await StateMachineEnvironment.Default.RegisterState<MenuState>();
            await StateMachineEnvironment.Default.RegisterState<GameState>();
            await StateMachineEnvironment.Default.RegisterState<LibraryState>();
            await StateMachineEnvironment.Default.SetStateAsync<BootState>();
        }

        private async UniTask InitializeStorages()
        {
            // storages
            ServiceLocator.Get<IDataManager>().RegisterStorage<PlayerProgress>(new LocalStorage());
            ServiceLocator.Get<IDataManager>().RegisterStorage<HeroDeck>(new LocalStorage());
            ServiceLocator.Get<IDataManager>().RegisterStorage<Valet>(new LocalStorage());
            
            ServiceLocator.Get<IDataManager>().RegisterStorage<StageDefinition>(new AddressableScriptableObjectStorage());
            //ServiceLocator.Get<IDataManager>().RegisterStorage<StageDefinition>(new RemoteReadOnlyStorage(developmentMode));

            ServiceLocator.Get<IDataManager>().RegisterStorage<EnemyDefinition>(new AddressableScriptableObjectStorage());
            ServiceLocator.Get<IDataManager>().RegisterStorage<HeroDefinition>(new AddressableScriptableObjectStorage());
            ServiceLocator.Get<IDataManager>().RegisterStorage<PlayerProgressionDefinition>(new AddressableScriptableObjectStorage());

            // storage bindings   
            ServiceLocator.Get<IDataManager>().RegisterTypeToKeyBinding<PlayerProgress>(TypeToDataKeyBinding.PlayerProgress);
            ServiceLocator.Get<IDataManager>().RegisterTypeToKeyBinding<HeroDeck>(TypeToDataKeyBinding.HeroDeck);
            ServiceLocator.Get<IDataManager>().RegisterTypeToKeyBinding<Valet>(TypeToDataKeyBinding.Valet);
            
            ServiceLocator.Get<IDataManager>().RegisterTypeToKeyBinding<StageDefinition>(TypeToDataKeyBinding.StageDefinitionsTable);
            ServiceLocator.Get<IDataManager>().RegisterTypeToKeyBinding<EnemyDefinition>(TypeToDataKeyBinding.EnemyDefinitionsTable);
            ServiceLocator.Get<IDataManager>().RegisterTypeToKeyBinding<HeroDefinition>(TypeToDataKeyBinding.HeroDefinitionsTable);
            ServiceLocator.Get<IDataManager>().RegisterTypeToKeyBinding<PlayerProgressionDefinition>(TypeToDataKeyBinding.PlayerProgressionDefinitionTable);
        }



        protected override async UniTask OnBoot()
        {
            await FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
                var dependencyStatus = task.Result;
                if (dependencyStatus == DependencyStatus.Available)
                {
                    Debug.Log($"Firebase Initialized");

                }
                else
                {
                    Debug.LogError($"Could not resolve all Firebase dependencies: {dependencyStatus}");
                }
            });

            ServiceLocator.Get<IAutomaticPlayManager>().Play();
        }
    }
}
