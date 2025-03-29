using System;
using System.Collections.Generic;
using CastlePrototype.Battle.Logic.Managers;
using CastlePrototype.Battle.Logic.Managers.Skills;
using CastlePrototype.Battle.Logic.Managers.Slots;
using CastlePrototype.Battle.Logic.Systems;
using CastlePrototype.Data;
using Cysharp.Threading.Tasks;
using Unity.Entities;
using UnityEngine;

namespace CastlePrototype.Battle.Logic
{
    public class BattleController : IDisposable
    {
        private List<SystemHandle> systemHandles;

        public async UniTask InitializeBattle()
        {
            systemHandles = new List<SystemHandle>();
            var world = World.DefaultGameObjectInjectionWorld;
            var rootSystemGroup = world.GetExistingSystemManaged<SimulationSystemGroup>();
            rootSystemGroup.AddSystemToUpdateList(world.CreateSystem<VisualGroup>());

            systemHandles.Add(world.GetOrCreateSystem<BattleInitializeSystem>());
            systemHandles.Add(world.GetOrCreateSystem<EnemySpawnerSystem>());
            systemHandles.Add(world.GetOrCreateSystem<MovementSystem>());
            systemHandles.Add(world.GetOrCreateSystem<TargetingSystem>());
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

            WorldManagers.Register(world, new SkillManager(world));
            WorldManagers.Register(world, new SlotManager(world));
            WorldManagers.Register(world, new BattleEventsManager(world));
            WorldManagers.DefaultWorld = world;
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
        }
    }
}