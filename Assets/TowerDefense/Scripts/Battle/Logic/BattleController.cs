using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using OneDay.Core;
using TowerDefense.Battle.Logic.Managers;
using TowerDefense.Battle.Logic.Managers.Skills;
using TowerDefense.Battle.Logic.Managers.Slots;
using TowerDefense.Battle.Logic.Systems;
using TowerDefense.Battle.Visuals;
using TowerDefense.Managers;
using TowerDefensePrototype.Scripts.Battle.Logic.Managers.Ui;
using TowerDefensePrototype.Scripts.Battle.Logic.Managers.Units;
using Unity.Entities;
using UnityEngine;

namespace TowerDefense.Battle.Logic
{
    public class BattleController : IDisposable
    {
        private List<SystemHandle> systemHandles;

        public async UniTask InitializeBattle(int stageIndex)
        {
            systemHandles = new List<SystemHandle>();
            var world = World.DefaultGameObjectInjectionWorld;
            var rootSystemGroup = world.GetExistingSystemManaged<SimulationSystemGroup>();

            BattleInitializeSystem.stage = stageIndex;
            BattleInitializeSystem.stageName = (await ServiceLocator.Get<IPlayerManager>().StageGetter.GetStageDefinition(stageIndex)).StageName;
            WorldManagers.Register(world, new SkillManager(world));
            WorldManagers.Register(world, new SlotManager(world));
            WorldManagers.Register(world, new BattleEventsManager(world));
            WorldManagers.Register(world, new UnitManager(world));
            WorldManagers.Register(world, new StageManager(world));
            WorldManagers.Register(world, new UiHelperManager(world));

            WorldManagers.DefaultWorld = world;
            await WorldManagers.Initialize(world);
            
            
            rootSystemGroup.AddSystemToUpdateList(world.CreateSystem<VisualGroup>());

            systemHandles.Add(world.GetOrCreateSystem<BattleInitializeSystem>());
            systemHandles.Add(world.GetOrCreateSystem<EnemySpawnerSystem>());
            systemHandles.Add(world.GetOrCreateSystem<MovementSystem>());
            systemHandles.Add(world.GetOrCreateSystem<ManualTargetingSystem>());
            systemHandles.Add(world.GetOrCreateSystem<TargetingSystem>());
            systemHandles.Add(world.GetOrCreateSystem<LookAtTargetSystem>());
            systemHandles.Add(world.GetOrCreateSystem<AttackSystem>());
            systemHandles.Add(world.GetOrCreateSystem<ProjectileSystem>());
            systemHandles.Add(world.GetOrCreateSystem<DamageSystem>());
            systemHandles.Add(world.GetOrCreateSystem<VisualObjectSystem>());
            systemHandles.Add(world.GetOrCreateSystem<VisualEffectSystem>());
            systemHandles.Add(world.GetOrCreateSystem<DestroyEntitySystem>());
            systemHandles.Add(world.GetOrCreateSystem<BattleProgressionSystem>());
            systemHandles.Add(world.GetOrCreateSystem<BattleResolveSystem>());
            systemHandles.Add(world.GetOrCreateSystem<EventSystem>());

            systemHandles.ForEach(rootSystemGroup.AddSystemToUpdateList);
            //simulationGroup.SortSystems();

        }

        public void Dispose()
        {
            Debug.Log("🏳️ Battle Ended! Removing systems and clearing entities...");
    
            var world = World.DefaultGameObjectInjectionWorld;
            var simulationGroup = world.GetExistingSystemManaged<SimulationSystemGroup>();

            systemHandles.ForEach(simulationGroup.RemoveSystemFromUpdateList);
            systemHandles.ForEach(world.DestroySystem);
            world.EntityManager.DestroyEntity(world.EntityManager.UniversalQuery);
            WorldManagers.Clear(world);
            
            VisualManager.Default.Dispose();
        }
    }
}