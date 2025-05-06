namespace TowerDefensePrototype.Data.Definitions.Stages.Generator.Editor
{
    public interface IUnitStats
    {
        float GetUnitHp(string unitId);
        float GetUnitDps(string unitId);
    }
}