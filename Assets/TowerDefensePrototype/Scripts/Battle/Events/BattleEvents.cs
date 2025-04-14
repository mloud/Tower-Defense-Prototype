namespace CastlePrototype.Battle.Events
{
    public static class BattlePointsChanged
    {
        public static BattleEvent<int, int> Event = new();
    }

    public static class WaveCounterChanged
    {
        public static BattleEvent<int, int> Event = new();
    }

    public static class BattleTimeChanged
    {
        public static BattleEvent<float> Event = new();
    }
    
    public static class PlayerHpChanged
    {
        public static BattleEvent<float, float> Event = new();
    }
    
    public static class StageChanged
    {
        public static BattleEvent<string, int> Event = new();
    }
}