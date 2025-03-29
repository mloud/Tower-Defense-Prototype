using CastlePrototype.Battle.Logic.Systems;
using Unity.Entities;

namespace CastlePrototype.Battle.Logic.EcsUtils
{
    public static class PauseUtils
    {
        public static void SetLogicPaused(bool isPaused)
        {
            var world = World.DefaultGameObjectInjectionWorld.Unmanaged;
            world.GetExistingSystemState<AttackSystem>().Enabled = !isPaused;
            world.GetExistingSystemState<BattleInitializeSystem>().Enabled = !isPaused;
            world.GetExistingSystemState<BattleProgressionSystem>().Enabled = !isPaused;
            world.GetExistingSystemState<BattleResolveSystem>().Enabled = !isPaused;
            world.GetExistingSystemState<DamageSystem>().Enabled = !isPaused;
            world.GetExistingSystemState<EnemySpawnerSystem>().Enabled = !isPaused;
            world.GetExistingSystemState<MovementSystem>().Enabled = !isPaused;
            world.GetExistingSystemState<TargetingSystem>().Enabled = !isPaused;
            world.GetExistingSystemState<VisualObjectSystem>().Enabled = !isPaused;
            world.GetExistingSystemState<DestroyEntitySystem>().Enabled = !isPaused;
            world.GetExistingSystemState<ProjectileSystem>().Enabled = !isPaused;
        }
    }
}