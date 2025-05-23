using OneDay.Core.Modules.Data;
using UnityEngine;

namespace CastlePrototype.Data.Definitions
{
    [CreateAssetMenu(fileName = "EnemyDefinitionsTable", menuName = "ScriptableObjects/EnemyDefinitionsTable", order = 1)]

    public class EnemyDefinitionsTable : ScriptableObjectTable<EnemyDefinition>
    { }
}