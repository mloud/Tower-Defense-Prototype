using OneDay.Core.Modules.Data;
using UnityEngine;

namespace TowerDefense.Data.Definitions
{
    [CreateAssetMenu(fileName = "HeroDefinitionsTable", menuName = "ScriptableObjects/HeroDefinitionsTable", order = 1)]

    public class HeroDefinitionsTable : ScriptableObjectTable<HeroDefinition>
    {
    }
}