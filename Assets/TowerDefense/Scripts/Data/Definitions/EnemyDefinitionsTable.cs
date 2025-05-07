using OneDay.Core.Modules.Data;
using UnityEngine;

namespace TowerDefense.Data.Definitions
{
    [CreateAssetMenu(fileName = "EnemyDefinitionsTable", menuName = "TD/Data/Enemy Definitions Table", order = 1)]
    public class EnemyDefinitionsTable : ScriptableObjectTable<EnemyDefinition>
    {
    }
}