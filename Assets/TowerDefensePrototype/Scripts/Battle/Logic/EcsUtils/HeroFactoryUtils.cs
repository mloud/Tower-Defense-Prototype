using CastlePrototype.Battle.Logic.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace CastlePrototype.Battle.Logic.EcsUtils
{
    public static class HeroFactoryUtils
    {
        public static Entity CreateHeroFromArchetype(ref EntityCommandBuffer ecb, string heroId, float3 position)
        {
            switch (heroId)
            {
                case "hero_tank" : 
                    return CreateArchetypeTank(ref ecb, position, heroId);
                case "hero_soldier_rifle": 
                    return CreateArchetypeSoldierRifle(ref ecb, position, heroId); ;
                case "weapon_default": 
                    return CreateArchetypeWeapon(ref ecb, position, heroId);
            }
            return Entity.Null;
        }
        
        private static Entity CreateArchetypeTank(ref EntityCommandBuffer ecb, float3 position, string heroId)
        {
            var soldierEntity = ecb.CreateEntity();
            ecb.AddComponent(soldierEntity, new UnitComponent{DefinitionId = heroId});
            ecb.AddComponent(soldierEntity, new LocalTransform { Position = position });
            ecb.AddComponent(soldierEntity, new TeamComponent { Team = Team.Player });
            ecb.AddComponent(soldierEntity, new VisualComponent { VisualId = heroId });
            ecb.AddComponent(soldierEntity, new SettingComponent { DistanceAxes = new float3(1, 0, 1) });
            ecb.AddComponent(soldierEntity, new AttackComponent
            {
                AttackDamage = 2,
                AttackDistance = 10,
                AttackInterval = 7,
                TargetRange = 13,
                AoeRadius = 2,
                AttackType = AttackType.Range,
                ProjectileVisualId = "projectile_rocket",
                ProjectileSpeed = 5
            });
            return soldierEntity;
        }
        
        private static Entity CreateArchetypeSoldierRifle(ref EntityCommandBuffer ecb, float3 position, string heroId)
        {
            var soldierEntity = ecb.CreateEntity();
            ecb.AddComponent(soldierEntity, new UnitComponent{DefinitionId = heroId});
            ecb.AddComponent(soldierEntity, new LocalTransform { Position = position});
            ecb.AddComponent(soldierEntity, new TeamComponent { Team = Team.Player });
            ecb.AddComponent(soldierEntity, new VisualComponent { VisualId = heroId });
            ecb.AddComponent(soldierEntity, new SettingComponent { DistanceAxes = new float3(1, 0, 1) });
            ecb.AddComponent(soldierEntity, new AttackComponent
            {
                AttackDamage = 1,
                AttackDistance = 7,
                AttackInterval = 3,
                TargetRange = 10,
                AttackType = AttackType.Range,
                ProjectileVisualId = "projectile_default",
                ProjectileSpeed = 13
            });
            return soldierEntity;
        }
        
        private static Entity CreateArchetypeWeapon(ref EntityCommandBuffer ecb, float3 position, string heroId)
        {
            var weaponEntity = ecb.CreateEntity();
            ecb.AddComponent(weaponEntity, new UnitComponent{DefinitionId = heroId});
            ecb.AddComponent(weaponEntity, new LocalTransform { Position = position });
            ecb.AddComponent(weaponEntity, new TeamComponent { Team = Team.Player });
            ecb.AddComponent(weaponEntity, new VisualComponent { VisualId = heroId });
            ecb.AddComponent(weaponEntity, new SettingComponent { DistanceAxes = new float3(1, 0, 1) });
            ecb.AddComponent(weaponEntity, new AttackComponent
            {
                AttackDamage = 2,
                AttackDistance = 15,
                AttackInterval = 4,
                TargetRange = 15,
                AttackType = AttackType.Range,
                ProjectileVisualId = "projectile_default",
                ProjectileSpeed = 6
            });
            return weaponEntity;
        }
        
        private static Entity CreateWall(ref EntityCommandBuffer ecb, string heroId)
        {
            var wallEntity = ecb.CreateEntity();
            ecb.AddComponent(wallEntity, new HpComponent { Hp = 5, MaxHp = 5 });
            ecb.AddComponent(wallEntity, new LocalTransform { Position = new float3(0, 0, -5) });
            ecb.AddComponent(wallEntity, new TeamComponent { Team = Team.Player });
            ecb.AddComponent(wallEntity, new SettingComponent { DistanceAxes = new float3(0, 0, 1) });
            ecb.AddComponent(wallEntity, new VisualComponent { VisualId = heroId });
            return wallEntity;
        }
    }
}