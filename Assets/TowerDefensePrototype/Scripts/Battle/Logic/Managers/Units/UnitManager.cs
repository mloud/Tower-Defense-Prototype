using System.Collections.Generic;
using System.Linq;
using CastlePrototype.Battle.Logic.Components;
using CastlePrototype.Battle.Logic.Managers;
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