using CastlePrototype.Battle.Logic.Systems;
using TowerDefensePrototype.Scripts.Data.Definitions.Editor.OneDay.Core.Modules.Data;

namespace CastlePrototype.Data.Definitions
{
    public static class StageDifficultyCalculator
    {
        public class StageDifficulty
        {
            public float TotalHp;
            public float TotalDps;
            public float Threat;
        }

        public static StageDifficulty Calculate(StageDefinition stageDefinition)
        {
            var enemyDefinitionsTable = TableLoader.Load<EnemyDefinitionsTable>();
            if (enemyDefinitionsTable == null)
            {
                UnityEngine.Debug.LogError("EnemyDefinitionsTable could not be loaded.");
                return new StageDifficulty();
            }

            var totalHp = 0f;
            var totalDps = 0f;
        

            foreach (var wave in stageDefinition.Waves)
            {
                var enemyDef = enemyDefinitionsTable.Data.Find(x => x.UnitId == wave.EnemyId);
                if (enemyDef == null)
                {
                    UnityEngine.Debug.LogWarning($"EnemyDefinition for UnitId {wave.EnemyId} not found.");
                    continue;
                }

                var enemyHp = enemyDef.GetLeveledHeroStat(StatUpgradeType.Hp, 1);
                var enemyDamage = enemyDef.GetLeveledHeroStat(StatUpgradeType.Damage, 1);
                var enemyCooldown = enemyDef.GetLeveledHeroStat(StatUpgradeType.Cooldown, 1);
                var enemyDps = (enemyCooldown > 0f) ? (enemyDamage / enemyCooldown) : 0f;

                totalHp += wave.EnemiesCount * enemyHp;
                totalDps += wave.EnemiesCount * enemyDps;
            }

            float levelDuration = 0;
            if (stageDefinition.Waves.Count > 0)
            {
                levelDuration = stageDefinition.Waves[^1].Time;
                levelDuration += ComputeWaveDuration(stageDefinition.Waves[^1]);
            }
           
            var threat = (levelDuration > 0f) ? (totalHp * totalDps / levelDuration) : 0f;

            return new StageDifficulty
            {
                TotalHp = totalHp,
                TotalDps = totalDps,
                Threat = threat
            };


            float ComputeWaveDuration(WaveDefinition wave)
            {
                return wave.EnemiesCount * wave.SpawnInterval
                       / ((EnemySpawnerSystem.MinEnemiesInMicroWave + EnemySpawnerSystem.MaxEnemiesInMicroWave) / 2.0f);
            }
        }
    }
}