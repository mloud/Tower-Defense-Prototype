using System;
using System.Collections.Generic;
using OneDay.Core.Extensions;
using TowerDefense.Data.Definitions;
using UnityEngine;


namespace TowerDefensePrototype.Data.Definitions.Stages.Generator.Editor
{
    public interface IStageGenerator
    {
        void Generate(StageGeneratorConfig stageGeneratorConfig,  StageDefinition stageDefinition);
    }
    public class StageGenerator : IStageGenerator
    {
        private readonly IUnitStats unitStats;

        public StageGenerator(IUnitStats unitStats) => this.unitStats = unitStats;


        public void ModifyByWaveTimingCurve(StageGeneratorConfig stageGeneratorConfig, StageDefinition stageDefinition)
        {
            SetWavesTiming(
                stageGeneratorConfig.MinDelayBetweenWaves, 
                stageGeneratorConfig.MaxDelayBetweenWaves,
                stageGeneratorConfig.DelayCurve, 
                stageDefinition);
        }
        public void Generate(StageGeneratorConfig stageGeneratorConfig, StageDefinition stageDefinition)
        {
            stageDefinition.Waves.Clear();
            
            GenerateEmptyWaves(stageGeneratorConfig.WavesCount, stageDefinition);
            AddRequiredWaves(stageGeneratorConfig.RequiredWaves, stageDefinition);
            SetWavesTiming(
                stageGeneratorConfig.MinDelayBetweenWaves, 
                stageGeneratorConfig.MaxDelayBetweenWaves,
                stageGeneratorConfig.DelayCurve, 
                stageDefinition);
            SetWavesMicroSpawnInterval(2, stageDefinition);
            SetWavesStats(
                stageGeneratorConfig.TotalHp,
                stageGeneratorConfig.TotalDps,
                stageGeneratorConfig.UnitsUsed,
                stageGeneratorConfig.UnitsProbabilities,
                stageDefinition, unitStats);
            
            SetReward(stageDefinition);
        }

        private void GenerateEmptyWaves(int count, StageDefinition stageDefinition)
        {
            stageDefinition.Waves = new List<WaveDefinition>();
            for (int i = 0; i < count; i++)
            {
                stageDefinition.Waves.Add(new WaveDefinition());
            }
        }

        private void AddRequiredWaves(IReadOnlyList<RequiredWave> requiredWaves, StageDefinition stageDefinition)
        {
            foreach (var requiredWave in requiredWaves)
            {
                if (requiredWave.WaveIndex >= stageDefinition.Waves.Count)
                {
                    throw new ArgumentException($"Required wave: {requiredWave.WaveIndex} exceeds number of waves");
                }

                stageDefinition.Waves[requiredWave.WaveIndex].EnemiesCount = requiredWave.EnemyCount;
                stageDefinition.Waves[requiredWave.WaveIndex].EnemyId = requiredWave.EnemyId;
            }
        }

        private void SetWavesTiming(float minDelay, float maxDelay, AnimationCurve delayCurve, StageDefinition stageDefinition)
        {
            float time = 0;
            for (int i = 0; i < stageDefinition.Waves.Count; i++)
            {
                var waveProgress = (float)i / stageDefinition.Waves.Count;
                stageDefinition.Waves[i].Time = time;
                var delay = UnityEngine.Random.Range(minDelay, maxDelay);
                delay *= delayCurve.Evaluate(waveProgress);
                time += delay;
            }
        }

        private void SetWavesMicroSpawnInterval(float microSpawnInterval, StageDefinition stageDefinition)
        {
            for (int i = 0; i < stageDefinition.Waves.Count; i++)
            {
                stageDefinition.Waves[i].SpawnInterval = microSpawnInterval;
            }
        }
        
        private void SetWavesStats(
            float totalHp, 
            float totalDps, 
            IReadOnlyList<string> unitIds,
            List<float> unitsProbabilities,
            StageDefinition stageDefinition,
            IUnitStats unitStats)
        {
            if (stageDefinition.Waves == null || stageDefinition.Waves.Count == 0)
                return;

            float hpPerWave = totalHp / stageDefinition.Waves.Count;
            float dpsPerWave = totalDps / stageDefinition.Waves.Count;

            foreach (var wave in stageDefinition.Waves)
            {
                if (!string.IsNullOrEmpty(wave.EnemyId) && wave.EnemiesCount > 0)
                    continue;

                // string selectedUnit = unitIds[UnityEngine.Random.Range(0, unitIds.Count)];

                var selectedUnit = unitIds.GetRandomWithProbabilities(unitsProbabilities);
                float unitHp = unitStats.GetUnitHp(selectedUnit);
                float unitDps = unitStats.GetUnitDps(selectedUnit);

                int countByHp = Mathf.CeilToInt(hpPerWave / unitHp);
                int countByDps = Mathf.CeilToInt(dpsPerWave / unitDps);
                int enemiesCount = Mathf.Max(countByHp, countByDps);

                wave.EnemyId = selectedUnit;
                wave.EnemiesCount = enemiesCount;
            }
        }

        private void SetReward(StageDefinition stageDefinition)
        {
            stageDefinition.Reward = new StageReward();
        }
    }
}