using CastlePrototype.Battle.Logic.Components;
using TowerDefensePrototype.Scripts.Battle.Logic.Managers.Units;
using Unity.Entities;
using Unity.Mathematics;

namespace CastlePrototype.Battle.Logic.Managers.Skills
{
    public class PlaceTrapSkill : ASkill
    {
        public PlaceTrapSkill(string name, float value, string description, string trapId) 
            : base(name, description, value)
        {
            SkillType = SkillType.PlaceTrap;
            NeedsUnit = false;
            DefinitionId = trapId;
        }

        public override void Apply(EntityManager entityManager)
        {
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            var battleFieldC = entityManager.CreateEntityQuery(typeof(BattleFieldComponent))
                .GetSingleton<BattleFieldComponent>();

            for (int i = 0; i < Value; i++)
            {
                var rndPosition = GetRandomPositionForTrap(battleFieldC);
                var trapEntity = WorldManagers.Get<UnitManager>(AttachedWorld)
                    .CreateTrap(ref ecb, rndPosition, DefinitionId, Team.Player);
            }

            ecb.Playback(entityManager);
            ecb.Dispose();
        }

        private float3 GetRandomPositionForTrap(BattleFieldComponent battleFieldC)
        {
            var center = Utils.GetCenter(battleFieldC.MinCorner, battleFieldC.MaxCorner);
            float width = battleFieldC.MaxCorner.x - battleFieldC.MinCorner.x;
            float height = battleFieldC.MaxCorner.y - battleFieldC.MinCorner.y;

            // use just 70 percent of width and height
            width *= 0.7f;
            height *= 0.5f;
            
            return new float3(
                UnityEngine.Random.Range(center.x - width / 2, center.x + width / 2),
                0,
                UnityEngine.Random.Range(center.y - height / 2, center.y + height / 2));
        }
    }
}