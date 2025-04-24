using TowerDefense.Battle.Logic.Systems;
using Unity.Entities;

namespace TowerDefense.Battle.Logic.EcsUtils
{
    public static class PauseUtils
    {
        public static void SetLogicPaused(bool isPaused, bool keepDestroySystem = false)
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
            world.GetExistingSystemState<LookAtTargetSystem>().Enabled = !isPaused;
            world.GetExistingSystemState<VisualObjectSystem>().Enabled = !isPaused;
            if (!keepDestroySystem)
            {
                world.GetExistingSystemState<DestroyEntitySystem>().Enabled = !isPaused;
            }

            world.GetExistingSystemState<ProjectileSystem>().Enabled = !isPaused;
        }
    }
}