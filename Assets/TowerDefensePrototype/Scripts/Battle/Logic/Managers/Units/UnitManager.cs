using System.Collections.Generic;
using System.Linq;
using CastlePrototype.Battle.Logic.Components;
using CastlePrototype.Battle.Logic.Managers;
using CastlePrototype.Data;
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

        public UnitManager(World world) : base(world)
        {
            dataManager = ServiceLocator.Get<IDataManager>();
        }

        protected override async UniTask OnInitialize()
        {
            var heroes = await dataManager.GetAll<HeroDefinition>();
            heroDefinitions = heroes.ToDictionary(x => x.UnitId, x => x);
        }

        public Entity CreateHeroUnit(ref EntityCommandBuffer ecb, float3 position, string heroId)
        {
            if (heroDefinitions.TryGetValue(heroId, out var definition))
            {
                return CreateHeroUnit(ref ecb, position, definition);
            }

            Debug.Assert(false, $"No such hero definition with id {heroId} exists");
            return Entity.Null;
        }

        private Entity CreateHeroUnit(ref EntityCommandBuffer ecb, float3 position, HeroDefinition definition)
        {
            var soldierEntity = ecb.CreateEntity();
            ecb.AddComponent(soldierEntity, new UnitComponent { DefinitionId = definition.UnitId });
            ecb.AddComponent(soldierEntity, new LocalTransform { Position = position });
            ecb.AddComponent(soldierEntity, new TeamComponent { Team = Team.Player });
            ecb.AddComponent(soldierEntity, new VisualComponent { VisualId = definition.VisualId });
            ecb.AddComponent(soldierEntity, new SettingComponent { DistanceAxes = new float3(1, 0, 1) });
            ecb.AddComponent(soldierEntity, new AttackComponent
            {
                // HEROES ARE ALWAYS RANGED
                AttackType = AttackType.Range,
                AttackDamage = definition.Damage,
                AttackDistance = definition.AttackDistance < 0 ? 999:definition.AttackDistance,
                Cooldown = definition.Cooldown,
                TargetRange = definition.TargetRange < 0 ? 999:definition.TargetRange,
                AoeRadius = definition.AoeRadius,
                AoeDamage = definition.AoeDamage,
                Bounce = definition.Bounce,
                Fireagain = definition.Fireagain,
                Knockback = definition.Knockback,
                
                ProjectileVisualId = definition.ProjectileVisualId, //"projectile_rocket"
                ProjectileSpeed = definition.ProjectileSpeed,
            });
            return soldierEntity;
        }

        protected override void OnRelease()
        { }
    }
}