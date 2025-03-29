using OneDay.Core.Modules.Data;
using UnityEngine;

namespace CastlePrototype.Data.Definitions
{
    [CreateAssetMenu(fileName = "WeaponDefinitionsTable", menuName = "ScriptableObjects/WeaponDefinitionsTable", order = 1)]

    public class WeaponDefinitionsTable : ScriptableObjectTable<WeaponDefinition>
    { }
}