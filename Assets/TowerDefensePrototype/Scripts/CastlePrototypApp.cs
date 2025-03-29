using CastlePrototype.Data;
using CastlePrototype.Data.Definitions;
using CastlePrototype.Managers;
using CastlePrototype.States;
using Cysharp.Threading.Tasks;
using OneDay.Core;
using OneDay.Core.Modules.Assets;
using OneDay.Core.Modules.Data;
using OneDay.Core.Modules.Sm;
using OneDay.Core.Modules.Ui;
using OneDay.Core.Modules.Update;
using UnityEngine;

public class CastlePrototypApp : ABaseApp
{
    [SerializeField] private UiManager uiManager;
    [SerializeField] private DataManager dataManager;
    [SerializeField] private AssetManager assetManager;
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private UpdateManager updateManager;
    
    protected override async UniTask RegisterServices()
    {
        ServiceLocator.Register<IUiManager>(uiManager);
        ServiceLocator.Register<IDataManager>(dataManager);
        ServiceLocator.Register<IAssetManager>(assetManager);
        ServiceLocator.Register<IPlayerManager>(playerManager);
        ServiceLocator.Register<IUpdateManager>(updateManager);

        
        var initializeTasks = ServiceLocator.GetAll().Select(x => x.Initialize());
        await UniTask.WhenAll(initializeTasks);
        
        var postInitializeTasks = ServiceLocator.GetAll().Select(x => x.PostInitialize());
        await UniTask.WhenAll(postInitializeTasks);
    }

    protected override async UniTask SetApplicationStateMachine()
    {
        StateMachineEnvironment.UnregisterAll();
        StateMachineEnvironment.RegisterStateMachine("Application", new StateMachine(), true);
            
        await StateMachineEnvironment.Default.RegisterState<BootState>();
        await StateMachineEnvironment.Default.RegisterState<MenuState>();
        await StateMachineEnvironment.Default.RegisterState<GameState>();
        await StateMachineEnvironment.Default.SetStateAsync<BootState>();
    }

    protected override async UniTask OnBoot()
    {
        // storages
        ServiceLocator.Get<IDataManager>().RegisterStorage<PlayerProgress>(new LocalStorage());
        ServiceLocator.Get<IDataManager>().RegisterStorage<WeaponDeck>(new LocalStorage());
        ServiceLocator.Get<IDataManager>().RegisterStorage<HeroDeck>(new LocalStorage());
        ServiceLocator.Get<IDataManager>().RegisterStorage<Player>(new LocalStorage());

        ServiceLocator.Get<IDataManager>().RegisterStorage<EnemyDefinition>(new AddressableScriptableObjectStorage());
        ServiceLocator.Get<IDataManager>().RegisterStorage<HeroDefinition>(new AddressableScriptableObjectStorage());
        ServiceLocator.Get<IDataManager>().RegisterStorage<WeaponDefinition>(new AddressableScriptableObjectStorage());

        // storage bindings   
        ServiceLocator.Get<IDataManager>().RegisterTypeToKeyBinding<PlayerProgress>(TypeToDataKeyBinding.PlayerProgress);
        ServiceLocator.Get<IDataManager>().RegisterTypeToKeyBinding<Player>(TypeToDataKeyBinding.Player);
        ServiceLocator.Get<IDataManager>().RegisterTypeToKeyBinding<WeaponDeck>(TypeToDataKeyBinding.WeaponDeck);
        ServiceLocator.Get<IDataManager>().RegisterTypeToKeyBinding<HeroDeck>(TypeToDataKeyBinding.HeroDeck);

        ServiceLocator.Get<IDataManager>().RegisterTypeToKeyBinding<EnemyDefinition>(TypeToDataKeyBinding.EnemyDefinitionsTable);
        ServiceLocator.Get<IDataManager>().RegisterTypeToKeyBinding<HeroDefinition>(TypeToDataKeyBinding.HeroDefinitionsTable);
        ServiceLocator.Get<IDataManager>().RegisterTypeToKeyBinding<WeaponDefinition>(TypeToDataKeyBinding.WeaponDefinitionsTable);
      
        await ServiceLocator.Get<IPlayerManager>().InitializePlayer();
    }
}
