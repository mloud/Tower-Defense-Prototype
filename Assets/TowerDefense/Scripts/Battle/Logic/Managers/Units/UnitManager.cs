using System.Collections.Generic;
using System.Linq;
using TowerDefense.Battle.Logic.Components;
using TowerDefense.Battle.Logic.Managers;
using TowerDefense.Battle.Visuals;
using TowerDefense.Data;
using TowerDefense.Data.Definitions;
using TowerDefense.Data.Progress;
using TowerDefense.Managers;
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
        protected HeroDeck heroDeck;

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

            heroDeck = await ServiceLocator.Get<IPlayerManager>().GetHeroDeck();
        }

        public Entity CreateBarricade(ref SystemState state, float3 position)
        {
            string definitionId = "barricade";
            
            
            if (!heroDefinitions.TryGetValue(definitionId, out var definition))
            {
                Debug.Assert(false, $"No such barricade definition with id {definitionId} exists");
                return Entity.Null;
            }

            var level = heroDeck.Heroes[definitionId].Level;
            
            var barricadeEntity = state.EntityManager.CreateEntity();
            state.EntityManager.AddComponentData(barricadeEntity, CreateHpComponent(definition, level));
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
                int level = heroDeck.Heroes[heroId].Level;
                
                var entity = CreateUnit(ref ecb, position, definition, Team.Player, level);
                ecb.AddComponent(entity, new LookAtTargetComponent());
                if (heroId == "weapon")
                {
                    ecb.AddComponent<WeaponComponent>(entity);
                }
                return entity;
            }
            

            Debug.Assert(false, $"No such hero definition with id {heroId} exists");
            return Entity.Null;
        }
        
        public Entity CreateEnemyUnit(ref EntityCommandBuffer ecb, float3 position, string heroId)
        {
            int level = 1;
            if (enemyDefinitions.TryGetValue(heroId, out var definition))
            {
                return CreateUnit(ref ecb, position, definition, Team.Enemy, level);
            }

            Debug.Assert(false, $"No such enemy definition with id {heroId} exists");
            return Entity.Null;
        }

        private Entity CreateUnit(ref EntityCommandBuffer ecb, float3 position, HeroDefinition definition, Team team, int level)
        {
            var entity = ecb.CreateEntity();
            if (definition.MoveSpeed > 0)
            {
                ecb.AddComponent(entity, new MovementComponent { Speed = definition.MoveSpeed, MaxSpeed = definition.MoveSpeed});
            }

           
            if (team == Team.Player)
            {
                level = heroDeck.Heroes[definition.UnitId].Level;
            }
            
            var hp = definition.GetLeveledHeroStat(StatUpgradeType.Hp, level);
            
            if (hp > 0)
            {
                ecb.AddComponent(entity, CreateHpComponent(definition, level));
            }
            
            ecb.AddComponent(entity, new UnitComponent { DefinitionId = definition.UnitId });
            ecb.AddComponent(entity, new LocalTransform { Position = position });
            ecb.AddComponent(entity, new TeamComponent { Team = team });
            ecb.AddComponent(entity, new VisualComponent { VisualId = definition.VisualId,Level = level});
            ecb.AddComponent(entity, new SettingComponent { DistanceAxes = new float3(1, 0, 1) });
            ecb.AddComponent(entity, new AttackComponent
            {
                // HEROES ARE ALWAYS RANGED
                AttackType = definition.AttackType,
                AttackDamage = definition.GetLeveledHeroStat(StatUpgradeType.Damage, level),
                AttackDistance = definition.GetLeveledHeroStat(StatUpgradeType.AttackDistance, level) < 0 ? 999:definition.GetLeveledHeroStat(StatUpgradeType.AttackDistance, level),
                Cooldown = definition.GetLeveledHeroStat(StatUpgradeType.Cooldown, level),
                TargetRange = definition.GetLeveledHeroStat(StatUpgradeType.TargetRange, level) < 0 ? 999:definition.GetLeveledHeroStat(StatUpgradeType.TargetRange, level),
                AoeRadius = definition.GetLeveledHeroStat(StatUpgradeType.AoeRadius, level),
                AoeDamage = definition.GetLeveledHeroStat(StatUpgradeType.AoeDamage, level),
                Bounce = (int)definition.GetLeveledHeroStat(StatUpgradeType.Bounce, level),
                FireAgain = (int)definition.GetLeveledHeroStat(StatUpgradeType.FireAgain, level),
                FireAgainSpread = (int)definition.GetLeveledHeroStat(StatUpgradeType.FireAgainSpread, level),
                KnockBack = definition.Knockback,
                Penetration =(int)definition.GetLeveledHeroStat(StatUpgradeType.Penetration, level),
                AoeOnly = definition.AoeOnly,
                ProjectileVisualId = definition.ProjectileVisualId,
                ProjectileSpeed = definition.ProjectileSpeed,
                FireAgainInterval =definition.FireAgainInterval,
                AttackAnimDelay =  definition.AttackDelay,
            });

            if (definition.KnockbackResistent)
            {
                ecb.AddComponent(entity, new KnockBackResistent());
            }
            return entity;
        }
        
        public Entity CreateTrap(ref EntityCommandBuffer ecb, float3 position, string trapId, Team team)
        {
            if (!heroDefinitions.TryGetValue(trapId, out var definition))
            {
                Debug.Assert(false, $"No trap definition found {trapId}");
                return Entity.Null;
            }

            var entity = ecb.CreateEntity();
            Debug.Assert(definition.MoveSpeed == 0, "definition.MoveSpeed == 0");
            Debug.Assert(definition.AttackType == AttackType.None);
            Debug.Assert(team == Team.Player);

            var level = heroDeck.Heroes[definition.UnitId].Level;
            var hp = definition.GetLeveledHeroStat(StatUpgradeType.Hp, level);
            
            if (hp > 0)
            {
                ecb.AddComponent(entity, CreateHpComponent(definition, level));
            }
            
            ecb.AddComponent(entity, new UnitComponent { DefinitionId = definition.UnitId });
            ecb.AddComponent(entity, new LocalTransform { Position = position });
            ecb.AddComponent(entity, new TeamComponent { Team = team });
            ecb.AddComponent(entity, new VisualComponent { VisualId = definition.VisualId,Level = level});
            ecb.AddComponent(entity, new SettingComponent { DistanceAxes = new float3(1, 0, 1), Width = 1.12f, Radius = 0.2f,NeedsTouchToGetTargeted = true});
            ecb.AddComponent(entity, new TrapComponent());
            if (definition.KnockbackResistent)
            {
                ecb.AddComponent(entity, new KnockBackResistent());
            }
            return entity;
        }

        protected override void OnRelease()
        { }

        
        private HpComponent CreateHpComponent(HeroDefinition definition, int level) =>
            new()
            {
                Hp = definition.GetLeveledHeroStat(StatUpgradeType.Hp, level), 
                MaxHp = definition.GetLeveledHeroStat(StatUpgradeType.Hp, level)
            };
    }
}