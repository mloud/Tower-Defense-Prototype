using CastlePrototype.Battle.Events;
using CastlePrototype.Battle.Logic.Components;
using CastlePrototype.Battle.Logic.Managers;
using Unity.Entities;


namespace CastlePrototype.Battle
{
    public class BattleEventsManager : WorldManager
    {
        public BattleEventsManager(World world) : base(world)
        { }
        
        public void UpdateBattlePoints(BattleProgressionComponent progressionC) => BattlePointsChanged.Event.Invoke(progressionC.BattlePoints, progressionC.BattlePointsNeeded);
        public void UpdateBattleTime(BattleProgressionComponent progressionC) => BattleTimeChanged.Event.Invoke(progressionC.Timer);
        public void UpdateWaveCounter(EnemySpawnerComponent spawnerC) => WaveCounterChanged.Event.Invoke(spawnerC.currentWave, spawnerC.totalWaves);
        public void UpdatePlayerHp(HpComponent hpC) => PlayerHpChanged.Event.Invoke(hpC.Hp, hpC.MaxHp);
 
        protected override void OnRelease() => BattlePointsChanged.Event.Clear();
    }
}