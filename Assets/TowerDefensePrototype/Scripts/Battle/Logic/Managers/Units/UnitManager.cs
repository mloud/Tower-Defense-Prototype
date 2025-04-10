using System.Collections.Generic;
using System.Linq;
using CastlePrototype.Battle.Logic.Components;
using CastlePrototype.Battle.Logic.Managers;
using CastlePrototype.Battle.Visuals;
using CastlePrototype.Data.Definitions;
using Cysharp.Threading.Tasks;
using OneDay.Core;
using OneDay.Core.Modules.Data;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace TowerDefensePrototype.Scripts.Battle.Logic.Managers.Units
{
    public class UnitManager : WorldManager
    {
        private IDataManager dataManager;
        protected Dictionary<string, HeroDefinition> heroDefinitions;
        protected Dictionary<string, EnemyDefinition> enemyDefinitions;

        public UnitManager(World world) : base(world)
        {
            dataManager = ServiceLocator.Get<IDataManager>();
        }

        protected override async UniTask OnInitialize()
        {
            var heroes = await dataManager.GetAll<HeroDefinition>();
            heroDefinitions = heroes.ToDictionary(x => x.UnitId, x => x);
            var enemies = await dataManager.GetAll<EnemyDefinition>();
            enemyDefinitions = enemies.ToDictionary(x => x.UnitId, x => x);
        }

        public Entity CreateBarricade(ref SystemState state, float3 position, string definitionId)
        {
            if (!heroDefinitions.TryGetValue(definitionId, out var definition))
            {
                Debug.Assert(false, $"No such barricade definition with id {definitionId} exists");
                return Entity.Null;
            }

            var barricadeEntity = state.EntityManager.CreateEntity();
            state.EntityManager.AddComponentData(barricadeEntity, new HpComponent { Hp = definition.Hp, MaxHp = definition.Hp });
            state.EntityManager.AddComponentData(barricadeEntity, new LocalTransform { Position = VisualManager.Default.GetObjectPosition("barricade")});
            state.EntityManager.AddComponentData(barricadeEntity, new TeamComponent { Team = Team.Player });
            state.EntityManager.AddComponentData(barricadeEntity, new SettingComponent { DistanceAxes = new float3(0, 0, 1) });
            // mark this entity that it has already visual..Visual is part of the environment
            state.EntityManager.AddComponentData(barricadeEntity, new VisualComponent { HasVisual = true});
            state.EntityManager.AddComponentData(barricadeEntity, new BarricadeComponent());
            state.EntityManager.AddComponentData(barricadeEntity, new UnitComponent {DefinitionId = definitionId, UnitType = UnitType.Barricade});


            return barricadeEntity;
        }
        

        public Entity CreateHeroUnit(ref EntityCommandBuffer ecb, float3 position, string heroId)
        {
            if (heroDefinitions.TryGetValue(heroId, out var definition))
            {
                var entity = CreateUnit(ref ecb, position, definition, Team.Player);
                ecb.AddComponent(entity, new LookAtTargetComponent());
                return entity;
            }
            

            Debug.Assert(false, $"No such hero definition with id {heroId} exists");
            return Entity.Null;
        }
        
        public Entity CreateEnemyUnit(ref EntityCommandBuffer ecb, float3 position, string heroId)
        {
            if (enemyDefinitions.TryGetValue(heroId, out var definition))
            {
                return CreateUnit(ref ecb, position, definition, Team.Enemy);
            }

            Debug.Assert(false, $"No such enemy definition with id {heroId} exists");
            return Entity.Null;
        }

        private Entity CreateUnit(ref EntityCommandBuffer ecb, float3 position, HeroDefinition definition, Team team)
        {
            var entity = ecb.CreateEntity();
            if (definition.MoveSpeed > 0)
            {
                ecb.AddComponent(entity, new MovementComponent { Speed = definition.MoveSpeed, MaxSpeed = definition.MoveSpeed});
            }

            if (definition.Hp > 0)
            {
                ecb.AddComponent(entity, new HpComponent { Hp = definition.Hp, MaxHp = definition.Hp});
            }
            
            ecb.AddComponent(entity, new UnitComponent { DefinitionId = definition.UnitId });
            ecb.AddComponent(entity, new LocalTransform { Position = position });
            ecb.AddComponent(entity, new TeamComponent { Team = team });
            ecb.AddComponent(entity, new VisualComponent { VisualId = definition.VisualId });
            ecb.AddComponent(entity, new SettingComponent { DistanceAxes = new float3(1, 0, 1) });
            ecb.AddComponent(entity, new AttackComponent
            {
                // HEROES ARE ALWAYS RANGED
                AttackType = definition.AttackType,
                AttackDamage = definition.Damage,
                AttackDistance = definition.AttackDistance < 0 ? 999:definition.AttackDistance,
                Cooldown = definition.Cooldown,
                TargetRange = definition.TargetRange < 0 ? 999:definition.TargetRange,
                AoeRadius = definition.AoeRadius,
                AoeDamage = definition.AoeDamage,
                Bounce = definition.Bounce,
                FireAgain = definition.Fireagain,
                KnockBack = definition.Knockback,
                Penetration = definition.Penetration,
                AoeOnly = definition.AoeOnly,
                ProjectileVisualId = definition.ProjectileVisualId,
                ProjectileSpeed = definition.ProjectileSpeed,
                FireAgainInterval =definition.FireAgainInterval,
                AttackAnimDelay =  definition.AttackDelay
            });

            if (definition.KnockbackResistent)
            {
                ecb.AddComponent(entity, new KnockBackResistent());
            }
            return entity;
        }

        protected override void OnRelease()
        { }
    }
}