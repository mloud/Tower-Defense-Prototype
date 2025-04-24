using TowerDefense.Battle.Events;
using TowerDefense.Battle.Logic.Components;
using TowerDefense.Battle.Logic.Managers;
using Unity.Entities;


namespace TowerDefense.Battle
{
    public class BattleEventsManager : WorldManager
    {
        public BattleEventsManager(World world) : base(world)
        { }
        
        public void UpdateBattlePoints(BattleProgressionComponent progressionC) => BattlePointsChanged.Event.Invoke(progressionC.BattlePoints, progressionC.BattlePointsNeeded);
        public void UpdateBattleTime(BattleProgressionComponent progressionC) => BattleTimeChanged.Event.Invoke(progressionC.Timer);
        public void UpdateWaveCounter(EnemySpawnerComponent spawnerC) => WaveCounterChanged.Event.Invoke(spawnerC.currentWave, spawnerC.waves.Length);
        public void UpdatePlayerHp(HpComponent hpC) => PlayerHpChanged.Event.Invoke(hpC.Hp, hpC.MaxHp);
        public void UpdateStage(string stageName, int stageIndex) => StageChanged.Event.Invoke(stageName, stageIndex);
        protected override void OnRelease() => BattlePointsChanged.Event.Clear();
    }
}