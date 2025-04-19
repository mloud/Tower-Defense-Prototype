using OneDay.Core.Modules.Data;
using UnityEditor;
using UnityEngine;

namespace CastlePrototype.Data.Definitions
{
    [CreateAssetMenu(fileName = "EnemyDefinitionsTable", menuName = "ScriptableObjects/EnemyDefinitionsTable",
        order = 1)]

    public class EnemyDefinitionsTable : ScriptableObjectTable<EnemyDefinition>
    {
    }
}